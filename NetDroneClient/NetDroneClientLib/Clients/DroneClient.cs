using NetDroneClientLib.Models;

namespace NetDroneClientLib.Clients;

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

    public void SendLocationToOperator(Vec3<int> currentLocation)
    {
        var command = new Command(
            CommandType.State,
            currentLocation
        );
        _networkClient.SendCommand(command, DroneState.Id);
    }

    public List<Vec3<int>> GetPendingMovements()
    {
        // Needs to be interpolated
        var pendingMovements = _movementQueue.GetPendingMovements();
        Console.WriteLine($"{pendingMovements.Count} pending movements found for drone {DroneState.Id}");
        return pendingMovements;
    }

    protected override void HandleIncomingMessages()
    {
        _networkClient.OnMessageReceived += message =>
        {
            Console.WriteLine($"Message received: {message}");
            var data = message.Command.Data;
            _movementQueue.AddMovement(
                new Vec3<int>(
                    data.X,
                    data.Y,
                    data.Z
                )
            );
        };
    }
}