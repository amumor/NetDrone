using net_drone_client.Util;
using NetDroneClientLib.Models;
using NetDroneServerLib.Models;

namespace NetDroneClientLib.Clients;

public class DroneClient : AbstractNetDroneClient
{
    public readonly MovementQueue _movementQueue = new();
    public int MessageId = 1;

    public DroneClient(int clientPort, int serverPort, string serverIp, int droneId, int operatorId)
    {
        ClientPort = clientPort;
        ServerPort = serverPort;
        ServerIp = serverIp;
        DroneState = new DroneState { Id = droneId, OperatorId = operatorId };
        SetupUdpConnection();
    }

    public void SendLocationToOperator(int messageId, Vec3 currentLocation)
    {
        var command = new Command
        {
            Cmd = CommandType.State,
            Data = new Vec3
            {
                X = currentLocation.X,
                Y = currentLocation.Y,
                Z = currentLocation.Z
            }
        };
        _networkClient.SendCommand(messageId, command, DroneState.Id, DroneState.OperatorId);
    }

    public LocationMessage GetNextMovement()
    {
        var locationMessage = _movementQueue.GetNextMovement();
        if (locationMessage.MessageId != MessageId)
        {
            SendLocationToOperator(locationMessage.MessageId, locationMessage.Position);
            MessageId = locationMessage.MessageId;
        }
        return locationMessage;
    }
    
    public void SetMovementInterpolation(bool shouldInterpolate)
    {
        _movementQueue.ShouldInterpolate = shouldInterpolate;
    }

    protected override void HandleIncomingMessages()
    {
        _networkClient.OnMessageReceived += message =>
        {
            Console.WriteLine($"Message received: {message}");
            var data = message.Command.Data;
            _movementQueue.AddMovement(
                new LocationMessage()
                {
                    MessageId = message.MessageId,
                    Position = message.Command.Data
                }
            );
        };
    }
}