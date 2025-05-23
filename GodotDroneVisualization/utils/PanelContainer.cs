using Godot;
using GodotDroneVisualization.Entities;

namespace GodotDroneVisualization.Entities;

public partial class InfoPanel : Godot.PanelContainer
{
    private Label _statusLabel;
    private Label _positionLabel;
    
    // Called when the node enters the scene tree for the first time
    public override void _Ready()
    {
        // Create a vertical container
        var vbox = new VBoxContainer();
        
        var drone = GetNode<NetDroneApplication>("/Drone");
        var titleLabel = new Label { Text = drone.Mode.ToString() };
        
        // Add labels to container
        vbox.AddChild(titleLabel);
        
        // Add container to panel
        AddChild(vbox);
    }

}
