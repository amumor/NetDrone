using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using NetDroneServerLib.Models;

namespace NetDroneServerLib.Utils;

public class UdpSender
{
    public static async Task ForwardToOperatorAsync(Message message, IPEndPoint operatorEP, UdpClient udpClient, CancellationToken token)
    {
        try
        {
            Console.WriteLine($"Drone id: {message.DroneId}");
            Console.WriteLine($"Drone EP: {operatorEP}");

            var json = JsonSerializer.Serialize(message);
            var bytes = Encoding.UTF8.GetBytes(json);
            await udpClient.SendAsync(bytes, bytes.Length, operatorEP);
            Console.WriteLine($"Forwarded to operator: {message.OperatorId} for drone {message.DroneId} at {operatorEP}");
        }
        catch
        {
            Console.WriteLine($"No endpoint for operator: {message.OperatorId} of drone {message.DroneId}");
        }
    }

    public static async Task ForwardToDroneAsync(Message message, IPEndPoint droneEP, UdpClient udpClient, CancellationToken token)
    {
        try
        {
            Console.WriteLine($"Drone id: {message.DroneId}");
            Console.WriteLine($"Drone EP: {droneEP}");

            var json = JsonSerializer.Serialize(message);
            var bytes = Encoding.UTF8.GetBytes(json);
            await udpClient.SendAsync(bytes, bytes.Length, droneEP);
            Console.WriteLine($"Forwarded to drone {message.DroneId} for operator: {message.OperatorId} at {droneEP}");
        }
        catch
        {
            Console.WriteLine($"No endpoint for drone: {message.OperatorId} with operator: {message.OperatorId}");
        }
    }
}
