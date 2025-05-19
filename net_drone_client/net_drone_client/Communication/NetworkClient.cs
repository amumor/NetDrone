using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using net_drone_client.Models;

namespace net_drone_client.Communication;

public static class NetworkClient
{
    private static UdpClient _udpClient;

    public static void Initialize(string ip, int port)
    {
        _udpClient = new UdpClient();
        _udpClient.Connect(ip, port);
        Console.WriteLine($"UDP client initialized and connected to {ip}:{port}");
    }

    public static void SendLocation(Command location)
    {
        var serverMessage = new ServerMessage
        {
            Type = "state",
            DroneId = "drone_1",
            X = location.Data.X,
            Y = location.Data.Y,
            Z = location.Data.Z
        };

        var messageBytes = JsonSerializer.SerializeToUtf8Bytes(serverMessage);
        _udpClient.Send(messageBytes, messageBytes.Length);
        Console.WriteLine("Location data sent to server.");
    }

    public static void SendCommand(Command command)
    {
        var clientMessage = new ClientMessage
        {
            Type = "command",
            DroneId = "drone_1",
            Command = command
        };

        var messageBytes = JsonSerializer.SerializeToUtf8Bytes(clientMessage);
        _udpClient.Send(messageBytes, messageBytes.Length);
        Console.WriteLine("Command sent to server.");
    }

    public static ServerMessage ReceiveMessage(int port)
    {
        var remoteEndPoint = new IPEndPoint(IPAddress.Any, port);
        var receivedBytes = _udpClient.Receive(ref remoteEndPoint);
        var serverMessage = JsonSerializer.Deserialize<ServerMessage>(receivedBytes);

        return serverMessage ?? new ServerMessage
        {
            Type = "unknown",
            DroneId = "unknown",
            X = 0,
            Y = 0,
            Z = 0
        };
    }

    public static void Close()
    {
        _udpClient.Close();
        Console.WriteLine("UDP client closed.");
    }
}