using NetDroneClientLib.Models;
using NetDroneServerLib.Models;

namespace net_drone_client.Util;

public static class Interpolator
{
    public static List<Vec3> Interpolate(Vec3 start, Vec3 end, int steps)
    {
        var interpolatedPoints = new List<Vec3>();

        for (var i = 0; i <= steps; i++)
        {
            var t = (float)i / steps;
            var x = (int)(start.X + (end.X - start.X) * t);
            var y = (int)(start.Y + (end.Y - start.Y) * t);
            var z = (int)(start.Z + (end.Z - start.Z) * t);
            interpolatedPoints.Add(new Vec3 { X = x, Y = y, Z = z });
        }

        return interpolatedPoints;
    }
}