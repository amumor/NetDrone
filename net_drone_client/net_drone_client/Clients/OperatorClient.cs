using net_drone_client.Models;

namespace net_drone_client.Clients;

public class OperatorClient : AbstractNetDroneClient
{
    private readonly MovementQueue _movementQueue = new();
    
    public OperatorClient(int clientPort, int serverPort, string serverIp, int droneId)
    {
        ClientPort = clientPort;
        ServerPort = serverPort;
        ServerIp = serverIp;
        DroneState = new DroneState { Id = droneId };
        SetupUdpConnection();
    }
    
    public void SendCommandToDrone(Command command)
    {
        _networkClient.SendCommand(command, DroneState.Id);
    }

    public Vec3<int> GetLatestLocationFromDrone()
    {
        var position = DroneState.Position;
        Console.WriteLine($"Fetching latest location from drone: {position}");
        return position;
    }
    
    public void UpdatePredictedPosition(Vec3<int> predictedPosition)
    {
        DroneState.PredictedPosition = predictedPosition;
        Console.WriteLine($"Predicted position updated to: {predictedPosition}");
    }
    
    protected override void HandleIncomingMessages()
    {
        _networkClient.OnMessageReceived += message =>
        {
            Console.WriteLine($"Received location update from drone {message.DroneId}");
            var data = message.Command.Data;
            DroneState.Position = new Vec3<int>(
                data.X,
                data.Y,
                data.Z
            );
        };
    }
}