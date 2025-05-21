using System.Text.Json.Serialization;
using NetDroneServerLib.Models;

namespace DroneServerCore.Models;

public class Message
{
    [JsonPropertyName("drone_id")] public int DroneId { get; set; }
    [JsonPropertyName("command")] public Command Command { get; set; } = new();
}