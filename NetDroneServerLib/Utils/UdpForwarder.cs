using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using NetDroneServerLib.Models;

namespace NetDroneServerLib.Utils;

/// <summary>
/// Class holding static helper methods for sending messages over udp.
/// </summary>
public class UdpSender
{
    /// <summary>
    /// Helper method tha forwards a message to the given IPEndpoint.
    /// </summary>
    /// <param name="message">Message</param>
    /// <param name="operatorEP">Operator IPEndPint</param>
    /// <param name="udpClient">UDP Client</param>
    /// <param name="token">Cancellation token</param>
    /// <returns></returns>
    public static async Task ForwardToOperatorAsync(Message message, IPEndPoint operatorEP, UdpClient udpClient)
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

    /// <summary>
    /// Helper method that forwards a message to the given Drone IPEndpoint
    /// </summary>
    /// <param name="message">Message</param>
    /// <param name="droneEP">Drone IPEndPoint</param>
    /// <param name="udpClient">UDP Client</param>
    /// <param name="token">Cancellation token</param>
    /// <returns></returns>
    public static async Task ForwardToDroneAsync(Message message, IPEndPoint droneEP, UdpClient udpClient)
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
