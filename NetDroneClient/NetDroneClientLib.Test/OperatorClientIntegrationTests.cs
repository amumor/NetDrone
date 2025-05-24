using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using NetDroneClientLib.Clients;
using NetDroneServerLib.Models;

namespace NetDroneClient.Test;

[TestFixture]
public class OperatorClientTests
{
    private UdpClient _mockServer;
    private OperatorClient _operatorClient;
    private const int ServerPort = 8080;
    private const int ClientPort = 8083;
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
        _operatorClient = new OperatorClient(
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
        _operatorClient.Disconnect();
        _mockServer.Dispose();
    }

    [Test]
    public async Task ShouldSendMoveCommandToDrone()
    {
        var targetPosition = new Vec3 { X = 10, Y = 20, Z = 30 };
        
        var moveCommand = new Command
        {
            Cmd = CommandType.Move,
            Data = targetPosition
        };

        Message? receivedMessage = null;

        var serverTask = Task.Run(async () =>
        {
            var result = await _mockServer.ReceiveAsync(); // Receive register message
            result = await _mockServer.ReceiveAsync(); // Receive the move command
            var jsonString = Encoding.UTF8.GetString(result.Buffer);
            receivedMessage = JsonSerializer.Deserialize<Message>(jsonString);
        });

        await Task.Delay(100);
        
        _operatorClient.SendCommandToDrone(moveCommand);

        var timeoutTask = Task.Delay(5000);
        var completedTask = await Task.WhenAny(serverTask, timeoutTask);

        Assert.Multiple(() =>
        {
            Assert.That(completedTask, Is.EqualTo(serverTask));
            Assert.That(receivedMessage, Is.Not.Null);
            Assert.That(receivedMessage.Command.Cmd, Is.EqualTo(CommandType.Move));
            Assert.That(receivedMessage.Command.Data, Is.EqualTo(targetPosition));
            Assert.That(receivedMessage.DroneId, Is.EqualTo(DroneId));
            Assert.That(receivedMessage.OperatorId, Is.EqualTo(OperatorId));
            Assert.That(receivedMessage.MessageId, Is.EqualTo(1));
        });
    }

    [Test]
    public async Task ShouldSendNonMoveCommandToDrone()
    {
        var command = new Command
        {
            Cmd = CommandType.State,
            Data = new Vec3 { X = 0, Y = 0, Z = 0 }
        };

        Message? receivedMessage = null;

        var serverTask = Task.Run(async () =>
        {
            var result = await _mockServer.ReceiveAsync(); // Receive register message
            result = await _mockServer.ReceiveAsync(); // Receive the state command
            var jsonString = Encoding.UTF8.GetString(result.Buffer);
            receivedMessage = JsonSerializer.Deserialize<Message>(jsonString);
        });

        await Task.Delay(100);
        
        _operatorClient.SendCommandToDrone(command);

        var timeoutTask = Task.Delay(5000);
        var completedTask = await Task.WhenAny(serverTask, timeoutTask);

        Assert.Multiple(() =>
        {
            Assert.That(completedTask, Is.EqualTo(serverTask));
            Assert.That(receivedMessage, Is.Not.Null);
            Assert.That(receivedMessage.Command.Cmd, Is.EqualTo(CommandType.State));
            Assert.That(receivedMessage.MessageId, Is.EqualTo(1));
        });
    }

    [Test]
    public async Task ShouldIncrementMessageIdForEachCommand()
    {
        var command1 = new Command { Cmd = CommandType.Move, Data = new Vec3 { X = 1, Y = 1, Z = 1 } };
        var command2 = new Command { Cmd = CommandType.Move, Data = new Vec3 { X = 2, Y = 2, Z = 2 } };

        var receivedMessages = new List<Message>();

        var serverTask = Task.Run(async () =>
        {
            var result = await _mockServer.ReceiveAsync(); // Receive register message
            
            for (int i = 0; i < 2; i++)
            {
                result = await _mockServer.ReceiveAsync();
                var jsonString = Encoding.UTF8.GetString(result.Buffer);
                var message = JsonSerializer.Deserialize<Message>(jsonString);
                receivedMessages.Add(message);
            }
        });

        await Task.Delay(100);
        
        _operatorClient.SendCommandToDrone(command1);
        _operatorClient.SendCommandToDrone(command2);

        var timeoutTask = Task.Delay(5000);
        var completedTask = await Task.WhenAny(serverTask, timeoutTask);

        Assert.Multiple(() =>
        {
            Assert.That(completedTask, Is.EqualTo(serverTask));
            Assert.That(receivedMessages, Has.Count.EqualTo(2));
            Assert.That(receivedMessages[0].MessageId, Is.EqualTo(1));
            Assert.That(receivedMessages[1].MessageId, Is.EqualTo(2));
        });
    }

