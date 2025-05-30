﻿using NetDroneServerLib;

internal class Program
{
    private static async Task Main(string[] args)
    {
        int dronePort = 4001;
        int clientPort = 4002;

        var server = new NetDroneServer();
        var cts = new CancellationTokenSource();
        System.Console.WriteLine($"🚀 Starting NetDrone server...");
        await server.StartListenersAsync(dronePort, clientPort, cts.Token);
    }
}