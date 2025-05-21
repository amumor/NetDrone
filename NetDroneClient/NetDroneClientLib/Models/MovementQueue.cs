namespace NetDroneClientLib.Models;

public class MovementQueue
{
    private readonly Queue<Vec3<int>> _movements = new();

    public void AddMovement(Vec3<int> movement)
    {
        _movements.Enqueue(movement);
    }

    public List<Vec3<int>> GetPendingMovements() =>
        _movements
            .Reverse()
            .ToList();
}