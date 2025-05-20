using net_drone_client.Communication;
using net_drone_client.Models;

namespace net_drone_client.Clients;

public class DroneClient : AbstractNetDroneClient
{

    
    private readonly MovementQueue _movementQueue = new();

    public DroneClient(string ip, int port)
    {
        Ip = ip;
        Port = port;
        SetupUdpConnection();
    }

    public void SendLocationToOperator(Command command)
    {
        _networkClient.SendLocation(command);
    }
    
    public List<Vec3<float>> GetPendingMovements()
    {
        // Needs to be interpolated
        return _movementQueue.GetPendingMovements();
    }
    
    protected override void HandleIncomingMessages()
    {
        _networkClient.OnMessageReceived += message =>
        {
            _movementQueue.AddMovement(
                new Vec3<float>(message.X, message.Y, message.Z)
            );
            Console.WriteLine($"Handling message for DroneClient: {message.Type}");
        };
    }
}