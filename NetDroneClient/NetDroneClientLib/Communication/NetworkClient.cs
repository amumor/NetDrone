using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using NetDroneClientLib.Models;
using NetDroneServerLib.Models;

namespace NetDroneClientLib.Communication;

public class NetworkClient
{
    private readonly UdpClient _udpClient = new();
    private readonly IPEndPoint _serverEndpoint;
    private bool _isRunning;
    public event Action<Message> OnMessageReceived = delegate { };

    public NetworkClient(int clientPort, int serverPort, string serverIp)
    {
        // Exposes the UDP client to the outside world
        _udpClient.Client.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), clientPort));
        Console.WriteLine($"UDP client is listening on port {clientPort}");
        
        // Connects to the server
        _serverEndpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), serverPort);
        Console.WriteLine($"UDP client connected to server at {serverIp}:{serverPort}\n");
        
        _isRunning = true;
        Task.Run(ListenForMessages);
    }
    
    public void SendCommand(int messageId, Command command, int droneId, int operatorId)
    {
        var outgoingMessage = new Message
        {
            MessageId = messageId,
            DroneId = droneId,
            OperatorId = operatorId,
            Command = command
        };

        var messageBytes = JsonSerializer.SerializeToUtf8Bytes(outgoingMessage);
        _udpClient.Send(messageBytes, messageBytes.Length, _serverEndpoint);
        /*
        var message = command.Cmd == CommandType.Register
            ? $"{Client.CLIENT_TYPE} with id {droneId} registered with server."
            : $"Command [{command.Cmd}] was sent to droneId {droneId} from {Client.CLIENT_TYPE}";
        Console.WriteLine(message);
        */
    }

    private async Task ListenForMessages()
    {
        while (_isRunning)
        {
            try
            {
                var receivedBytes = await _udpClient.ReceiveAsync();
                var incomingMessage = JsonSerializer.Deserialize<Message>(receivedBytes.Buffer);

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