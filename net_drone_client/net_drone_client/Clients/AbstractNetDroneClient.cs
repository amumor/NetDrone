using net_drone_client.Communication;
using net_drone_client.Models;

namespace net_drone_client.Clients;

public abstract class AbstractNetDroneClient
{
    protected string Ip { get; set; }
    protected int Port { get; set; }
    protected DroneState DroneState { get; } = new();
    
    protected NetworkClient _networkClient;
    
    protected void SetupUdpConnection()
    {
        _networkClient = new NetworkClient(Ip, Port);
        HandleIncomingMessages();
        Console.WriteLine($"Connected to UDP at {Ip}:{Port}");
    }
    
    protected abstract void HandleIncomingMessages();
    
    public void Disconnect()
    {
        _networkClient.Close();
        Console.WriteLine("Disconneced from UDP");
    }
}