namespace net_drone_client.Models;

public class DroneState
{ 
    public int Id { get; set; }
    public Vec3<float> Position { get; set; } = new(0, 0, 0);
}