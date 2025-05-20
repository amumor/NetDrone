using net_drone_client.Models;

namespace net_drone_client.Clients;

public class OperatorClient : AbstractNetDroneClient
{
    public OperatorClient(int clientPort, int serverPort, string serverIp, int droneId)
    {
        ClientPort = clientPort;
        ServerPort = serverPort;
        ServerIp = serverIp;
        DroneState = new DroneState { Id = droneId };
        SetupUdpConnection();
    }
    
    public void SendCommandToDrone(Command command)
    {
        _networkClient.SendCommand(command, DroneState.Id);
    }

    public Vec3<float> GetLatestLocationFromDrone()
    {
        var position = DroneState.Position;
        Console.WriteLine($"Fetching latest location from drone: {position}");
        return position;
    }
    
    protected override void HandleIncomingMessages()
    {
        _networkClient.OnMessageReceived += message =>
        {
            Console.WriteLine($"Received location update from drone {message.DroneId}");
            var data = message.Command.Data;
            DroneState.Position = new Vec3<float>(
                data.X,
                data.Y,
                data.Z
            );
        };
    }
}