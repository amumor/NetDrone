using NetDroneServerLib.Models;

namespace NetDroneClientLib.Models;

public class MovementQueueItem
{
    public bool IsInterpolated { get; set; }
    public Vec3 Position { get; set; } = new();
}   