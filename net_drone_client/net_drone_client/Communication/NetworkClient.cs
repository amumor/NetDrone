using System.Net.Sockets;
using System.Text.Json;
using net_drone_client.Models;

namespace net_drone_client.Communication;

public class NetworkClient
{
    private readonly UdpClient _udpClient = new();
    private bool _isRunning;
    public event Action<ServerMessage> OnMessageReceived = delegate { };

    public NetworkClient(string ip, int port)
    {
        _udpClient.Connect(ip, port);
        _isRunning = true;
        Task.Run(ListenForMessages);
        Console.WriteLine($"UDP client initialized and connected to {ip}:{port}");
    }

    public void SendLocation(Command location)
    {
        var clientMessage = new ClientMessage {
            Type = "state",
            DroneId = "drone_1",
            Command = new Command(
                CommandType.Move,
                new Vec3<int>(location.Data.X, location.Data.Y, location.Data.Z)
            ) 
        };

        var messageBytes = JsonSerializer.SerializeToUtf8Bytes(clientMessage);
        _udpClient.Send(messageBytes, messageBytes.Length);
        Console.WriteLine("Location data sent to server.");
    }

    public void SendCommand(Command command)
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

    private async Task ListenForMessages()
    {
        while (_isRunning)
        {
            try
            {
                var receivedBytes = await _udpClient.ReceiveAsync();
                var serverMessage = JsonSerializer.Deserialize<ServerMessage>(receivedBytes.Buffer);

                if (serverMessage != null)
                {
                    OnMessageReceived?.Invoke(serverMessage);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error receiving message: {ex.Message}");
            }
        }
    }

    public void Close()
    {
        _udpClient.Close();
        _isRunning = false;
        Console.WriteLine("UDP client closed.");
    }
}