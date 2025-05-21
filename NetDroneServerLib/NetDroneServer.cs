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
    private readonly ConcurrentDictionary<int, IPEndPoint> _droneEndpoints = new();
    private readonly ConcurrentDictionary<int, IPEndPoint> _operatorEndpoints = new();

    public async Task StartListenersAsync(int droneListenPort, int operatorListenPort, CancellationToken token)
    {
        var droneTask = DroneListenerLoopAsync(droneListenPort, token);
        System.Console.WriteLine($"...Listening for Drone messages on Port: {_droneEndpoints}");

        var operatorTask = OperatorListenerLoopAsync(operatorListenPort, token);
        System.Console.WriteLine($"...Listening for Operator messages on Port: {_operatorEndpoints}");

        await Task.WhenAll(droneTask, operatorTask);
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
                _droneEndpoints[message.DroneId] = result.RemoteEndPoint;
                if (!message.Command.Cmd.Equals(CommandType.Register))
                {
                    await ForwardToOperatorAsync(message, token);
                }
            }
        }
    }

    private async Task OperatorListenerLoopAsync(int port, CancellationToken token)
    {
        using var operatorListener = new UdpClient(port);

        while (!token.IsCancellationRequested)
        {
            // The two tasks below allows the loop to respond to both incoming UPD packets or a cancellation token
            var receiveTask = operatorListener.ReceiveAsync();
            var completedTask = await Task.WhenAny(receiveTask, Task.Delay(-1, token));
            if (completedTask != receiveTask) break;

            var result = receiveTask.Result;
            var json = Encoding.UTF8.GetString(result.Buffer);
            var message = JsonSerializer.Deserialize<Message>(json);

            Console.WriteLine($"[Operator Listener] Received from {result.RemoteEndPoint} : {json}");

            if (message != null)
            {
                _operatorEndpoints[message.DroneId] = result.RemoteEndPoint;
                if (!message.Command.Cmd.Equals(CommandType.Register))
                {
                    await ForwardToDroneAsync(message, token);
                }
            }
        }
    }

    private async Task ForwardToDroneAsync(Message message, CancellationToken token)
    {
        Console.WriteLine($"Drone id: {message.DroneId}");
        if (_droneEndpoints.TryGetValue(message.DroneId, out var droneEP))
        {
            Console.WriteLine($"Drone EP: {droneEP}");
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

    private async Task ForwardToOperatorAsync(Message message, CancellationToken token)
    {
        Console.WriteLine($"Drone id: {message.DroneId}");
        if (_operatorEndpoints.TryGetValue(message.DroneId, out var operatorEP))
        {
            Console.WriteLine($"Drone EP: {operatorEP}");
            using var sender = new UdpClient();
            var json = JsonSerializer.Serialize(message);
            var bytes = Encoding.UTF8.GetBytes(json);
            await sender.SendAsync(bytes, bytes.Length, operatorEP);
            Console.WriteLine($"Forwarded to operator for drone {message.DroneId} at {operatorEP}");
        }
        else
        {
            Console.WriteLine($"No endpoint for operator of drone {message.DroneId}");
        }
    }
}