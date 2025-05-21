namespace NetDroneServerLib;

using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using DroneServerCore.Models;

public class NetDroneServer
{
    private ConcurrentDictionary<int, IPEndPoint> droneEndpoints = new();
    private ConcurrentDictionary<int, IPEndPoint> clientEndpoints = new();

    public async Task StartListenersAsync(int droneListenPort, int clientListenPort, CancellationToken token)
    {
        var droneTask = DroneListenerLoopAsync(droneListenPort, token);
        var clientTask = ClientListenerLoopAsync(clientListenPort, token);
        await Task.WhenAll(droneTask, clientTask);
    }

    private async Task DroneListenerLoopAsync(int port, CancellationToken token)
    {
        using var droneListener = new UdpClient(port);

        while (!token.IsCancellationRequested)
        {
            // The two tasks below allows the loop to respond to both incoming UPD packets or a cancellation token
            var receiveTask = droneListener.ReceiveAsync();
            var completedTask = await Task.WhenAny(receiveTask, Task.Delay(-1, token));
            if (completedTask != receiveTask) break;

            var result = receiveTask.Result;
            var json = Encoding.UTF8.GetString(result.Buffer);
            var message = JsonSerializer.Deserialize<Message>(json);

            Console.WriteLine($"[Drone Listener] Received from {result.RemoteEndPoint} : {json}");

            if (message != null)
            {
                droneEndpoints[message.DroneId] = result.RemoteEndPoint;
                if (!message.Command.Cmd.Equals(CommandType.Register))
                {
                    await ForwardToClientAsync(message, token);
                }
            }
        }
    }

    private async Task ClientListenerLoopAsync(int port, CancellationToken token)
    {
        using var clientListener = new UdpClient(port);

        while (!token.IsCancellationRequested)
        {
            // The two tasks below allows the loop to respond to both incoming UPD packets or a cancellation token
            var receiveTask = clientListener.ReceiveAsync();
            var completedTask = await Task.WhenAny(receiveTask, Task.Delay(-1, token));
            if (completedTask != receiveTask) break;

            var result = receiveTask.Result;
            var json = Encoding.UTF8.GetString(result.Buffer);
            var message = JsonSerializer.Deserialize<Message>(json);

            Console.WriteLine($"[Client Listener] Received from {result.RemoteEndPoint} : {json}");

            if (message != null)
            {
                clientEndpoints[message.DroneId] = result.RemoteEndPoint;
                if (!message.Command.Cmd.Equals(CommandType.Register))
                {
                    await ForwardToDroneAsync(message, token);
                }
            }
        }
    }

    private async Task ForwardToDroneAsync(Message message, CancellationToken token)
    {
        if (droneEndpoints.TryGetValue(message.DroneId, out var droneEP))
        {
            using var sender = new UdpClient();
            var json = JsonSerializer.Serialize(message);
            var bytes = Encoding.UTF8.GetBytes(json);
            await sender.SendAsync(bytes, bytes.Length, droneEP);
            Console.WriteLine($"Forwarded to drone {message.DroneId} at {droneEP}");
        }
        else
        {
            Console.WriteLine($"No endpoint for drone {message.DroneId}");
        }
    }

    private async Task ForwardToClientAsync(Message message, CancellationToken token)
    {
        if (clientEndpoints.TryGetValue(message.DroneId, out var clientEP))
        {
            using var sender = new UdpClient();
            var json = JsonSerializer.Serialize(message);
            var bytes = Encoding.UTF8.GetBytes(json);
            await sender.SendAsync(bytes, bytes.Length, clientEP);
            Console.WriteLine($"Forwarded to client for drone {message.DroneId} at {clientEP}");
        }
        else
        {
            Console.WriteLine($"No endpoint for client of drone {message.DroneId}");
        }
    }
}