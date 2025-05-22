

using NetDroneClientLib.Models;
using NetDroneServerLib.Models;

namespace NetDroneClientLib.Clients;

public class OperatorClient : AbstractNetDroneClient
{
    private readonly MovementQueue _movementQueue = new();
    private int _messageId = 1;

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
        _networkClient.SendCommand(_messageId, command, DroneState.Id, DroneState.OperatorId);
        _messageId++;
    }

    public Vec3 GetLatestLocationFromDrone()
    {
        var position = DroneState.Position;
        Console.WriteLine($"Fetching latest location from drone: {position}");
        return position;
    }
    
    
    protected override void HandleIncomingMessages()
    {
        // HashMap<int moveId, (x, y, z)> MoveStore
        //
        // - -> Move sendes ut
        //     - Lagre Move i MoveSore
        // - <- State kommer inn
        //     - Sjekke message.data.x y z opp mot MoveStore.get(message.moveId)
        //         - Hvis lik
        //             - gi faen
        //         - Hvis ikke lik
        //             - Oppdater operator pos
        //         - hvis null
        //             - oppdater operator pos


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