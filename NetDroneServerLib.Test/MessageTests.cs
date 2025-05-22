using System.Text.Json;
using NetDroneServerLib.Models;

namespace NetDroneServerLib.Test;

public class MessageTests
{
    [Test]
    public void Message_Serializes_And_Deserializes_Correctly()
    {
        var message = new Message
        {
            MessageId = 42,
            DroneId = 1,
            OperatorId = 2,
            Command = new Command
            {
                Cmd = CommandType.Move,
                Data = new Vec3 { X = 1, Y = 2, Z = 3 }
            }
        };

        var json = JsonSerializer.Serialize(message);
        var deserialized = JsonSerializer.Deserialize<Message>(json);

        Assert.That(deserialized is not null);
        Assert.That(deserialized.MessageId, Is.EqualTo(message.MessageId));
        Assert.That(deserialized.DroneId, Is.EqualTo(message.DroneId));
        Assert.That(deserialized.OperatorId, Is.EqualTo(message.OperatorId));
        Assert.That(deserialized.Command.Cmd, Is.EqualTo(message.Command.Cmd));
        Assert.That(deserialized.Command.Data.X, Is.EqualTo(message.Command.Data.X));
        Assert.That(deserialized.Command.Data.Y, Is.EqualTo(message.Command.Data.Y));
        Assert.That(deserialized.Command.Data.Z, Is.EqualTo(message.Command.Data.Z));
    }
}