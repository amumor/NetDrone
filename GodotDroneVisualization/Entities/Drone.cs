
using System;
using System.Linq;

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
    private float _droneTickRate = 0.6f;      // 20ms for drone mode
    private float _operatorTickRate = 0.3f;   // 30ms for operator mode (more responsive)

    public override void _Ready()
    {
        GetWindow().MinSize = new Vector2I(1600, 800);

        var arguments = OS.GetCmdlineArgs();
        SetApplicationMode(arguments);
        GetWindow().Title = $"Mode: {Mode}";
        var label = GetParent().GetNode<Label>("ModeLabel");
        label.Text = $"Mode: {Mode}";

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
        _tickSystem.CreateTimer("operator_input", _operatorTickRate);
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
                if (_tickSystem.IsReady("operator_input"))
                {
                    RunOperatorMode();
                }
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void RunOperatorMode()
    {
        Vector2 newPosition;
        if (Input.IsActionPressed("ui_right") && Position.X < ViewportWidth - DroneDimension / 2)
        {
            newPosition = new Vector2(Position.X + MovementSpeed, Position.Y);
            _operatorService.MoveDrone(
                (int)newPosition.X,
                (int)newPosition.Y,
                0
            );
            Position = newPosition;
        }

        if (Input.IsActionPressed("ui_left") && Position.X > DroneDimension / 2)
        {
            newPosition = new Vector2(Position.X - MovementSpeed, Position.Y);
            _operatorService.MoveDrone(
                (int)newPosition.X,
                (int)newPosition.Y,
                0
            );
            Position = newPosition;
        }

        if (Input.IsActionPressed("ui_up") && Position.Y > DroneDimension / 2)
        {
            newPosition = new Vector2(Position.X, Position.Y - MovementSpeed);
            _operatorService.MoveDrone(
                (int)newPosition.X,
                (int)newPosition.Y,
                0
            );
            Position = newPosition;
        }

        if (Input.IsActionPressed("ui_down") && Position.Y < ViewportHeight - DroneDimension / 2)
        {
            newPosition = new Vector2(Position.X, Position.Y + MovementSpeed);
            _operatorService.MoveDrone(
                (int)newPosition.X,
                (int)newPosition.Y,
                0
            );
            Position = newPosition;
        }
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
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventKey keyEvent && keyEvent.Pressed && !keyEvent.Echo)
        {
            if (keyEvent.PhysicalKeycode == Key.I)
            {
                Console.WriteLine("Toggle Interpolation -----------------------------");
                _droneService.ToggleInterpolation();
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
        }
        else if (args.Contains("--drone"))
        {
            GD.Print("Drone mode activated");
            Mode = ApplicationMode.Drone;
            _droneService.Setup();
        }
    }
}
