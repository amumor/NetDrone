using net_drone_client.Clients;
using net_drone_client.Models;

namespace net_drone_client;

class Program
{
    static void Main(string[] args)
    {
        var cancellationTokenSource = new CancellationTokenSource();

        Console.CancelKeyPress += (sender, eventArgs) =>
        {
            Console.WriteLine("Shutting down...");
            cancellationTokenSource.Cancel();
            eventArgs.Cancel = true; // Prevent immediate termination
        };

        if (args.Contains("--operator"))
        {
            Console.WriteLine("Running in operator mode...");
            RunOperatorMode();
        }
        else if (args.Contains("--drone"))
        {
            Console.WriteLine("Running in drone mode...");
            RunDroneMode();
        }

        Console.WriteLine("Press Ctrl+C to exit.");
        // Block the main thread until cancellation is requested
        cancellationTokenSource.Token.WaitHandle.WaitOne();
    }

    public static void RunOperatorMode()
    {
        var operatorClient = new OperatorClient("127.0.0.1", 5000);
        /*
        var command = new Command(
            CommandType.Move,
            new Vec3<int>(10, 100, 0)
        );

        operatorClient.SendCommandToDrone(command);
        operatorClient.GetLatestLocationFromDrone();

        operatorClient.Disconnect();
        */
    }

    public static void RunDroneMode()
    {
        var droneClient = new DroneClient("127.0.0.1", 5001);

        /*
        var command = new Command(
            CommandType.Move,
            new Vec3<int>(10, 100, 0)
        );
        
        droneClient.GetPendingMovements();
        droneClient.SendLocationToOperator(command);

        droneClient.Disconnect();
        */
    }
}
