namespace net_drone_client.Models;

public class MovementQueue
{
    private readonly Queue<Vec3<float>> _movements = new();

    public void AddMovement(Vec3<float> movement)
    {
        _movements.Enqueue(movement);
    }

    public List<Vec3<float>> GetPendingMovements() =>
        _movements
            .Reverse()
            .ToList();
}