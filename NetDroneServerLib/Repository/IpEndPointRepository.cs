using System;
using System.Collections.Concurrent;
using System.Net;

namespace NetDroneServerLib.Repository;

/// <summary>
/// Static class holding two Dictionaries containing operator and drone endpoints. 
/// </summary>
public static class IpEndPointRepository
{
    private static readonly ConcurrentDictionary<int, IPEndPoint> _droneEndpoints = new();
    private static readonly ConcurrentDictionary<int, IPEndPoint> _operatorEndpoints = new();

    /// <summary>
    /// Register or update operator registry.
    /// </summary>
    /// <param name="droneId"></param>
    /// <param name="endPoint"></param>
    public static void RegisterDrone(int droneId, IPEndPoint endPoint)
    {
        _droneEndpoints[droneId] = endPoint;
    }


    /// <summary>
    /// Register or update operator registry.
    /// </summary>
    /// <param name="operatorId">Operator ID</param>
    /// <param name="endPoint">IPEndpoint output</param>
    public static void RegisterOperator(int operatorId, IPEndPoint endPoint)
    {
        _operatorEndpoints[operatorId] = endPoint;
    }

    /// <summary>
    /// Try getter for drone endpoints.
    /// </summary>
    /// <param name="droneId">Drone ID</param>
    /// <param name="endPoint">IPEndpoint output</param>
    /// <returns></returns>
    public static bool TryGetDrone(int droneId, out IPEndPoint? endPoint)
    {
        return _droneEndpoints.TryGetValue(droneId, out endPoint);
    }
    /// <summary>
    /// Try getter for operator endpoints.
    /// </summary>
    /// <param name="operatorId">Operator ID</param>
    /// <param name="endPoint">IPEndpoint output</param>
    /// <returns></returns>
    public static bool TryGetOperator(int operatorId, out IPEndPoint? endPoint)
    {
        return _operatorEndpoints.TryGetValue(operatorId, out endPoint);
    }
}