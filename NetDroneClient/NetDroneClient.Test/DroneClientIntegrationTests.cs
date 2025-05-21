using net_drone_client.Clients;
using net_drone_client.Models;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace net_drone_client.test;

[TestFixture]
public class DroneClientIntegrationTests
{
    private UdpClient _mockServer;
    private DroneClient _droneClient;
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
        _droneClient = new DroneClient(ClientPort, ServerPort, ServerIp, DroneId);
    }
    
    [TearDown]
    public void TearDown()
    {
        _droneClient.Disconnect();
        _mockServer.Dispose();
    }
    
    [Test]
    public async Task ShouldSendLocationMessageToServer()
    {
        var locationCommand = new Command(
            CommandType.State, 
            new Vec3<int>(10, 20, 30)
        );
        
        var expectedMessage = new ServerMessage
        {
            DroneId = 1,
            Command = locationCommand
        };
        
        ServerMessage? receivedMessage = null;
        
        var serverTask = Task.Run(async () => {
            var result = await _mockServer.ReceiveAsync();
            var jsonString = Encoding.UTF8.GetString(result.Buffer);
            receivedMessage = JsonSerializer.Deserialize<ServerMessage>(jsonString);
        });
        
        await Task.Delay(500); 
        
        _droneClient.SendLocationToOperator(locationCommand);
        
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
    public async Task ShouldReceivePendingMovementsFromServer()
    {
        for (var i = 0; i < 2; i++)
        {
            var movement = new Command(
                CommandType.Move, 
                new Vec3<int>(5, 10, 15)
            );
        
            var expectedMessage = new ServerMessage
            {
                DroneId = 1,
                Command = movement
            };
        
            var jsonString = JsonSerializer.Serialize(expectedMessage);
            var messageBytes = Encoding.UTF8.GetBytes(jsonString);
        
            await _mockServer.SendAsync(messageBytes, messageBytes.Length);
        }
        
        await Task.Delay(500);
        
        var pendingMovements = _droneClient.GetPendingMovements();
        
        Assert.That(pendingMovements, Has.Count.EqualTo(2));
        Assert.That(pendingMovements[0].X, Is.EqualTo(5));
    }
}