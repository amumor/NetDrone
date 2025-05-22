using NetDroneClientLib.Models;
using NetDroneServerLib.Models;

namespace net_drone_client.Util;

public static class Interpolator
{
    public static List<Vec3> Interpolate(Vec3 movement, int steps)
    {
        var interpolatedMovements = new List<Vec3>();
        var stepMovement = new Vec3
        {
            X = movement.X / steps,
            Y = movement.Y / steps,
            Z = movement.Z / steps
        };

        for (var i = 0; i < steps; i++)
        {
            interpolatedMovements.Add(stepMovement);
        }
        
        var remainder = new Vec3
        {
            X = movement.X % steps,
            Y = movement.Y % steps,
            Z = movement.Z % steps
        };

        if (remainder.X != 0 || remainder.Y != 0 || remainder.Z != 0)
        {
            interpolatedMovements[interpolatedMovements.Count - 1] = new Vec3
            {
                X = stepMovement.X + remainder.X,
                Y = stepMovement.Y + remainder.Y,
                Z = stepMovement.Z + remainder.Z
            };
        }

        return interpolatedMovements;
    }
}