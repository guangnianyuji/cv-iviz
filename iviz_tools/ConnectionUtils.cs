using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Iviz.Tools;

public static class ConnectionUtils
{
    /// <summary>
    /// A list of host aliases in the form of (key: ip address, value: real ip or name).
    /// Used by all iviz connections. 
    /// </summary>
    public static Dictionary<string, string> GlobalResolver { get; } = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Convenience function to connect a <see cref="TcpClient"/> to the given address,
    /// while taking into account timeouts and cancellation tokens.
    /// These functions are standard in NET5, but are missing in Standard 2.1.
    /// </summary>
    /// <param name="client">The client to connect.</param>
    /// <param name="hostname">The destination address. Takes into account <see cref="GlobalResolver"/>.</param>
    /// <param name="port">The destination port.</param>
    /// <param name="token">A cancellation token.</param>
    /// <param name="timeoutInMs">The timeout in milliseconds.</param>
    public static async ValueTask TryConnectAsync(this TcpClient client, string hostname, int port,
        CancellationToken token, int timeoutInMs = -1)
    {
        token.ThrowIfCancellationRequested();

        string resolvedHostname = GlobalResolver.TryGetValue(hostname, out string? newHostname)
            ? newHostname
            : hostname;

        var tcs = TaskUtils.CreateCompletionSource<IAsyncResult>();
        var socket = client.Client;

        socket.BeginConnect(resolvedHostname, port, CallbackHelpers.OnComplete, tcs);

        if (tcs.Task.IsCompleted)
        {
            socket.EndConnect(await tcs.Task);
            return;
        }

        if (timeoutInMs == -1)
        {
            // ReSharper disable once UseAwaitUsing
            using (token.Register(CallbackHelpers.OnCanceled, tcs))
            {
                socket.EndConnect(await tcs.Task);
                return;
            }
        }

        using var timeoutTs = new CancellationTokenSource(timeoutInMs);

        // ReSharper disable once UseAwaitUsing
        using (token.Register(CallbackHelpers.OnCanceled, tcs)) 
        // ReSharper disable once UseAwaitUsing
        using (timeoutTs.Token.Register(CallbackHelpers.OnTimeout, tcs))
        {
            socket.EndConnect(await tcs.Task);
        }
    }

    /// <summary>
    /// Convenience function to connect a <see cref="UdpClient"/> to the given address,
    /// </summary>
    /// <param name="client">The client to connect.</param>
    /// <param name="hostname">The destination address. Takes into account <see cref="GlobalResolver"/>.</param>
    /// <param name="port">The destination port.</param>
    public static void TryConnect(this UdpClient client, string hostname, int port)
    {
        string resolvedHostname = GlobalResolver.TryGetValue(hostname, out string? newHostname)
            ? newHostname
            : hostname;

        if (IPAddress.TryParse(resolvedHostname, out var address))
        {
            client.Connect(new IPEndPoint(address, port));
            return;
        }

        client.Connect(resolvedHostname, port);
    }

    /// <summary>
    /// Checks if socket is connected and can transmit data.
    /// Taken from https://stackoverflow.com/questions/2661764/how-to-check-if-a-socket-is-connected-disconnected-in-c".
    /// </summary>
    public static bool CheckIfAlive(this Socket? socket) =>
        socket is { Connected: true } && !(socket.Poll(1, SelectMode.SelectRead) && socket.Available == 0);

    public static int GetProcessId()
    {
#if NET5_0_OR_GREATER
        return Environment.ProcessId;
#else
        return Process.GetCurrentProcess().Id;
#endif
    }
}