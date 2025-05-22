using net_drone_client.Util;
using NetDroneServerLib.Models;

namespace NetDroneClientLib.Models;

public class MovementQueue
{
    public bool ShouldInterpolate { get; set; } = false;
    private readonly Queue<LocationMessage> _movements = new();

    public void AddMovement(LocationMessage movement)
    {
        if (!ShouldInterpolate)
        {
            _movements.Enqueue(movement);
        }
        else
        {
            var interpolatedMovements = Interpolator.Interpolate(movement.Position, 6);
            foreach (var interpolatedMovement in interpolatedMovements)
            {
                _movements.Enqueue(new LocationMessage
                {
                    MessageId = movement.MessageId,
                    Position = interpolatedMovement
                });
            }
        }
    }
    
    public LocationMessage GetNextMovement()
    {
        if (_movements.Count > 0)
        {
            return _movements.Dequeue();
        }

        return null;
    }
}