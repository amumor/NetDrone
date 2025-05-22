using System.Text.Json.Serialization;

namespace NetDroneServerLib.Models;

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
    [JsonPropertyName("message_id")] public int MessageId { get; set; }
    [JsonPropertyName("drone_id")] public int DroneId { get; set; }
    [JsonPropertyName("operator_id")] public int OperatorId { get; set; }
    [JsonPropertyName("command")] public Command Command { get; set; } = new();
}