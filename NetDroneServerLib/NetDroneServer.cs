﻿namespace NetDroneServerLib;

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

    /// <summary>
    /// Starts the NetDrone server.
    /// </summary>
    /// <param name="droneListenPort"></param>
    /// <param name="operatorListenPort"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public async Task StartListenersAsync(int droneListenPort, int operatorListenPort, CancellationToken token)
    {
        var droneTask = DroneListenerLoopAsync(droneListenPort, token);
        System.Console.WriteLine($"...Listening for Drone messages on Port: {droneListenPort}");

        var operatorTask = OperatorListenerLoopAsync(operatorListenPort, token);
        System.Console.WriteLine($"...Listening for Operator messages on Port: {operatorListenPort}");

        await Task.WhenAll(droneTask, operatorTask);
    }

    /// <summary>
    /// Listener loop for incoming messages from drones.
    /// </summary>
    /// <param name="port"></param>
    /// <param name="token"></param>
    /// <returns></returns>
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
                                await UdpSender.ForwardToOperatorAsync(message, endPoint, udpClient);
                            }
                        }
                        break;
                }
            }
        }
    }

    /// <summary>
    /// Listener loop for incoming messages from operators.
    /// </summary>
    /// <param name="port"></param>
    /// <param name="token"></param>
    /// <returns></returns>
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
                                await UdpSender.ForwardToDroneAsync(message, endPoint, udpClient);
                            }
                        }
                        break;
                }
            }
        }
    }
}