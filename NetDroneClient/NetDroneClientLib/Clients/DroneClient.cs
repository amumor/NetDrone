using NetDroneClientLib.Models;
using NetDroneServerLib.Models;

namespace NetDroneClientLib.Clients;

public class DroneClient : AbstractNetDroneClient
{
    public readonly MovementQueue<MovementQueueItem> MovementQueue;
    private int _currentMessageId;

    public DroneClient (
        int clientPort,
        int serverPort, 
        string serverIp,
        int droneId,
        int operatorId,
        int interpolationRate
    ) 
    {
        ClientPort = clientPort;
        ServerPort = serverPort;
        ServerIp = serverIp;
        MovementQueue = new DroneQueue { StepCount = interpolationRate };
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

    public MovementQueueItem? GetNextMovement()
    {
        var locationMessage = MovementQueue.GetNextMovement();
        if (locationMessage == null)
        {
            return null;
        }
        // This means that the drone has finished the whole movement
        if (!locationMessage.IsInterpolated)
        {
            SendLocationToOperator(_currentMessageId, locationMessage.Position);
        }
        return locationMessage;
    }
    
    public override void SetMovementInterpolation(bool shouldInterpolate)
    {
        MovementQueue.ShouldInterpolate = shouldInterpolate;
    }

    protected override void HandleIncomingMessages()
    {
        _networkClient.OnMessageReceived += message =>
        {
            Console.WriteLine($"Received command from operator {message.OperatorId}");
            
            _currentMessageId = message.MessageId;
            
            MovementQueue.AddMovement(
                new MovementQueueItem
                {
                    Position = message.Command.Data
                }
            );
        };
    }
}