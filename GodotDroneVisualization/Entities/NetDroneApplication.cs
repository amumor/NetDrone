using System;
using System.Linq;
using NetDroneClientLib.Clients;
using NetDroneServerLib.Models;
using Godot;
using GodotDroneVisualization.Utils;

namespace GodotDroneVisualization.Entities;

public partial class NetDroneApplication : Sprite2D
{
    public ApplicationMode Mode { get; private set; }
    private DroneClient _droneClient;
    private OperatorClient _operatorClient;
    private const float DroneDimension = 50.0f;
    private const float ViewportWidth = 1600.0f;
    private const float ViewportHeight = 800.0f;
    private const int MovementSpeed = 25;

    // Refresh system
    private readonly RefreshCycle _refreshCycle = new();

    // Configurable refresh rates
    private float _droneTickRate = 0.01f; 
    private float _operatorSendTickRate = 0.1f;
    private float _operatorReceiveTickRate = 0.01f;

    private void SetApplicationMode(string[] args)
    {
        if (args.Contains("--operator"))
        {
            GD.Print("Operator mode activated");
            Mode = ApplicationMode.Operator;
            
            _operatorClient = new OperatorClient(
                clientPort: 5002,
                serverPort: 4002,
                serverIp: "127.0.0.1",
                droneId: 1,
                operatorId: 1,
                interpolationRate: 6
            );
            
            _operatorClient.DroneState.Position = new Vec3
            {
                X = (int)Position.X,
                Y = (int)Position.Y,
                Z = 0
            };
        }
        else if (args.Contains("--drone"))
        {
            GD.Print("Drone mode activated");
            Mode = ApplicationMode.Drone;
            
            _droneClient = new DroneClient(
                clientPort: 5001,
                serverPort: 4001,
                serverIp: "127.0.0.1",
                droneId: 1,
                operatorId: 1,
                interpolationRate: 6
            );
        }
    }
    
    public override void _Ready()
    {
        GetWindow().MinSize = new Vector2I(1600, 800);

        var arguments = OS.GetCmdlineArgs();
        SetApplicationMode(arguments);
        GetWindow().Title = $"Mode: {Mode}";
        var label = GetParent().GetNode<Label>("ModeLabel");
        label.Text = $"Mode: {Mode}, Press 'I' to toggle interpolation, press 'R' to toggle reconciliation";

        Position = new Vector2(
            ViewportWidth / 2,
            ViewportHeight / 2
        );

        var sprite = this;
        var textureSize = sprite.Texture.GetSize();
        sprite.Scale = new Vector2(
            DroneDimension / textureSize.X,
            DroneDimension / textureSize.Y
        );

        SetupTickSystem();
    }

    private void SetupTickSystem()
    {
        _refreshCycle.CreateTimer("drone_update", _droneTickRate);
        _refreshCycle.CreateTimer("operator_send", _operatorSendTickRate);
        _refreshCycle.CreateTimer("operator_receive", _operatorReceiveTickRate);
    }

    public override void _Process(double delta)
    {
        _refreshCycle.Update((float)delta);

        switch (Mode)
        {
            case ApplicationMode.Drone:
                if (_refreshCycle.IsReady("drone_update"))
                {
                    RunDroneMode();
                }
                break;

            case ApplicationMode.Operator:
                if (_refreshCycle.IsReady("operator_send"))
                {
                    RunOperatorSend();
                }
                if (_refreshCycle.IsReady("operator_receive"))
                {
                    RunOperatorUpdate();
                }
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }
    } 

// OPERATOR MODE -------------------------------------------------------------------------------------------------------
    private void RunOperatorSend()
    {
        var newX = (int)Position.X;
        var newY = (int)Position.Y;

        if (Input.IsActionPressed("ui_right") && newX < ViewportWidth - DroneDimension / 2)
            newX += MovementSpeed;
        if (Input.IsActionPressed("ui_left") && newX > DroneDimension / 2)
            newX -= MovementSpeed;
        if (Input.IsActionPressed("ui_up") && newY > DroneDimension / 2)
            newY -= MovementSpeed;
        if (Input.IsActionPressed("ui_down") && newY < ViewportHeight - DroneDimension / 2)
            newY += MovementSpeed;

        // Only send if position changed
        if (newX != (int)Position.X || newY != (int)Position.Y)
        {
            MoveDrone(newX, newY, 0);
        }
    }

    private void RunOperatorUpdate()
    {
        var nextMovement = _operatorClient.GetNextMovement();
        if (nextMovement == null)
        {
            return;
        }
        Position = new Vector2(nextMovement.X, nextMovement.Y);
        Console.WriteLine($"Current position: {Position}");
    }
    
    private void MoveDrone(int x, int y, int z)
    {
        var pos = new Vec3 { X = x, Y = y, Z = z };
        _operatorClient.DroneState.Position = pos;
        
        var queue = _operatorClient.MovementQueue;
        if (queue.Movements.Count == 0 || !pos.Equals(queue.LastMovement))
        {
            queue.Movements.Clear(); 
            queue.AddMovement(pos);
        }

        var command = new Command
        {
            Cmd = CommandType.Move,
            Data = new Vec3 { X = x, Y = y, Z = z }
        };
        _operatorClient.SendCommandToDrone(command);
    }
    
// DRONE MODE ----------------------------------------------------------------------------------------------------------

    private void RunDroneMode()
    {
        var locationMessage = _droneClient.GetNextMovement();
        if (locationMessage == null)
        {
            return;
        }
        var movement = locationMessage.Position;
        if (movement.X != 0 || movement.Y != 0)
        {
            Position = new Vector2(movement.X, movement.Y);
            if (_droneClient != null)
            {
                _droneClient.DroneState.Position = new Vec3
                {
                    X = (int)Position.X,
                    Y = (int)Position.Y,
                    Z = 0
                };
            }
        }
    }
    
// Interpolation/Reconciliation toggling -------------------------------------------------------------------------------
    public override void _Input(InputEvent @event)
    {
        if (@event is not InputEventKey { Pressed: true, Echo: false } keyEvent)
            return;

        switch (keyEvent.PhysicalKeycode)
        {
            case Key.I:
                Console.WriteLine("Toggle Interpolation -----------------------------");
                switch (Mode)
                {
                    case ApplicationMode.Drone:
                        _droneClient.SetMovementInterpolation(!_droneClient.MovementQueue.ShouldInterpolate);
                        break;
                    case ApplicationMode.Operator:
                        _operatorClient.SetMovementInterpolation(!_operatorClient.MovementQueue.ShouldInterpolate);
                        break;
                }
                break;
            case Key.R:
                Console.WriteLine("R key pressed");
                if (Mode == ApplicationMode.Operator)
                {
                    _operatorClient.SetReconciliation(!_operatorClient.ShouldReconcile);
                }
                break;
        }
    }
}
