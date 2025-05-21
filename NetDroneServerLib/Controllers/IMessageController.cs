using System;
using System.Net;
using System.Net.Sockets;

namespace NetDroneServerLib.Controllers;

public interface IMessageController
{
    Task HandleMessageAsync(string json, IPEndPoint sender, UdpClient udpClient, CancellationToken token);
}
