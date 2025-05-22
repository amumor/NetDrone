using NetDroneClientLib.Models;
using NetDroneServerLib.Models;

namespace NetDroneClientLib.Clients;

public class DroneClient : AbstractNetDroneClient
{
    public readonly MovementQueue<LocationMessage> MovementQueue = new DroneQueue();
    private int _messageId = 1;

    public DroneClient(int clientPort, int serverPort, string serverIp, int droneId, int operatorId)
    {
        ClientPort = clientPort;
        ServerPort = serverPort;
        ServerIp = serverIp;
        DroneState = new DroneState { Id = droneId, OperatorId = operatorId, Position = new Vec3()};
        SetupUdpConnection();
    }

    private void SendLocationToOperator(int messageId, Vec3 currentLocation)
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

    public LocationMessage? GetNextMovement()
    {
        var locationMessage = MovementQueue.GetNextMovement();
        if (locationMessage == null)
        {
            return null;
        }
        
        if (locationMessage.MessageId != 0)
        {
            SendLocationToOperator(locationMessage.MessageId, locationMessage.Position);
            _messageId = locationMessage.MessageId;
        }
        return locationMessage;
    }
    
    public void SetMovementInterpolation(bool shouldInterpolate)
    {
        MovementQueue.ShouldInterpolate = shouldInterpolate;
    }

    protected override void HandleIncomingMessages()
    {
        _networkClient.OnMessageReceived += message =>
        {
            Console.WriteLine($"Received command from operator {message.OperatorId}");
            MovementQueue.AddMovement(
                new LocationMessage
                {
                    MessageId = message.MessageId,
                    Position = message.Command.Data
                }
            );
        };
    }
}