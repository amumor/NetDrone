using NetDroneClientLib.Models;
using NetDroneServerLib.Models;

namespace NetDroneClientLib.Clients;

public class OperatorClient : AbstractNetDroneClient
{
    public readonly MovementQueue<Vec3> MovementQueue;
    private readonly Dictionary<int, Vec3> _moveStore = [];
    private int _messageId = 1;
    public bool ShouldReconcile { get; set; }

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
        Console.WriteLine(
            $"{new String('-', 80)}\n" +
            $"Sending command [{command.Cmd}] to drone {DroneState.Id} " +
            $"with operator {DroneState.OperatorId} and message ID {_messageId}"
        );
        _networkClient.SendCommand(_messageId, command, DroneState.Id, DroneState.OperatorId);
        _messageId++;
    }

    public Vec3? GetNextMovement() => 
        MovementQueue.GetNextMovement();
    
    public override void SetMovementInterpolation(bool shouldInterpolate) =>
        MovementQueue.ShouldInterpolate = shouldInterpolate;
    
    public void SetReconciliation(bool shouldReconcile) =>
        ShouldReconcile = shouldReconcile;

    protected override void HandleIncomingMessages()
    {
        _networkClient.OnMessageReceived += message =>
        {
            Console.WriteLine(
                $"Received command [{message.Command.Cmd}] " +
                $"with location ({message.Command.Data}) " +
                $"from drone {message.DroneId}\n"
            );
            if (!ShouldReconcile)
                return;
            Console.WriteLine("Reconciling movement with drone position...");
            if (_moveStore.TryGetValue(message.MessageId, out var vector) && vector is not null)
            {
                if (vector.X == DroneState.Position.X
                        && vector.Y == DroneState.Position.Y
                        && vector.Z == DroneState.Position.Z)
                {
                    MovementQueue.AddMovement(vector);
                }
                _moveStore.Remove(message.MessageId);
            }
            else
            {
                MovementQueue.AddMovement(message.Command.Data);
            }
        };
        if (_moveStore.Count <= 50)
        {
            _moveStore.Clear();
        }
    }
}