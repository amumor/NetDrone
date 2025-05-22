

using NetDroneClientLib.Models;
using NetDroneServerLib.Models;

namespace NetDroneClientLib.Clients;

public class OperatorClient : AbstractNetDroneClient
{
    public readonly MovementQueue<Vec3> MovementQueue = new OperatorQueue();
    private int _messageId = 1;
    private readonly Dictionary<int, Vec3> _moveStore = [];

    public OperatorClient(int clientPort, int serverPort, string serverIp, int droneId, int operatorId)
    {
        ClientPort = clientPort;
        ServerPort = serverPort;
        ServerIp = serverIp;
        DroneState = new DroneState { Id = droneId, OperatorId = operatorId };
        SetupUdpConnection();
    }

    public void SendCommandToDrone(Command command)
    {
        Console.WriteLine($"Sending command to drone {DroneState.Id}: {command}");
        if (command.Cmd == CommandType.Move)
        {
            _moveStore.Add(_messageId, command.Data);
            Console.WriteLine($"Storing move in store with id: {_messageId}");
        }
        _networkClient.SendCommand(_messageId, command, DroneState.Id, DroneState.OperatorId);
        _messageId++;
    }

    public void UpdateDronePosition(Vec3 position)
    {
        MovementQueue.AddMovement(position);
    }

    public Vec3? GetNextMovement()
    {
        return MovementQueue.GetNextMovement();
    }
    
    public void SetMovementInterpolation(bool shouldInterpolate)
    {
        MovementQueue.ShouldInterpolate = shouldInterpolate;
    }

    protected override void HandleIncomingMessages()
    {
        _networkClient.OnMessageReceived += message =>
        {
            Console.WriteLine($"Received location update from drone {message.DroneId}");
            Console.WriteLine($"Preforming reconciliation...");
            if (_moveStore.TryGetValue(message.MessageId, out var vector) && vector is not null)
            {
                Console.WriteLine("--------------------------------------");
                Console.WriteLine($"id: {message.MessageId}");
                Console.WriteLine($"vector in store: {vector}");
                Console.WriteLine($"vector in message: {message.Command.Data}");
                if (vector.X == DroneState.Position.X
                        && vector.Y == DroneState.Position.Y
                        && vector.Z == DroneState.Position.Z)
                {
                    Console.WriteLine("Position matches id, but move has not been performed.");
                    // If drone position is not equal to drone position in store,
                    // store is updated to actual position.
                    DroneState.Position = message.Command.Data;
                    Console.WriteLine("--------------------------------------");
                }
                _moveStore.Remove(message.MessageId);
            }
            else
            {
                Console.WriteLine("Id not found in store, storing move.");
                // If state update is not an answer to a Move command, 
                // update position in store in case of unintended movement (wind drift, etc).
                DroneState.Position = message.Command.Data;
            }
        };
        if (_moveStore.Count <= 50)
        {
            // clear old moves in case of mem leak
            _moveStore.Clear();
        }
    }
}