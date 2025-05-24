using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using NetDroneClientLib.Clients;
using NetDroneClientLib.Models;
using NetDroneServerLib.Models;

namespace NetDroneClient.Test;

[TestFixture]
public class DroneClientTests
{
    private UdpClient _mockServer;
    private DroneClient _droneClient;
    private const int ServerPort = 8080;
    private const int ClientPort = 8082;
    private const string ServerIp = "127.0.0.1";
    private const int DroneId = 1;
    private const int OperatorId = 100;
    private const int InterpolationRate = 4;

    [SetUp]
    public void Setup()
    {
        _mockServer = new UdpClient();
        _mockServer.Client.Bind(new IPEndPoint(IPAddress.Any, ServerPort));
        _mockServer.Connect(
            new IPEndPoint(IPAddress.Parse(ServerIp), ClientPort)
        );
        _droneClient = new DroneClient(
            ClientPort, 
            ServerPort, 
            ServerIp, 
            DroneId, 
            OperatorId, 
            InterpolationRate
        );
    }

    [TearDown]
    public void TearDown()
    {
        _droneClient.Disconnect();
        _mockServer.Dispose();
    }

    [Test]
    public async Task ShouldSendLocationToOperatorWhenMovementComplete()
    {
        var targetPosition = new Vec3 { X = 10, Y = 20, Z = 30 };
        
        var movementCommand = new Command
        {
            Cmd = CommandType.Move,
            Data = targetPosition
        };

        var incomingMessage = new Message
        {
            MessageId = 123,
            DroneId = DroneId,
            OperatorId = OperatorId,
            Command = movementCommand
        };

        Message? receivedMessage = null;

        var serverTask = Task.Run(async () =>
        {
            var result = await _mockServer.ReceiveAsync(); // Receive register message
            result = await _mockServer.ReceiveAsync(); // Receive the state update
            var jsonString = Encoding.UTF8.GetString(result.Buffer);
            receivedMessage = JsonSerializer.Deserialize<Message>(jsonString);
        });

        // Send movement command to drone
        var jsonString = JsonSerializer.Serialize(incomingMessage);
        var messageBytes = Encoding.UTF8.GetBytes(jsonString);
        await _mockServer.SendAsync(messageBytes, messageBytes.Length);

        await Task.Delay(500);

        // Process movements until we get the final one
        MovementQueueItem? movement;
        do
        {
            movement = _droneClient.GetNextMovement();
        } while (movement is { IsInterpolated: true });

        var timeoutTask = Task.Delay(5000);
        var completedTask = await Task.WhenAny(serverTask, timeoutTask);

        Assert.Multiple(() =>
        {
            Assert.That(completedTask, Is.EqualTo(serverTask));
            Assert.That(receivedMessage, Is.Not.Null);
            Assert.That(receivedMessage.Command.Cmd, Is.EqualTo(CommandType.State));
            Assert.That(receivedMessage.Command.Data, Is.EqualTo(targetPosition));
            Assert.That(receivedMessage.DroneId, Is.EqualTo(DroneId));
            Assert.That(receivedMessage.OperatorId, Is.EqualTo(OperatorId));
        });
    }

    [Test]
    public async Task ShouldReceiveMovementCommandsFromOperator()
    {
        var targetPosition = new Vec3 { X = 5, Y = 10, Z = 15 };
        
        var movementCommand = new Command
        {
            Cmd = CommandType.Move,
            Data = targetPosition
        };

        var incomingMessage = new Message
        {
            MessageId = 456,
            DroneId = DroneId,
            OperatorId = OperatorId,
            Command = movementCommand
        };

        var jsonString = JsonSerializer.Serialize(incomingMessage);
        var messageBytes = Encoding.UTF8.GetBytes(jsonString);

        await _mockServer.SendAsync(messageBytes, messageBytes.Length);
        await Task.Delay(500);

        var movement = _droneClient.GetNextMovement();

        Assert.Multiple(() =>
        {
            Assert.That(movement, Is.Not.Null);
            Assert.That(movement.Position, Is.EqualTo(targetPosition));
            Assert.That(movement.IsInterpolated, Is.False);
        });
    }

    [Test]
    public void ShouldReturnNullWhenNoMovementsQueued()
    {
        var movement = _droneClient.GetNextMovement();
        
        Assert.That(movement, Is.Null);
    }

    [Test]
    public void ShouldToggleMovementInterpolation()
    {
        _droneClient.SetMovementInterpolation(false);
        
        Assert.That(_droneClient.MovementQueue.ShouldInterpolate, Is.False);
        
        _droneClient.SetMovementInterpolation(true);
        
        Assert.That(_droneClient.MovementQueue.ShouldInterpolate, Is.True);
    }

    [Test]
    public void ShouldInitializeWithCorrectDroneState()
    {
        Assert.Multiple(() =>
        {
            Assert.That(_droneClient.DroneState.Id, Is.EqualTo(DroneId));
            Assert.That(_droneClient.DroneState.OperatorId, Is.EqualTo(OperatorId));
            Assert.That(_droneClient.DroneState.Position, Is.Not.Null);
            Assert.That(_droneClient.MovementQueue.StepCount, Is.EqualTo(InterpolationRate));
        });
    }
}