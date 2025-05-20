using System.Text.Json.Serialization;

namespace DroneServerCore.Models;

public class Vec3
{
  [JsonPropertyName("x")] public int X { get; set; }
  [JsonPropertyName("y")] public int Y { get; set; }
  [JsonPropertyName("z")] public int Z { get; set; }
}

public enum CommandType
{
  Move,
  State,
  Emergency,
  Register,
  Heartbeat
}

public class Command
{
  [JsonPropertyName("cmd")] public CommandType Cmd { get; set; }
  [JsonPropertyName("data")] public Vec3 Data { get; set; } = new();
}

public class Message
{
  [JsonPropertyName("drone_id")] public int DroneId { get; set; }
  [JsonPropertyName("command")] public Command Command { get; set; } = new();
}