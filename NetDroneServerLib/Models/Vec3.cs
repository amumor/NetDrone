using System;
using System.Text.Json.Serialization;

namespace NetDroneServerLib.Models;

public class Vec3
{
    [JsonPropertyName("x")] public int X { get; set; }
    [JsonPropertyName("y")] public int Y { get; set; }
    [JsonPropertyName("z")] public int Z { get; set; }
}