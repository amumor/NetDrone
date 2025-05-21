namespace net_drone_client.Models;

public class DroneState
{ 
    public int Id { get; set; }
    public Vec3<int> Position { get; set; } = new(0, 0, 0);
    public Vec3<int> PredictedPosition { get; set; } = new(0, 0, 0);
}