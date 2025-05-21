using System;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using DroneServerCore.Models;
using Microsoft.Extensions.Logging;
using NetDroneServerLib.Models;
using NetDroneServerLib.Services;
using NetDroneServerLib.Utils;

namespace NetDroneServerLib.Controllers;

public class DroneController : IMessageController
{
    private readonly DroneService _droneService;
    private readonly UdpSender _udpSender = new UdpSender();
    public DroneController(DroneService droneService)
    {
        _droneService = droneService;
    }

    public async Task HandleMessageAsync(string json, IPEndPoint sender, UdpClient udpClient, CancellationToken token)
    {
        var message = JsonSerializer.Deserialize<Message>(json);
        if (message == null) return; // TODO: bedre feil håndtering

        switch (message.Command.Cmd)
        {
            case CommandType.Register:
                System.Console.WriteLine("Register command from drone received");
                await _droneService.HandleRegister(message, sender, udpClient, token);
                break;
            case CommandType.Move:
                System.Console.WriteLine("Move command from drone received");
                await _udpSender.SendMessageAsync(message, sender, udpClient, token);
                break;
            default:
                System.Console.WriteLine("!!! Unknown command");
                break;
        }
    }
}
