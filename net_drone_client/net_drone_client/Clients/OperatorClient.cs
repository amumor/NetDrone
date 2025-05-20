using net_drone_client.Communication;
using net_drone_client.Models;

namespace net_drone_client.Clients;

public class OperatorClient : AbstractNetDroneClient
{
    public OperatorClient(string ip, int port)
    {
        Ip = ip;
        Port = port;
        SetupUdpConnection();
    }
    
    public void SendCommandToDrone(Command command)
    {
        _networkClient.SendCommand(command);
    }

    public Vec3<float> GetLatestLocationFromDrone()
    {
        return DroneState.Position;
    }
    
    protected override void HandleIncomingMessages()
    {
        _networkClient.OnMessageReceived += message =>
        {
            DroneState.Position = new Vec3<float>(
                message.X,
                message.Y,
                message.Z
            );
            Console.WriteLine($"Handling message for DroneClient: {message.Type}");
        };
    }
}