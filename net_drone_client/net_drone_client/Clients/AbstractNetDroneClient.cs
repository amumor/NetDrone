using net_drone_client.Communication;
using net_drone_client.Models;

namespace net_drone_client.Clients;

public abstract class AbstractNetDroneClient
{
    protected string Ip { get; set; }
    protected int Port { get; set; }
    protected DroneState DroneState { get; set; }
    
    protected NetworkClient _networkClient;
    
    protected void SetupUdpConnection()
    {
        _networkClient = new NetworkClient(Ip, Port);
        HandleIncomingMessages();
    }
    
    protected abstract void HandleIncomingMessages();
    
    public void Disconnect()
    {
        _networkClient.Close();
        Console.WriteLine("Disconnected from UDP");
    }
}