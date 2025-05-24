using NetDroneClientLib.Util;
using NetDroneServerLib.Models;

namespace NetDroneClient.Test;

public class InterpolatorTests
{
    private readonly Vec3 _start = new() { X = 0, Y = 0, Z = 0 };
    private readonly Vec3 _end = new() { X = 10, Y = 10, Z = 10 };
    private readonly Vec3 _expectedInterpolatedPoint = new() { X = 2, Y = 2, Z = 2 };
    private const int Steps = 4;
    
    [Test]
    public void ShouldInterpolateMovementQueueItemFromDroneClient()
    {
        var result = Interpolator.InterpolateDroneMovement(_start, _end, Steps);
        Assert.Multiple(() =>
        {
            Assert.That(Steps + 1, Is.EqualTo(result.Count));
            Assert.That(_start.X, Is.EqualTo(result[0].Position.X));
            Assert.That(_end.X, Is.EqualTo(result[^1].Position.X));
            Assert.That(result[^2].IsInterpolated);
            Assert.That(!result[^1].IsInterpolated);
            Assert.That(result[1].Position, Is.EqualTo(_expectedInterpolatedPoint));
        });
    }

    [Test]
    public void ShouldInterpolateVec3FromOperatorClient()
    {
        var result = Interpolator.InterpolateOperatorMovement(_start, _end, Steps);
        Assert.Multiple(() =>
        {
            Assert.That(Steps + 1, Is.EqualTo(result.Count));
            Assert.That(_start.X, Is.EqualTo(result[0].X));
            Assert.That(_end.X, Is.EqualTo(result[^1].X));
            Assert.That(result[1], Is.EqualTo(_expectedInterpolatedPoint));
        });
    }
}
