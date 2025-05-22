using NetDroneClientLib.Clients;
using NetDroneClientLib.Models;

namespace GodotDroneVisualization.Entities;

public class DroneService
{
    public DroneClient DroneClient { get; private set; }
    
    public void Setup()
    {
        DroneClient = new DroneClient(
            clientPort: 5001,
            serverPort: 4001,
            serverIp: "127.0.0.1",
            droneId: 1,
            operatorId: 1
        );
    }
    
    public void ToggleInterpolation()
    {
        var shouldInterpolate = !DroneClient._movementQueue.ShouldInterpolate;
        DroneClient.SetMovementInterpolation(shouldInterpolate);
    }
    
    public LocationMessage GetNextMovement()
    {
        return DroneClient.GetNextMovement();
    }
}