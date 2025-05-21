using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using NetDroneClientLib.Clients;
using NetDroneClientLib.Models;

namespace NetDroneClient.Test;


[TestFixture]
public class OperatorClientIntegrationTests
{
    private UdpClient _mockServer;
    private OperatorClient _operatorClient;
    private const int ServerPort = 8080;
    private const int ClientPort = 8082;
    private const string ServerIp = "127.0.0.1";
    private const int DroneId = 1;

    [SetUp]
    public void Setup()
    {
        _mockServer = new UdpClient();
        _mockServer.Client.Bind(new IPEndPoint(IPAddress.Any, ServerPort));
        _mockServer.Connect(
            new IPEndPoint(IPAddress.Parse(ServerIp), ClientPort)
        );

        _operatorClient = new OperatorClient(ClientPort, ServerPort, ServerIp, DroneId);
    }

    [TearDown]
    public void TearDown()
    {
        _operatorClient.Disconnect();
        _mockServer.Dispose();
    }

    [Test]
    public async Task ShouldSendMovementCommandToServer()
    {
        var moveCommand = new Command(
            CommandType.Move,
            new Vec3<int>(10, 20, 30)
        );

        var expectedMessage = new ServerMessage
        {
            DroneId = 1,
            Command = moveCommand
        };

        ServerMessage? receivedMessage = null;

        var serverTask = Task.Run(async () =>
        {
            var result = await _mockServer.ReceiveAsync();
            var jsonString = Encoding.UTF8.GetString(result.Buffer);
            receivedMessage = JsonSerializer.Deserialize<ServerMessage>(jsonString);
        });

        _operatorClient.SendCommandToDrone(moveCommand);

        var timeoutTask = Task.Delay(5000);

        var completedTask = await Task.WhenAny(serverTask, timeoutTask);

        Assert.That(completedTask, Is.EqualTo(serverTask));
        Assert.That(receivedMessage, Is.Not.Null);
        Assert.That(
            JsonSerializer.Serialize(receivedMessage),
            Is.EqualTo(JsonSerializer.Serialize(expectedMessage))
        );
    }

    [Test]
    public async Task ShouldReceiveLocationMessageFromServer()
    {
        for (var i = 0; i < 2; i++)
        {
            var movement = new Command(
                CommandType.State,
                new Vec3<int>(i + 1, 10, 15)
            );

            var serverMessage = new ServerMessage
            {
                DroneId = 1,
                Command = movement
            };

            var jsonString = JsonSerializer.Serialize(serverMessage);
            var messageBytes = Encoding.UTF8.GetBytes(jsonString);

            await _mockServer.SendAsync(messageBytes, messageBytes.Length);
        }

        await Task.Delay(500);

        var latestLocation = _operatorClient.GetLatestLocationFromDrone();
        Assert.That(latestLocation.X, Is.EqualTo(2));
    }
}