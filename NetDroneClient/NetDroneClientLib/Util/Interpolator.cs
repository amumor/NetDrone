using NetDroneClientLib.Models;
using NetDroneServerLib.Models;

namespace net_drone_client.Util;

public static class Interpolator
{
    public static List<LocationMessage> InterpolateDroneMovement(int messageId, Vec3 start, Vec3 end, int steps)
    {
        var messages = new List<LocationMessage>();

        for (var i = 0; i <= steps; i++)
        {
            var t = (float)i / steps;
            var position = new Vec3 
            { 
                X = (int)(start.X + (end.X - start.X) * t),
                Y = (int)(start.Y + (end.Y - start.Y) * t),
                Z = (int)(start.Z + (end.Z - start.Z) * t)
            };
        
            messages.Add(new LocationMessage 
            { 
                MessageId = i == steps ? messageId : 0, 
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
            var x = (int)(start.X + (end.X - start.X) * t);
            var y = (int)(start.Y + (end.Y - start.Y) * t);
            var z = (int)(start.Z + (end.Z - start.Z) * t);
            interpolatedPoints.Add(new Vec3 { X = x, Y = y, Z = z });
        }

        return interpolatedPoints;
    }
}