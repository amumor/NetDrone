using net_drone_client.Clients;
using net_drone_client.Models;

// DroneClient
var droneClient = new DroneClient("192.88.12.244", 8888);

droneClient.Initialize();

var command = new Command(
    CommandType.Move,
    new Vec3<int>(10, 100, 0)
);

// gets called every refresh in Godot
droneClient.GetPendingMovements();
droneClient.SendLocationToOperator(command);

droneClient.Disconnect();

// OperatorClient
var operatorClient = new OperatorClient("192.88.12.244", 8888);

operatorClient.Initialize();

operatorClient.SendCommandToDrone(command);
operatorClient.GetLatestLocationFromDrone();

operatorClient.Disconnect();
