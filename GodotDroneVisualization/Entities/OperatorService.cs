using System.Threading.Tasks;
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
            operatorId: 1,
            interpolationRate: 6
        );
    }
    
    public void MoveDrone(int x, int y, int z)
    {
        var pos = new Vec3 { X = x, Y = y, Z = z };
        OperatorClient.DroneState.Position = pos;
        
        var queue = OperatorClient.MovementQueue;
        if (queue.Movements.Count == 0 || !pos.Equals(queue.LastMovement))
        {
            queue.Movements.Clear(); 
            queue.AddMovement(pos);
        }

        var command = new Command
        {
            Cmd = CommandType.Move,
            Data = new Vec3 { X = x, Y = y, Z = z }
        };
        OperatorClient.SendCommandToDrone(command);
    }
    
    public Vec3 GetNextMovement()
    {
        return OperatorClient.GetNextMovement();
    }
    
    public void ToggleInterpolation()
    {
        var shouldInterpolate = !OperatorClient.MovementQueue.ShouldInterpolate;
        OperatorClient.SetMovementInterpolation(shouldInterpolate);
    }
}