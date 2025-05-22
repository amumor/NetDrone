using System.Net;
using NetDroneServerLib.Repository;
using NUnit.Framework;

namespace NetDroneServerLib.Test;

public class IpEndPointRepositoryTests
{
    [Test]
    public void RegisterAndRetrieveDroneEndpoint_Works()
    {
        var ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1234);
        IpEndPointRepository.RegisterDrone(1, ep);

        var found = IpEndPointRepository.TryGetDrone(1, out var result);

        Assert.That(found);
        Assert.That(result, Is.EqualTo(ep));
    }

    [Test]
    public void RegisterAndRetrieveOperatorEndpoint_Works()
    {
        var ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 4321);
        IpEndPointRepository.RegisterOperator(2, ep);

        var found = IpEndPointRepository.TryGetOperator(2, out var result);

        Assert.That(found);
        Assert.That(ep, Is.EqualTo(result));
    }
}