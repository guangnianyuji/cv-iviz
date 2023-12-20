﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using Iviz.Roslib.Utils;
using Iviz.Tools;

namespace Iviz.Roslib;

[DataContract]
public abstract class SubscriberReceiverState : JsonToString
{
    [DataMember] public Uri RemoteUri { get; }
    [DataMember] public abstract TransportType? TransportType { get; }
    [DataMember] public string? RemoteId { get; internal set; }
    [DataMember] public ReceiverStatus Status { get; internal set; }
    [DataMember] public Endpoint EndPoint { get; internal set; }
    [DataMember] public Endpoint RemoteEndpoint { get; internal set; }
    [DataMember] public long NumReceived { get; internal set; }
    [DataMember] public long NumDropped { get; internal set; }
    [DataMember] public long BytesReceived { get; internal set; }
    [DataMember] public ErrorMessage? ErrorDescription { get; internal set; }
    [DataMember] public bool IsAlive { get; internal set; }

    protected SubscriberReceiverState(Uri remoteUri)
    {
        RemoteUri = remoteUri;
    }
}

[DataContract]
public sealed class TcpReceiverState : SubscriberReceiverState
{
    [DataMember] public override TransportType? TransportType => Roslib.TransportType.Tcp;
    [DataMember] public bool RequestNoDelay { get; internal set; }

    internal TcpReceiverState(Uri remoteUri) : base(remoteUri)
    {
    }
}

[DataContract]
public sealed class UdpReceiverState : SubscriberReceiverState
{
    [DataMember] public override TransportType? TransportType => Roslib.TransportType.Udp;
    [DataMember] public int MaxPacketSize { get; internal set; }

    internal UdpReceiverState(Uri remoteUri) : base(remoteUri)
    {
    }
}

[DataContract]
public sealed class UninitializedReceiverState : SubscriberReceiverState
{
    [DataMember] public override TransportType? TransportType => null;

    internal UninitializedReceiverState(Uri remoteUri) : base(remoteUri)
    {
    }
}

[DataContract]
public sealed class SubscriberTopicState : JsonToString
{
    [DataMember] public string Topic { get; }
    [DataMember] public string Type { get; }
    [DataMember] public string[] SubscriberIds { get; }
    [DataMember] public SubscriberReceiverState[] Receivers { get; }

    internal SubscriberTopicState(string topic, string type, string[] topicIds, SubscriberReceiverState[] receivers)
    {
        Topic = topic;
        Type = type;
        SubscriberIds = topicIds;
        Receivers = receivers;
    }

    public void Deconstruct(out string topic, out string type, out string[] subscriberIds,
        out SubscriberReceiverState[] receivers)
        => (topic, type, subscriberIds, receivers) = (Topic, Type, SubscriberIds, Receivers);
}

[DataContract]
public sealed class SubscriberState
{
    [DataMember] public SubscriberTopicState[] Topics { get; }

    internal SubscriberState(SubscriberTopicState[] topics)
    {
        Topics = topics;
    }
}