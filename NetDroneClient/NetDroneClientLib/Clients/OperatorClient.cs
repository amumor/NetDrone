

using NetDroneClientLib.Models;
using NetDroneServerLib.Models;

namespace NetDroneClientLib.Clients;

public class OperatorClient : AbstractNetDroneClient
{
    private readonly MovementQueue _movementQueue = new();
    private int messageId = 0;

    public OperatorClient(int clientPort, int serverPort, string serverIp, int droneId, int operatorId)
    {
        ClientPort = clientPort;
        ServerPort = serverPort;
        ServerIp = serverIp;
        DroneState = new DroneState { Id = droneId, OperatorId = operatorId};
        SetupUdpConnection();
    }

    public void SendCommandToDrone(Command command)
    {
        Console.WriteLine($"Sending command to drone {DroneState.Id}: {command}");
        _networkClient.SendCommand(messageId, command, DroneState.Id, DroneState.OperatorId);
        messageId++;
    }

    public Vec3 GetLatestLocationFromDrone()
    {
        var position = DroneState.Position;
        Console.WriteLine($"Fetching latest location from drone: {position}");
        return position;
    }

    public void UpdatePredictedPosition(Vec3 predictedPosition)
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
            DroneState.Position = new Vec3
            {
                X = data.X,
                Y = data.Y,
                Z = data.Z
            };
        };
    }
}