using net_drone_client.Communication;
using net_drone_client.Models;

namespace net_drone_client.Clients;

public abstract class AbstractNetDroneClient
{
    protected int ClientPort { get; set; }
    protected int ServerPort { get; set; }
    protected string ServerIp { get; set; }
    protected DroneState DroneState { get; set; }
    
    protected NetworkClient _networkClient;
    
    protected void SetupUdpConnection()
    {
        _networkClient = new NetworkClient(ClientPort, ServerPort, ServerIp);
        RegisterClientToServer();
        HandleIncomingMessages();
    }
    
    private void RegisterClientToServer()
    {
        var command = new Command(

            CommandType.Register,
            null
        );
        _networkClient.SendCommand(command, DroneState.Id);
    }
    
    protected abstract void HandleIncomingMessages();
    
    public void Disconnect()
    {
        _networkClient.Close();
    }
}