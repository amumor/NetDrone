using net_drone_client.Util;
using NetDroneServerLib.Models;

namespace NetDroneClientLib.Models;

public abstract class MovementQueue<T>
{
    public bool ShouldInterpolate { get; set; }
    public readonly Queue<T> _movements = new();
    public T? _lastMovement;

    public void AddMovement(T movement)
    {
        if (IsFirstMovement(movement) || _lastMovement == null)
        {
            _lastMovement = movement;
        }

        if (!ShouldInterpolate)
        {
            _movements.Enqueue(ConvertToOutput(movement));
        }
        else
        {
            Console.WriteLine($"Last movement: {_lastMovement}");
            Console.WriteLine($"Current movement: {movement}");
            var interpolatedMovements = Interpolate(_lastMovement, movement);
            foreach (var interpolatedMovement in interpolatedMovements)
            {
                _movements.Enqueue(interpolatedMovement);
            }
            _lastMovement = movement; // Update after interpolation
        }
    }

    public T? GetNextMovement()
    {
        if (_movements.Count > 0)
        {
            var movement = _movements.Dequeue();
            Console.WriteLine($"Dequeued movement: {movement}");
            return movement;
        }
        return default(T);
    }

    protected abstract bool IsFirstMovement(T movement);
    protected abstract T ConvertToOutput(T movement);
    protected abstract IEnumerable<T> Interpolate(T start, T end);
}

public class DroneQueue : MovementQueue<LocationMessage>
{
    protected override bool IsFirstMovement(LocationMessage movement) => movement.MessageId == 1;
    
    protected override LocationMessage ConvertToOutput(LocationMessage movement) => movement;

    protected override IEnumerable<LocationMessage> Interpolate(LocationMessage start, LocationMessage end)
    {
        return Interpolator.InterpolateDroneMovement(start.MessageId, start.Position, end.Position, 6);
    }
}

public class OperatorQueue : MovementQueue<Vec3>
{
    protected override bool IsFirstMovement(Vec3 movement) => false; // Or whatever logic you need
    
    protected override Vec3 ConvertToOutput(Vec3 movement) => movement;

    protected override IEnumerable<Vec3> Interpolate(Vec3 start, Vec3 end)
    {
        return Interpolator.InterpolateOperatorMovement(start, end, 6);
    }
}