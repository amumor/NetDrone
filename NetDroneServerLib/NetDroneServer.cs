using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using NetDroneServerLib.Services;
using NetDroneServerLib.Controllers;
using Microsoft.Extensions.Logging;

namespace NetDroneServerLib;

public class NetDroneServer
{
    private readonly DroneService _droneService = new();
    private readonly OperatorService _operatorService = new();
    private readonly DroneController _droneController;
    private readonly OperatorController _operatorController;

    public NetDroneServer()
    {
        _droneController = new DroneController(_droneService);
        _operatorController = new OperatorController(_operatorService);
    }

    public async Task StartListenersAsync(int dronePort, int operatorPort, CancellationToken token)
    {
        var droneTask = ListenAsync(dronePort, _droneController, token);
        var operatorTask = ListenAsync(operatorPort, _operatorController, token);

        await Task.WhenAll(droneTask, operatorTask);
    }

    private async Task ListenAsync(int port, IMessageController controller, CancellationToken token)
    {
        using var udpClient = new UdpClient(port);
        while (!token.IsCancellationRequested)
        {
            var result = await udpClient.ReceiveAsync();
            var json = System.Text.Encoding.UTF8.GetString(result.Buffer);
            await controller.HandleMessageAsync(json, result.RemoteEndPoint, udpClient, token);
        }
    }
}