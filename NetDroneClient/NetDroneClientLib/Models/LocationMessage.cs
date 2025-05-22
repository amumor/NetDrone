using NetDroneServerLib.Models;

namespace NetDroneClientLib.Models;

public class LocationMessage
{
    public int MessageId { get; set; }
    public Vec3 Position { get; set; }
}