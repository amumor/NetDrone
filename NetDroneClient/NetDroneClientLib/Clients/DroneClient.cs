using net_drone_client.Util;
using NetDroneClientLib.Models;
using NetDroneServerLib.Models;

namespace NetDroneClientLib.Clients;

public class DroneClient : AbstractNetDroneClient
{
    public readonly MovementQueue _movementQueue = new();

    public DroneClient(int clientPort, int serverPort, string serverIp, int droneId, int operatorId)
    {
        ClientPort = clientPort;
        ServerPort = serverPort;
        ServerIp = serverIp;
        DroneState = new DroneState { Id = droneId, OperatorId = operatorId };
        SetupUdpConnection();
    }

    public void SendLocationToOperator(Vec3 currentLocation)
    {
        var command = new Command();
        _networkClient.SendCommand(command, DroneState.Id, DroneState.OperatorId);
    }

    public Vec3 GetNextMovement()
    {
        return _movementQueue.GetNextMovement();
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
                new Vec3
                {
                    X = data.X,
                    Y = data.Y,
                    Z = data.Z
                }
            );
        };
    }
}