    [Test]
    public async Task ShouldHandleIncomingMessagesWithReconciliationEnabled()
    {
        _operatorClient.SetReconciliation(true);
        _operatorClient.DroneState.Position = new Vec3 { X = 5, Y = 10, Z = 15 };

        var targetPosition = new Vec3 { X = 5, Y = 10, Z = 15 };
        
        var moveCommand = new Command { Cmd = CommandType.Move, Data = targetPosition };
        _operatorClient.SendCommandToDrone(moveCommand);

        await Task.Delay(100);
        
        var stateMessage = new Message
        {
            MessageId = 1, // Same as the move command we sent
            DroneId = DroneId,
            OperatorId = OperatorId,
            Command = new Command { Cmd = CommandType.State, Data = targetPosition }
        };

        var jsonString = JsonSerializer.Serialize(stateMessage);
        var messageBytes = Encoding.UTF8.GetBytes(jsonString);

        await _mockServer.SendAsync(messageBytes, messageBytes.Length);
        await Task.Delay(500);

        var movement = _operatorClient.GetNextMovement();

        Assert.Multiple(() =>
        {
            Assert.That(movement, Is.Not.Null);
            Assert.That(movement, Is.EqualTo(targetPosition));
        });
    }

    [Test]
    public async Task ShouldIgnoreIncomingMessagesWithReconciliationDisabled()
    {
        _operatorClient.SetReconciliation(false);

        var stateMessage = new Message
        {
            MessageId = 1,
            DroneId = DroneId,
            OperatorId = OperatorId,
            Command = new Command { Cmd = CommandType.State, Data = new Vec3 { X = 1, Y = 2, Z = 3 } }
        };

        var jsonString = JsonSerializer.Serialize(stateMessage);
        var messageBytes = Encoding.UTF8.GetBytes(jsonString);

        await _mockServer.SendAsync(messageBytes, messageBytes.Length);
        await Task.Delay(500);

        var movement = _operatorClient.GetNextMovement();

        Assert.That(movement, Is.Null);
    }

    [Test]
    public void ShouldReturnNullWhenNoMovementsQueued()
    {
        var movement = _operatorClient.GetNextMovement();
        
        Assert.That(movement, Is.Null);
    }

    [Test]
    public void ShouldToggleMovementInterpolation()
    {
        _operatorClient.SetMovementInterpolation(false);
        
        Assert.That(_operatorClient.MovementQueue.ShouldInterpolate, Is.False);
        
        _operatorClient.SetMovementInterpolation(true);
        
        Assert.That(_operatorClient.MovementQueue.ShouldInterpolate, Is.True);
    }

    [Test]
    public void ShouldToggleReconciliation()
    {
        _operatorClient.SetReconciliation(false);
        
        Assert.That(_operatorClient.ShouldReconcile, Is.False);
        
        _operatorClient.SetReconciliation(true);
        
        Assert.That(_operatorClient.ShouldReconcile, Is.True);
    }

    [Test]
    public void ShouldInitializeWithCorrectDroneState()
    {
        Assert.Multiple(() =>
        {
            Assert.That(_operatorClient.DroneState.Id, Is.EqualTo(DroneId));
            Assert.That(_operatorClient.DroneState.OperatorId, Is.EqualTo(OperatorId));
            Assert.That(_operatorClient.MovementQueue.StepCount, Is.EqualTo(InterpolationRate));
        });
    }
}