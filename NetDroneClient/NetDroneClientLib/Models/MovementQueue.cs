using net_drone_client.Util;
using NetDroneServerLib.Models;

namespace NetDroneClientLib.Models;

public class MovementQueue
{
    public bool ShouldInterpolate { get; set; } = false;
    private readonly Queue<Vec3> _movements = new();

    public void AddMovement(Vec3 movement)
    {
        if (!ShouldInterpolate)
        {
            _movements.Enqueue(movement);
        }
        else
        {
            var interpolatedMovements = Interpolator.Interpolate(movement, 6);
            foreach (var interpolatedMovement in interpolatedMovements)
            {
                _movements.Enqueue(interpolatedMovement);
            }
        }
    }
    
    public Vec3 GetNextMovement()
    {
        if (_movements.Count > 0)
        {
            return _movements.Dequeue();
        }

        return new Vec3 { X = 0, Y = 0, Z = 0 };
    }

    private List<Vec3> GetPendingMovements()
    {
        var pendingMovements = new List<Vec3>();
        var i = 0;
        while (_movements.Count > 0 && i < 10)
        {
            i++;
            var movement = _movements.Dequeue();
            pendingMovements.Add(movement);
        }
        
        return pendingMovements;
    }
}