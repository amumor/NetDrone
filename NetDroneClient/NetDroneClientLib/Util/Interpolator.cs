using NetDroneClientLib.Models;
using NetDroneServerLib.Models;

namespace NetDroneClientLib.Util;

public static class Interpolator
{
    private static Vec3 InterpolateVec3(Vec3 start, Vec3 end, float t)
    {
        return new Vec3
        {
            X = (int)(start.X + (end.X - start.X) * t),
            Y = (int)(start.Y + (end.Y - start.Y) * t),
            Z = (int)(start.Z + (end.Z - start.Z) * t)
        };
    }

    public static List<MovementQueueItem> InterpolateDroneMovement(Vec3 start, Vec3 end, int steps)
    {
        var messages = new List<MovementQueueItem>();

        for (var i = 0; i <= steps; i++)
        {
            var t = (float)i / steps;
            var position = InterpolateVec3(start, end, t);

            messages.Add(new MovementQueueItem
            {
                IsInterpolated = i != steps,
                Position = position
            });
        }

        return messages;
    }

    public static List<Vec3> InterpolateOperatorMovement(Vec3 start, Vec3 end, int steps)
    {
        var interpolatedPoints = new List<Vec3>();

        for (var i = 0; i <= steps; i++)
        {
            var t = (float)i / steps;
            interpolatedPoints.Add(InterpolateVec3(start, end, t));
        }

        return interpolatedPoints;
    }
}