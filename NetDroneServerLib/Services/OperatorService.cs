using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using DroneServerCore.Models;
using NetDroneServerLib.Communication;
using NetDroneServerLib.Models;
using NetDroneServerLib.Utils;

namespace NetDroneServerLib.Services;

public class OperatorService
{
    private readonly ConcurrentDictionary<int, IPEndPoint> _operatorEndpoints = new(); // TODO: move registry to a singleton
    private UdpSender _udpSender = new UdpSender();

    public async Task HandleRegister(Message message, IPEndPoint sender, UdpClient udpClient, CancellationToken token)
    {
        _operatorEndpoints[message.DroneId] = sender;
        var ack = new Message
        {
            DroneId = message.DroneId,
            Command = new Command
            {
                Cmd = CommandType.Register,
                Data = null
            },
        };
        await _udpSender.SendMessageAsync(ack, sender, udpClient, token);
    }
}
