using System;
using System.Collections.Concurrent;
using System.Net;

namespace NetDroneServerLib.Repository;

public static class IpEndPointRepository
{
    private static readonly ConcurrentDictionary<int, IPEndPoint> _droneEndpoints = new();
    private static readonly ConcurrentDictionary<int, IPEndPoint> _operatorEndpoints = new();

    public static void RegisterDrone(int droneId, IPEndPoint endPoint)
    {
        _droneEndpoints[droneId] = endPoint;
    }

    public static void RegisterOperator(int operatorId, IPEndPoint endPoint)
    {
        _operatorEndpoints[operatorId] = endPoint;
    }

    public static bool TryGetDrone(int droneId, out IPEndPoint? endPoint)
    {
        return _droneEndpoints.TryGetValue(droneId, out endPoint);
    }

    public static bool TryGetOperator(int operatorId, out IPEndPoint? endPoint)
    {
        return _operatorEndpoints.TryGetValue(operatorId, out endPoint);
    }
}