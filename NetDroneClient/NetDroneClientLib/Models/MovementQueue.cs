using NetDroneClientLib.Util;
using NetDroneServerLib.Models;

namespace NetDroneClientLib.Models;

public abstract class MovementQueue<T>
{
    public bool ShouldInterpolate { get; set; }
    public required int StepCount { get; set; }
    public readonly Queue<T> Movements = new();
    public T? LastMovement;

    public void AddMovement(T movement)
    {
        LastMovement ??= movement;

        if (!ShouldInterpolate)
        {
            Movements.Enqueue(ConvertToOutput(movement));
        }
        else
        {
            var interpolatedMovements = Interpolate(LastMovement, movement);
            foreach (var interpolatedMovement in interpolatedMovements)
            {
                Movements.Enqueue(interpolatedMovement);
            }
            LastMovement = movement;
        }
    }

    public T? GetNextMovement()
    {
        if (Movements.Count == 0)
        {
            return default;
            
        }
        
        var movement = Movements.Dequeue();
        Console.WriteLine($"Executing next movement from queue: ({movement})");
        return movement;
    }
    
    protected abstract T ConvertToOutput(T movement);
    protected abstract IEnumerable<T> Interpolate(T start, T end);
}

public class DroneQueue : MovementQueue<MovementQueueItem>
{
    protected override MovementQueueItem ConvertToOutput(MovementQueueItem movement) => movement;

    protected override IEnumerable<MovementQueueItem> Interpolate(MovementQueueItem start, MovementQueueItem end)
    {
        return Interpolator.InterpolateDroneMovement(start.Position, end.Position, StepCount);
    }
}

public class OperatorQueue : MovementQueue<Vec3>
{
    protected override Vec3 ConvertToOutput(Vec3 movement) => movement;

    protected override IEnumerable<Vec3> Interpolate(Vec3 start, Vec3 end)
    {
        return Interpolator.InterpolateOperatorMovement(start, end, StepCount);
    }
}