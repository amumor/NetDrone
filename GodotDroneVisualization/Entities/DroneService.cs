using System;
using System.Collections.Generic;
using NetDroneClientLib;
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
            droneId: 1
        );
    }
    
    public void UpdateDronePosition()
    {
        var currentState = DroneClient.DroneState;
        DroneClient.SendLocationToOperator(currentState.Position);
    }
    
    public List<Vec3<int>> GetPendingMovements()
    {
        return DroneClient.GetPendingMovements();
    }
}