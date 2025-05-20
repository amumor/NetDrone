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
        Console.WriteLine($"[{Ip}:{Port}] {command}");
        _networkClient.SendCommand(command);
    }
    
    public List<Vec3<float>> GetPendingMovements()
    {
        Console.WriteLine("Getting pending movements...");
        // Needs to be interpolated
        return _movementQueue.GetPendingMovements();
    }
    
    protected override void HandleIncomingMessages()
    {
        _networkClient.OnMessageReceived += message =>
        {
            Console.WriteLine($"Handling message for DroneClient: {message.Command.Cmd}");
            var data = message.Command.Data;
            _movementQueue.AddMovement(
                new Vec3<float>(
                    data.X, 
                    data.Y, 
                    data.Z
                )
            );
        };
    }
}