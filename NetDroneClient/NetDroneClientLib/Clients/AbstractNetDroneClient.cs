

using NetDroneClientLib.Communication;
using NetDroneClientLib.Models;
using NetDroneServerLib.Models;

namespace NetDroneClientLib.Clients;

public abstract class AbstractNetDroneClient
{
    protected int ClientPort { get; set; }
    protected int ServerPort { get; set; }
    protected string ServerIp { get; set; }
    public DroneState DroneState { get; set; }

    protected NetworkClient _networkClient;

    protected void SetupUdpConnection()
    {
        _networkClient = new NetworkClient(ClientPort, ServerPort, ServerIp);
        RegisterClientToServer();
        HandleIncomingMessages();
    }

    private void RegisterClientToServer()
    {
        var command = new Command
        {
            Cmd = CommandType.Register,
            Data = null
        };
        _networkClient.SendCommand(0, command, DroneState.Id, DroneState.OperatorId);
    }

    protected abstract void HandleIncomingMessages();

    public void Disconnect()
    {
        _networkClient.Close();
    }
}