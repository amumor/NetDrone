using NetDroneServerLib.Models;

namespace NetDroneClientLib.Models;

public class DroneState
{
    public int Id { get; set; }
    public int OperatorId { get; set; }
    public Vec3 Position { get; set; } = new(){X = 0, Y = 0, Z = 0};
}