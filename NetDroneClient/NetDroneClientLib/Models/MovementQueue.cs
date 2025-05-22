namespace NetDroneClientLib.Models;

public class MovementQueue
{
    private readonly Queue<Vec3<int>> _movements = new();

    public void AddMovement(Vec3<int> movement)
    {
        _movements.Enqueue(movement);
    }

    public List<Vec3<int>> GetPendingMovements()
    {
        var pendingMovements = new List<Vec3<int>>();
        while (_movements.Count > 0)
        {
            pendingMovements.Add(_movements.Dequeue());
        }
        
        return pendingMovements;
    }
}