namespace NetDroneClient.assets.sprites;

using Godot;

public partial class Drone : Sprite2D
{
	private const float DroneDimension = 60.0f;
	private const float ViewportWidth = 1920.0f;
	private const float ViewportHeight = 1080.0f;
	private const int MovementSpeed = 2;
	
	public override void _Ready()
	{
		var sprite = this;
		var textureSize = sprite.Texture.GetSize();
		sprite.Scale = new Vector2(
			DroneDimension / textureSize.X, 
			DroneDimension / textureSize.Y
		);
	}
	
	public override void _Process(double delta)
	{
		if (Input.IsActionPressed("ui_right") && Position.X < ViewportWidth - DroneDimension / 2)
		{
			Position += new Vector2(MovementSpeed, 0);
		}
		
		if (Input.IsActionPressed("ui_left") && Position.X > DroneDimension / 2)
		{
			Position -= new Vector2(MovementSpeed, 0);
		}
		
		if (Input.IsActionPressed("ui_up") && Position.Y > DroneDimension / 2)
		{
			Position -= new Vector2(0, MovementSpeed);
		}
		
		if (Input.IsActionPressed("ui_down") && Position.Y < ViewportHeight - DroneDimension / 2)
		{
			Position += new Vector2(0, MovementSpeed);
		}
	}
}
