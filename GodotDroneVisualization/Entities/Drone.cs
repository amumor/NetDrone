
using System;
using System.Linq;
using NetDroneServerLib.Models;

namespace GodotDroneVisualization.Entities;

using Godot;

public partial class Drone : Sprite2D
{
    public ApplicationMode Mode { get; private set; }
    private readonly DroneService _droneService = new();
    private readonly OperatorService _operatorService = new();
    private const float DroneDimension = 50.0f;
    private const float ViewportWidth = 1600.0f;
    private const float ViewportHeight = 800.0f;
    private const int MovementSpeed = 25;

    // Tick system
    private readonly TickSystem _tickSystem = new();

    // Configurable tick rates
    private float _droneTickRate = 0.01f;
    private float _operatorSendTickRate = 0.1f;
    private float _operatorReceiveTickRate = 0.01f;

    public override void _Ready()
    {
        GetWindow().MinSize = new Vector2I(1600, 800);

        var arguments = OS.GetCmdlineArgs();
        SetApplicationMode(arguments);
        GetWindow().Title = $"Mode: {Mode}";
        var label = GetParent().GetNode<Label>("ModeLabel");
        label.Text = $"Mode: {Mode}\nPress 'I' to toggle interpolation";

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
        _tickSystem.CreateTimer("drone_update", _droneTickRate);
        _tickSystem.CreateTimer("operator_send", _operatorSendTickRate);
        _tickSystem.CreateTimer("operator_receive", _operatorReceiveTickRate);
    }

    public override void _Process(double delta)
    {
        _tickSystem.Update((float)delta);

        switch (Mode)
        {
            case ApplicationMode.Drone:
                if (_tickSystem.IsReady("drone_update"))
                {
                    RunDroneMode();
                }
                break;

            case ApplicationMode.Operator:
                if (_tickSystem.IsReady("operator_send"))
                {
                    RunOperatorSend();
                }
                if (_tickSystem.IsReady("operator_receive"))
                {
                    RunOperatorUpdate();
                }
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void RunOperatorSend()
    {
        int newX = (int)Position.X;
        int newY = (int)Position.Y;

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
            _operatorService.MoveDrone(newX, newY, 0);
        }
    }

    private void RunOperatorUpdate()
    {
        var nextMovement = _operatorService.GetNextMovement();
        if (nextMovement == null)
        {
            return;
        }
        Position = new Vector2(nextMovement.X, nextMovement.Y);
        Console.WriteLine($"Current position: {Position}");
    }

    private void RunDroneMode()
    {
        var locationMessage = _droneService.GetNextMovement();
        if (locationMessage == null)
        {
            return;
        }
        var movement = locationMessage.Position;
        // Only update if movement is not zero
        if (movement.X != 0 || movement.Y != 0)
        {
            Position = new Vector2(movement.X, movement.Y);
            // Update the internal drone state as well
            if (_droneService.DroneClient != null)
            {
                _droneService.DroneClient.DroneState.Position = new Vec3
                {
                    X = (int)Position.X,
                    Y = (int)Position.Y,
                    Z = 0
                };
            }
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventKey keyEvent && keyEvent.Pressed && !keyEvent.Echo)
        {
            if (keyEvent.PhysicalKeycode == Key.I)
            {
                Console.WriteLine("Toggle Interpolation -----------------------------");
                switch (Mode)
                {
                    case ApplicationMode.Drone:
                        _droneService.ToggleInterpolation();
                        break;
                    case ApplicationMode.Operator:
                        _operatorService.ToggleInterpolation();
                        break;
                }
            }
        }
    }

    private void SetApplicationMode(string[] args)
    {
        if (args.Contains("--operator"))
        {
            GD.Print("Operator mode activated");
            Mode = ApplicationMode.Operator;
            _operatorService.Setup();
            _operatorService.OperatorClient.DroneState.Position = new Vec3
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
            _droneService.Setup();
        }
    }
}
