namespace NetDroneClientLib.Models;

using System.Text.Json.Serialization;
using System.Runtime.Serialization;

public class Vec3<T>
{
    [JsonPropertyName("x")]
    public T X { get; }

    [JsonPropertyName("y")]
    public T Y { get; }

    [JsonPropertyName("z")]
    public T Z { get; }

    public Vec3(T x, T y, T z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public override string ToString()
    {
        return $"({X}, {Y}, {Z})";
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
    [JsonPropertyName("cmd")]
    public CommandType Cmd { get; set; }

    [JsonPropertyName("data")]
    public Vec3<int> Data { get; set; }

    public Command(CommandType cmd, Vec3<int> data)
    {
        Cmd = cmd;
        Data = data;
    }
}

/* Message sent from client to server
public class ClientMessage
{
    [JsonPropertyName("type")]
    public string Type { get; set; } // "command"

    [JsonPropertyName("drone_id")]
    public string DroneId { get; set; }

    [JsonPropertyName("command")]
    public Command Command { get; set; }
}
*/

// Message sent from server to client
public class ServerMessage
{
    [JsonPropertyName("drone_id")]
    public int DroneId { get; set; }

    [JsonPropertyName("command")]
    public Command Command { get; set; }
}