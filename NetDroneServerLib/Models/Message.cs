using System.Text.Json.Serialization;

namespace NetDroneServerLib.Models;

/// <summary>
/// Represents a 3d vector, holding x, y and z coordinates.
/// </summary>
public class Vec3
{
    [JsonPropertyName("x")] public int X { get; set; }
    [JsonPropertyName("y")] public int Y { get; set; }
    [JsonPropertyName("z")] public int Z { get; set; }

    public override string ToString()
    {
        return $"{X},{Y},{Z}";
    }
}

/// <summary>
/// Enum describing the type of command being sent.
/// </summary>
public enum CommandType
{
    Move,
    State,
    Emergency,
    Register,
    Heartbeat
}

/// <summary>
/// Class holding the actual command data alongside the vector coordinates
/// </summary>
public class Command
{
    [JsonPropertyName("cmd")] public CommandType Cmd { get; set; }
    [JsonPropertyName("data")] public Vec3 Data { get; set; } = new();
}

/// <summary>
/// Class holding the a full message being sent.
/// </summary>
public class Message
{
    [JsonPropertyName("message_id")] public int MessageId { get; set; }
    [JsonPropertyName("drone_id")] public int DroneId { get; set; }
    [JsonPropertyName("operator_id")] public int OperatorId { get; set; }
    [JsonPropertyName("command")] public Command Command { get; set; } = new();
}