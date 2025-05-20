namespace net_drone_client.test;

using Moq;
using Clients;
using Communication;
using Models;
using NUnit.Framework;

[TestFixture]
public class DroneClientTests
{
    private Mock<NetworkClient> _mockNetworkClient;
    private DroneClient _droneClient;

    [SetUp]
    public void Setup()
    {
        _mockNetworkClient = new Mock<NetworkClient>("127.0.0.1", 8888);
        _droneClient = new DroneClient("127.0.0.1", 8888);
    }

    [Test]
    public void Should_SendLocationToOperator()
    {
        // Arrange
        var command = new Command(CommandType.Move, new Vec3<int>(10, 20, 30));

        // Act
        _droneClient.SendLocationToOperator(command);

        // Assert
        _mockNetworkClient.Verify(nc => nc.SendCommand(command), Times.Once);
    }
/*
    [Test]
    public void Should_HandleIncomingMessages()
    {
        // Arrange
        var serverMessage = new ServerMessage
        {
            X = 10,
            Y = 20,
            Z = 30,
            Type = "state",
            DroneId = "drone_1"
        };

        _mockNetworkClient
            .Setup(nc => nc.OnMessageReceived += It.IsAny<Action<ServerMessage>>())
            .Callback<Action<ServerMessage>>(callback => callback(serverMessage));

        // Act
        _droneClient.SetupUdpConnection();

        // Assert
        var pendingMovements = _droneClient.GetPendingMovements();
        Assert.AreEqual(1, pendingMovements.Count);
        Assert.AreEqual(10, pendingMovements[0].X);
        Assert.AreEqual(20, pendingMovements[0].Y);
        Assert.AreEqual(30, pendingMovements[0].Z);
    }
*/
}