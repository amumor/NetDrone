using NetDroneClientLib.Clients;
using NetDroneClientLib.Models;
using NetDroneServerLib.Models;

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
            droneId: 1,
            operatorId: 1
        );
    }
    
    public void MoveDrone(int x, int y, int z)
    {
        var command = new Command
        {
            Cmd = CommandType.Move,
            Data = new Vec3
            {
                X = x,
                Y = y,
                Z = z
            }
        };
        OperatorClient.SendCommandToDrone(command);
    }
    
    public Vec3 getLatestLocationFromDrone()
    {
        return OperatorClient.GetLatestLocationFromDrone();
    }
}