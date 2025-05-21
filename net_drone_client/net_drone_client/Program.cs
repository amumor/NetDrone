using net_drone_client.Clients;
using net_drone_client.Models;

namespace net_drone_client;

class Program
{
    public static ClientType CLIENT_TYPE;
    
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
            CLIENT_TYPE = ClientType.Operator;
            var operatorIdArg = args.FirstOrDefault(arg => arg.StartsWith("--id="));
            if (operatorIdArg == null)
            {
                Console.WriteLine("Error: Missing --id argument for operator.");
                return;
            }

            if (!int.TryParse(operatorIdArg.Split('=')[1], out var operatorId))
            {
                Console.WriteLine("Error: Invalid operator ID.");
                return;
            }
            RunOperatorMode(operatorId);
        }
        else if (args.Contains("--drone"))
        {
            CLIENT_TYPE = ClientType.Drone;
            var droneIdArg = args.FirstOrDefault(arg => arg.StartsWith("--id="));
            if (droneIdArg == null)
            {
                Console.WriteLine("Error: Missing --drone-id argument.");
                return;
            }

            if (!int.TryParse(droneIdArg.Split('=')[1], out var droneId))
            {
                Console.WriteLine("Error: Invalid drone ID.");
                return;
            }
            RunDroneMode(droneId);
        }

        Console.WriteLine("Press Ctrl+C to exit.");
        cancellationTokenSource.Token.WaitHandle.WaitOne();
    }

    public static void RunOperatorMode(int droneId)
    {
        Console.WriteLine($"Running operator mode on drone: {droneId}");
        var operatorClient = new OperatorClient(
            4102, 
            4002, 
            "127.0.0.1", 
            droneId
        );
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

    public static void RunDroneMode(int droneId)
    {
        Console.WriteLine($"Running drone mode with ID: {droneId}");
        var droneClient = new DroneClient(
            4101,
            4001,
            "127.0.0.1",
            droneId
        );

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
