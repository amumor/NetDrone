using net_drone_client.Models;

namespace net_drone_client.Clients;

public class DroneClient : AbstractNetDroneClient
{
    private readonly MovementQueue _movementQueue = new();

    public DroneClient(int clientPort, int serverPort, string serverIp, int droneId)
    {
        ClientPort = clientPort;
        ServerPort = serverPort;
        ServerIp = serverIp;
        DroneState = new DroneState { Id = droneId };
        SetupUdpConnection();
    }

    public void SendLocationToOperator(Command command)
    {
        Console.WriteLine($"[{ServerIp}:{ServerPort}] {command}");
        _networkClient.SendCommand(command, DroneState.Id);
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