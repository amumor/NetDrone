using NetDroneClientLib.Models;
using NetDroneServerLib.Models;

namespace NetDroneClientLib.Clients;

public class OperatorClient : AbstractNetDroneClient
{
    public readonly MovementQueue<Vec3> MovementQueue;
    private readonly Dictionary<int, Vec3> _moveStore = [];
    private int _messageId = 1;

    public OperatorClient (
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
        MovementQueue = new OperatorQueue { StepCount = interpolationRate };
        DroneState = new DroneState { Id = droneId, OperatorId = operatorId };
        SetupUdpConnection();
    }

    public void SendCommandToDrone(Command command)
    {
        if (command.Cmd == CommandType.Move)
        {
            _moveStore.Add(_messageId, command.Data);
        }
        _networkClient.SendCommand(_messageId, command, DroneState.Id, DroneState.OperatorId);
        _messageId++;
    }

    public Vec3? GetNextMovement() => 
        MovementQueue.GetNextMovement();
    
    public override void SetMovementInterpolation(bool shouldInterpolate) =>
        MovementQueue.ShouldInterpolate = shouldInterpolate;

    protected override void HandleIncomingMessages()
    {
        _networkClient.OnMessageReceived += message =>
        {
            if (_moveStore.TryGetValue(message.MessageId, out var vector) && vector is not null)
            {
                if (vector.X == DroneState.Position.X
                        && vector.Y == DroneState.Position.Y
                        && vector.Z == DroneState.Position.Z)
                {
                    DroneState.Position = message.Command.Data;
                }
                _moveStore.Remove(message.MessageId);
            }
            else
            {
                DroneState.Position = message.Command.Data;
            }
        };
        if (_moveStore.Count <= 50)
        {
            _moveStore.Clear();
        }
    }
}