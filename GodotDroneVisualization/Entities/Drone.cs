
using System;
using System.Linq;

namespace GodotDroneVisualization.Entities;

using Godot;

public partial class Drone : Sprite2D
{
	public ApplicationMode Mode { get; private set; }
	private readonly DroneService _droneService = new();
	private readonly OperatorService _operatorService = new();
	private const float DroneDimension = 60.0f;
	private const float ViewportWidth = 1920.0f;
	private const float ViewportHeight = 1080.0f;
	private const int MovementSpeed = 5;
	private float _processTimer;
	
	public override void _Ready()
	{
		var arguments = OS.GetCmdlineArgs();
		SetApplicationMode(arguments);
		
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
	}
	
	public override void _Process(double delta)
	{
		_processTimer += (float)delta;
		if (_processTimer >= 1f)
		{
			_processTimer = 0f;
			switch (Mode)
			{
				case ApplicationMode.Drone:
					RunDroneMode();
					break;
				case ApplicationMode.Operator:
					RunOperatorMode();
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
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
