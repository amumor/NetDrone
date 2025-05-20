using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using net_drone_client.Models;

namespace net_drone_client.Communication;

public class NetworkClient
{
    private readonly UdpClient _udpClient = new();
    private bool _isRunning;
    public event Action<ServerMessage> OnMessageReceived = delegate { };

    public NetworkClient(int clientPort, int serverPort, string serverIp)
    {
        _udpClient.Client.Bind(new IPEndPoint(IPAddress.Any, clientPort));
        _udpClient.Connect(
            new IPEndPoint(IPAddress.Parse(serverIp), serverPort)
        );
        _isRunning = true;
        Task.Run(ListenForMessages);
        
        Console.WriteLine($"UDP client initialized on {clientPort} and connected to server at {serverIp}:{serverPort}");
    }
    
    public void SendCommand(Command command, int droneId)
    {
        var outgoingMessage = new ServerMessage
        {
            //TODO: Server must assign the drone id as it is not known to the client
            DroneId = droneId,
            Command = command
        };

        var messageBytes = JsonSerializer.SerializeToUtf8Bytes(outgoingMessage);
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
                var incomingMessage = JsonSerializer.Deserialize<ServerMessage>(receivedBytes.Buffer);

                if (incomingMessage != null)
                {
                    OnMessageReceived?.Invoke(incomingMessage);
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