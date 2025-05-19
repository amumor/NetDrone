using System.Text.Json.Serialization;
using System.Runtime.Serialization;

// Simple 3D vector
public class Vec3<T>
{
    [JsonPropertyName("x")]
    public T X { get; set; }

    [JsonPropertyName("y")]
    public T Y { get; set; }

    [JsonPropertyName("z")]
    public T Z { get; set; }
}

// Enum for all valid client-side commands
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum CommandType
{
    [EnumMember(Value = "move")]
    Move
}

public class Command
{
    [JsonPropertyName("cmd")]
    public CommandType Cmd { get; set; }

    [JsonPropertyName("data")]
    public Vec3<int> Data { get; set; }
}

// Message sent from client to server
public class ClientMessage
{
    [JsonPropertyName("type")]
    public string Type { get; set; } // "command"

    [JsonPropertyName("drone_id")]
    public string DroneId { get; set; }

    [JsonPropertyName("command")]
    public Command Command { get; set; }
}

// Message sent from server to client
public class ServerMessage
{
    [JsonPropertyName("type")]
    public string Type { get; set; } // "state"

    [JsonPropertyName("drone_id")]
    public string DroneId { get; set; }

    [JsonPropertyName("x")]
    public float X { get; set; }

    [JsonPropertyName("y")]
    public float Y { get; set; }

    [JsonPropertyName("z")]
    public float Z { get; set; }
}