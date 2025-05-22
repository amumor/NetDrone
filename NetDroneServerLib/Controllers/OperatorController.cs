using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using DroneServerCore.Models;
using NetDroneServerLib.Models;
using NetDroneServerLib.Services;
using NetDroneServerLib.Utils;

namespace NetDroneServerLib.Controllers;

public class OperatorController : IMessageController
{
    private readonly OperatorService _operatorService;
    public OperatorController(OperatorService operatorService)
    {
        _operatorService = operatorService;
    }

    public async Task HandleMessageAsync(string json, IPEndPoint sender, UdpClient udpClient, CancellationToken token)
    {
        var message = JsonSerializer.Deserialize<Message>(json);
        if (message == null) return; // TODO: bedre feilhåndtering

        switch (message.Command.Cmd)
        {
            case CommandType.Register:
                System.Console.WriteLine("Register command from operator received");
                await _operatorService.HandleRegister(message, sender, udpClient, token);
                break;
            case CommandType.Move:
                System.Console.WriteLine("Move command from operator received");
                await _operatorService.HandleCommand(message, sender, udpClient, token);
                break;
            default:
                System.Console.WriteLine("!!! Unknown command");
                break;
        }
    }
}