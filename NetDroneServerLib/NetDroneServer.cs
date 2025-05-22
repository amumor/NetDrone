namespace NetDroneServerLib;

using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using NetDroneServerLib.Models;
using NetDroneServerLib.Repository;
using NetDroneServerLib.Utils;

public class NetDroneServer
{
    private readonly ConcurrentDictionary<int, IPEndPoint> _droneEndpoints = new();
    private readonly ConcurrentDictionary<int, IPEndPoint> _operatorEndpoints = new();

    public async Task StartListenersAsync(int droneListenPort, int operatorListenPort, CancellationToken token)
    {
        var droneTask = DroneListenerLoopAsync(droneListenPort, token);
        System.Console.WriteLine($"...Listening for Drone messages on Port: {droneListenPort}");

        var operatorTask = OperatorListenerLoopAsync(operatorListenPort, token);
        System.Console.WriteLine($"...Listening for Operator messages on Port: {operatorListenPort}");

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
                switch (message.Command.Cmd)
                {
                    case CommandType.Register:
                        System.Console.WriteLine("Register command from drone received");
                        IpEndPointRepository.RegisterDrone(message.DroneId, result.RemoteEndPoint);
                        break;

                    default:
                        System.Console.WriteLine("Forwarding message to Operator");
                        IpEndPointRepository.RegisterDrone(message.DroneId, result.RemoteEndPoint);
                        using (var udpClient = new UdpClient())
                        {
                            if (IpEndPointRepository.TryGetOperator(message.OperatorId, out var endPoint) && endPoint is not null)
                            {
                                await UdpSender.ForwardToOperatorAsync(message, endPoint, udpClient, token);
                            }
                        }
                        break;
                }
            }
        }
    }

    private async Task OperatorListenerLoopAsync(int port, CancellationToken token)
    {
        using var operatorListener = new UdpClient(port);

        while (!token.IsCancellationRequested)
        {
            // The two tasks below allows the loop to respond to both incoming UDP packets or a cancellation token
            var receiveTask = operatorListener.ReceiveAsync();
            var completedTask = await Task.WhenAny(receiveTask, Task.Delay(-1, token));
            if (completedTask != receiveTask) break;

            var result = receiveTask.Result;
            var json = Encoding.UTF8.GetString(result.Buffer);
            var message = JsonSerializer.Deserialize<Message>(json);

            Console.WriteLine($"[Operator Listener] Received from {result.RemoteEndPoint} : {json}");

            if (message != null)
            {
                switch (message.Command.Cmd)
                {
                    case CommandType.Register:
                        System.Console.WriteLine("Register command from operator received");
                        IpEndPointRepository.RegisterOperator(message.OperatorId, result.RemoteEndPoint);
                        break;

                    default:
                        System.Console.WriteLine("Forwarding message to Drone");
                        IpEndPointRepository.RegisterOperator(message.OperatorId, result.RemoteEndPoint);
                        using (var udpClient = new UdpClient())
                        {
                            if (IpEndPointRepository.TryGetDrone(message.DroneId, out var endPoint) && endPoint is not null)
                            {
                                await UdpSender.ForwardToDroneAsync(message, endPoint, udpClient, token);
                            }
                        }
                        break;
                }
            }
        }
    }
}