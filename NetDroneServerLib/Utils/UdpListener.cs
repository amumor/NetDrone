using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using DroneServerCore.Models;
using NetDroneServerLib.Controllers;

namespace NetDroneServerLib.Communication;

public class UdpListener
{
    private readonly DroneController _droneController;
    private readonly OperatorController _operatorController;

    public UdpListener(DroneController droneController, OperatorController operatorController)
    {
        _droneController = droneController;
        _operatorController = operatorController;
    }

    public async Task ListenAsync(int port, CancellationToken token, bool isDrone)
    {
        using var udpClient = new UdpClient(port);
        while (!token.IsCancellationRequested)
        {
            var result = await udpClient.ReceiveAsync();
            var json = Encoding.UTF8.GetString(result.Buffer);

            if (isDrone)
                await _droneController.HandleMessageAsync(json, result.RemoteEndPoint, udpClient, token);
            else
                await _operatorController.HandleMessageAsync(json, result.RemoteEndPoint, udpClient, token);
        }
    }
}
