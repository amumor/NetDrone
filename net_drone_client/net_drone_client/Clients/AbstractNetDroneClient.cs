using net_drone_client.Communication;
using net_drone_client.Models;

namespace net_drone_client.Clients;

public abstract class AbstractNetDroneClient
{
    public string Ip { get; set; }
    public int Port { get; set; }
    public DroneState DroneState { get; set; } = new();
    
    public void Initialize()
    {
        NetworkClient.Initialize(Ip, Port);
        HandleIncomingMessages();
        Console.WriteLine($"Connecting to drone at {Ip}:{Port}");
    }
    
    protected abstract void HandleIncomingMessages();

    
    public void Disconnect()
    {
        NetworkClient.Close();
        Console.WriteLine("Disconneced from drone");
    }
}