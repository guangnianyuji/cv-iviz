﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Iviz.Msgs;
using Iviz.Tools;

namespace Iviz.Roslib;

using System;

/// <summary>
/// Manager for a subscription to a ROS topic.
/// </summary>
public sealed class RosSubscriber<TMessage> : IRosSubscriber<TMessage> where TMessage : IMessage
{
    static RosCallback<TMessage>[] EmptyCallback => Array.Empty<RosCallback<TMessage>>();

    readonly Dictionary<string, RosCallback<TMessage>> callbacksById = new();
    readonly CancellationTokenSource runningTs = new();
    readonly RosClient client;
    readonly ReceiverManager<TMessage> manager;

    RosCallback<TMessage>[] cachedCallbacks = EmptyCallback; // cache to iterate through callbacks quickly
    int totalSubscribers;
    bool disposed;

    public bool IsPaused
    {
        get => manager.IsPaused;
        set => manager.IsPaused = value;
    }

    public CancellationToken CancellationToken => runningTs.Token;

    /// <summary>
    /// Whether this subscriber is valid.
    /// </summary>
    public bool IsAlive => !runningTs.IsCancellationRequested;

    public string Topic => manager.Topic;
    public string TopicType => manager.TopicType;
    public int NumPublishers => manager.NumConnections;
    public int NumActivePublishers => manager.NumActiveConnections;

    /// <summary>
    /// The number of ids generated by this subscriber.
    /// </summary>
    public int NumIds => callbacksById.Count;

    /// <summary>
    /// Whether the TCP_NODELAY flag was requested.
    /// </summary>
    public bool RequestNoDelay => manager.RequestNoDelay;

    /// <summary>
    /// Event triggered when a new publisher appears.
    /// </summary>
    public event Action<RosSubscriber<TMessage>>? NumPublishersChanged;

    public int TimeoutInMs
    {
        get => manager.TimeoutInMs;
        set => manager.TimeoutInMs = value;
    }

    internal RosSubscriber(RosClient client, TopicInfo topicInfo, bool requestNoDelay, int timeoutInMs,
        RosTransportHint transportHint)
    {
        this.client = client;
        manager = new ReceiverManager<TMessage>(this, client, topicInfo, requestNoDelay, transportHint)
            { TimeoutInMs = timeoutInMs };
    }

    internal void MessageCallback(in TMessage msg, IRosReceiver receiver)
    {
        foreach (var callback in cachedCallbacks)
        {
            try
            {
                callback(in msg, receiver);
            }
            catch (Exception e)
            {
                Logger.LogErrorFormat("{0}: Exception from " + nameof(MessageCallback) + ": {1}", this, e);
            }
        }
    }

    internal void RaiseNumPublishersChanged()
    {
        if (NumPublishersChanged == null)
        {
            return;
        }

        Task.Run(() =>
        {
            try
            {
                NumPublishersChanged?.Invoke(this);
            }
            catch (Exception e)
            {
                Logger.LogErrorFormat("{0}: Exception from " + nameof(NumPublishersChanged) + ": {1}", this, e);
            }
        }, default);
    }

    string GenerateId()
    {
        Interlocked.Increment(ref totalSubscribers);
        int prevNumSubscribers = totalSubscribers - 1;
        return prevNumSubscribers == 0 ? Topic : $"{Topic}-{prevNumSubscribers.ToString()}";
    }

    void AssertIsAlive()
    {
        if (!IsAlive)
        {
            throw new ObjectDisposedException(nameof(RosSubscriber<TMessage>), "This is not a valid subscriber");
        }
    }

    public SubscriberTopicState GetState()
    {
        AssertIsAlive();
        return new SubscriberTopicState(Topic, TopicType, callbacksById.Keys.ToArray(), manager.GetStates());
    }

    ValueTask IRosSubscriber.PublisherUpdateRpcAsync(IEnumerable<Uri> publisherUris, CancellationToken token)
    {
        return PublisherUpdateRcpAsync(publisherUris, token);
    }

    void IDisposable.Dispose()
    {
        Dispose();
    }

    void Dispose()
    {
        if (disposed)
        {
            return;
        }

        disposed = true;
        runningTs.Cancel();
        callbacksById.Clear();
        cachedCallbacks = EmptyCallback;
        NumPublishersChanged = null;
        manager.Stop();
    }

