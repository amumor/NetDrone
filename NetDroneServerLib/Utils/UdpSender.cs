using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using DroneServerCore.Models;

namespace NetDroneServerLib.Utils;

public class UdpSender
{
    public async Task SendMessageAsync(Message message, IPEndPoint sender, UdpClient udpClient, CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            var bytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
            await udpClient.SendAsync(bytes, bytes.Length, sender);
        }
    }
}
