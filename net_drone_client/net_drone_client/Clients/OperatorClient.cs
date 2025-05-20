using net_drone_client.Models;

namespace net_drone_client.Clients;

public class OperatorClient : AbstractNetDroneClient
{
    public OperatorClient(string ip, int port)
    {
        Ip = ip;
        Port = port;
        DroneState = new DroneState();
        SetupUdpConnection();
    }
    
    public void SendCommandToDrone(Command command)
    {
        Console.WriteLine($"Sending command to drone: {command}");
        _networkClient.SendCommand(command);
    }

    public Vec3<float> GetLatestLocationFromDrone()
    {
        Console.WriteLine($"Getting latest location from drone");
        return DroneState.Position;
    }
    
    protected override void HandleIncomingMessages()
    {
        _networkClient.OnMessageReceived += message =>
        {
            Console.WriteLine($"Received message: {message}");
            var data = message.Command.Data;
            DroneState.Position = new Vec3<float>(
                data.X,
                data.Y,
                data.Z
            );
        };
    }
}