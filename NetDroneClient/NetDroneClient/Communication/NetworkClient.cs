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
        // Exposes the UDP client to the outside world
        _udpClient.Client.Bind(new IPEndPoint(IPAddress.Any, clientPort));
        Console.WriteLine($"UDP client is listening on port {clientPort}");
        
        // Connects to the server
        _udpClient.Connect(
            new IPEndPoint(IPAddress.Parse(serverIp), serverPort)
        );
        Console.WriteLine($"UDP client connected to server at {serverIp}:{serverPort}\n");
        
        _isRunning = true;
        Task.Run(ListenForMessages);
    }
    
    public void SendCommand(Command command, int droneId)
    {
        var outgoingMessage = new ServerMessage
        {
            DroneId = droneId,
            Command = command
        };

        var messageBytes = JsonSerializer.SerializeToUtf8Bytes(outgoingMessage);
        _udpClient.Send(messageBytes, messageBytes.Length);
        var message = command.Cmd == CommandType.Register
            ? $"{Program.CLIENT_TYPE} with id {droneId} registered with server."
            : $"Command [{command.Cmd}] was sent to droneId {droneId} from {Program.CLIENT_TYPE}";
        Console.WriteLine(message);
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
        Console.WriteLine("\nUDP connection closed.");
    }
}