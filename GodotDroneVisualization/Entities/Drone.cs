
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
	private float _droneTickRate = 0.6f;      // 1 second for drone mode
	private float _operatorTickRate = 0.3f;   // 100ms for operator mode (more responsive)
	
	public override void _Ready()
	{
		var arguments = OS.GetCmdlineArgs();
		SetApplicationMode(arguments);
		GetWindow().Title = $"Mode: {Mode}";
		
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
		if (Input.IsActionPressed("ui_right") && Position.X < ViewportWidth - DroneDimension / 2)
		{
			_operatorService.MoveDrone(MovementSpeed, 0, 0);
			Position += new Vector2(MovementSpeed, 0);
		}
		
		if (Input.IsActionPressed("ui_left") && Position.X > DroneDimension / 2)
		{
			_operatorService.MoveDrone(-MovementSpeed, 0, 0);
			Position -= new Vector2(MovementSpeed, 0);
		}
		
		if (Input.IsActionPressed("ui_up") && Position.Y > DroneDimension / 2)
		{
			_operatorService.MoveDrone(0, -MovementSpeed, 0);
			Position -= new Vector2(0, MovementSpeed);
		}
		
		if (Input.IsActionPressed("ui_down") && Position.Y < ViewportHeight - DroneDimension / 2)
		{
			_operatorService.MoveDrone(0, MovementSpeed, 0);
			Position += new Vector2(0, MovementSpeed);
		}
	}

	private void RunDroneMode()
	{
		var pendingMovements = _droneService.GetPendingMovements();
		foreach (var movement in pendingMovements)
		{
			Position += new Vector2(movement.X, movement.Y);
		}
		_droneService.UpdateDronePosition();
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
