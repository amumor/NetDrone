using NetDroneClientLib.Clients;
using NetDroneClientLib.Models;

namespace GodotDroneVisualization.Entities;

public class OperatorService
{
    
    public OperatorClient OperatorClient { get; private set; }
    
    public void Setup()
    {
        OperatorClient = new OperatorClient(
            clientPort: 5002,
            serverPort: 4002,
            serverIp: "127.0.0.1",
            droneId: 1
        );
    }
    
    public void MoveDrone(int x, int y, int z)
    {
        var command = new Command(
            CommandType.Move,
            new Vec3<int>(x, y, z)
        );
        OperatorClient.SendCommandToDrone(command);
    }
}