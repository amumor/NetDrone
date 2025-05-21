using System;
using System.Text.Json.Serialization;

namespace NetDroneServerLib.Models;

public class Command
{
    [JsonPropertyName("cmd")] public CommandType Cmd { get; set; }
    [JsonPropertyName("data")] public Vec3 Data { get; set; } = new();
}