    public async ValueTask DisposeAsync(CancellationToken token)
    {
        if (disposed)
        {
            return;
        }

        disposed = true;
        runningTs.Cancel();
        callbacksById.Clear();
        cachedCallbacks = EmptyCallback;
        NumPublishersChanged = null;
        await manager.StopAsync(token).AwaitNoThrow(this);
    }

    public bool MessageTypeMatches(Type type)
    {
        return type == typeof(TMessage);
    }

    string IRosSubscriber.Subscribe(Action<IMessage> callback) =>
        Subscribe(msg => callback(msg));

    string IRosSubscriber.Subscribe(Action<IMessage, IRosReceiver> callback) =>
        Subscribe((in TMessage msg, IRosReceiver receiver) => callback(msg, receiver));


    /// <summary>
    /// Generates a new subscriber id with the given callback function.
    /// </summary>
    /// <param name="callback">The function to call when a message arrives.</param>
    /// <typeparam name="TMessage">The message type</typeparam>
    /// <returns>The subscribed id.</returns>
    /// <exception cref="ArgumentNullException">The callback is null.</exception>
    public string Subscribe(Action<TMessage> callback) =>
        Subscribe((in TMessage t, IRosReceiver _) => callback(t));

    public string Subscribe(RosCallback<TMessage> callback)
    {
        if (callback is null)
        {
            BuiltIns.ThrowArgumentNull(nameof(callback));
        }

        AssertIsAlive();

        string id = GenerateId();
        callbacksById.Add(id, callback);
        cachedCallbacks = callbacksById.Values.ToArray();
        return id;
    }

    public bool ContainsId(string id)
    {
        if (id is null)
        {
            BuiltIns.ThrowArgumentNull(nameof(id));
        }

        return callbacksById.ContainsKey(id);
    }

    public bool Unsubscribe(string id)
    {
        if (id == null)
        {
            BuiltIns.ThrowArgumentNull(nameof(id));
        }

        if (!IsAlive)
        {
            return true;
        }

        bool removed = callbacksById.Remove(id);
        cachedCallbacks = callbacksById.Values.ToArray();

        if (callbacksById.Count == 0)
        {
            Dispose();
            client.RemoveSubscriber(this);
        }

        return removed;
    }

    public async ValueTask<bool> UnsubscribeAsync(string id, CancellationToken token = default)
    {
        if (id is null)
        {
            BuiltIns.ThrowArgumentNull(nameof(id));
        }

        if (!IsAlive)
        {
            return true;
        }

        bool removed = callbacksById.Remove(id);
        if (callbacksById.Count != 0)
        {
            cachedCallbacks = callbacksById.Values.ToArray();
        }
        else
        {
            cachedCallbacks = EmptyCallback;
            var disposeTask = DisposeAsync(token).AwaitNoThrow(this);
            var unsubscribeTask = client.RemoveSubscriberAsync(this, token).AwaitNoThrow(this);
            await (disposeTask, unsubscribeTask).WhenAll();
        }

        return removed;
    }

    async ValueTask PublisherUpdateRcpAsync(IEnumerable<Uri> publisherUris, CancellationToken token)
    {
        if (runningTs.IsCancellationRequested)
        {
            return;
        }

        token.ThrowIfCancellationRequested();
        using var tokenSource = CancellationTokenSource.CreateLinkedTokenSource(token, runningTs.Token);
        await manager.PublisherUpdateRpcAsync(publisherUris, tokenSource.Token).AwaitNoThrow(this);
    }

    internal void PublisherUpdateRcp(IEnumerable<Uri> publisherUris, CancellationToken token)
    {
        _ = TaskUtils.Run(() => PublisherUpdateRcpAsync(publisherUris, token).AsTask(), token).AwaitNoThrow(this);
    }

    bool IRosSubscriber<TMessage>.TryGetLoopbackReceiver(in Endpoint endPoint, out ILoopbackReceiver<TMessage>? receiver) =>
        manager.TryGetLoopbackReceiver(endPoint, out receiver);

    public override string ToString() => $"[{nameof(RosSubscriber<TMessage>)} {Topic} [{TopicType}] ]";
}