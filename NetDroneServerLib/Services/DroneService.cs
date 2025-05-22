using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using DroneServerCore.Models;
using NetDroneServerLib.Models;
using NetDroneServerLib.Utils;

namespace NetDroneServerLib.Services;

public class DroneService
{
    private readonly ConcurrentDictionary<int, IPEndPoint> _droneEndpoints = new(); // TODO: move registry to a singleton
    private readonly UdpSender _udpSender = new UdpSender();

    public async Task HandleRegister(Message message, IPEndPoint sender, UdpClient udpClient, CancellationToken token)
    {
        _droneEndpoints[message.DroneId] = sender;
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

    public async Task HandleCommand(Message message, IPEndPoint sender, UdpClient udpClient, CancellationToken token)
    {
        _droneEndpoints[message.DroneId] = sender;
        await _udpSender.SendMessageAsync(message, sender, udpClient, token);

    }
}