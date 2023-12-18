using System;
using Godot;

namespace ProceduralPlanet.scripts.camera;

public partial class FreeLookCamera3D : Camera3D
{
	[Export(PropertyHint.Range, "0,10,0.01")] public float Sensitivity { get; set; } = 3;

	[Export(PropertyHint.Range, "0,1000,0.1")]
	public float DefaultSpeed { get; set; } = 100;

	[Export(PropertyHint.Range, "0,10,0.01")]
	public float SpeedScale { get; set; } = 1.17f;

	[Export] public float MaxSpeed { get; set; } = 1000;

	[Export] public float MinSpeed { get; set; } = 0.2f;

	private float _speed;
	private Vector3 _inputDirection;
	private bool _sprint;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_speed = DefaultSpeed;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		var deltaPosition = _inputDirection.Normalized() * _speed * (float)delta;
		if (_sprint)
		{
			deltaPosition *= 2;
		}
		Translate(deltaPosition);
	}

	public override void _Input(InputEvent inputEvent) {
		if (!Current)
		{
			return;
		}

		if (Input.MouseMode == Input.MouseModeEnum.Captured)
		{
			if (inputEvent is InputEventMouseMotion mouseMotion)
			{
				var rotation = Rotation;
				rotation.Y -= mouseMotion.Relative.X / 1000 * Sensitivity;
				rotation.X -= mouseMotion.Relative.Y / 1000 * Sensitivity;
				rotation.X = Math.Clamp(rotation.X, MathF.PI / -2, MathF.PI / 2);
				Rotation = rotation;
			}
		}

		switch (inputEvent)
		{
			case InputEventMouseButton mouseButton:
				switch (mouseButton.ButtonIndex)
				{
					case MouseButton.Right:
						Input.MouseMode = Input.MouseModeEnum.Visible;
						break;
					case MouseButton.Left:
						Input.MouseMode = Input.MouseModeEnum.Captured;
						break;
					case MouseButton.WheelDown:
						_speed = Math.Clamp(_speed / SpeedScale, MinSpeed, MaxSpeed);
						break;
					case MouseButton.WheelUp:
						_speed = Math.Clamp(_speed * SpeedScale, MinSpeed, MaxSpeed);
						break;
				}
				break;
		}

		_inputDirection = new Vector3(
			Input.GetAxis("move_left", "move_right"),
			Input.GetAxis("move_down", "move_up"),
			Input.GetAxis("move_forward", "move_backward")
		);
		_sprint = Input.IsActionPressed("move_fast");

		Console.WriteLine($"{Name} [{GetType().Name}] {nameof(_Input)}: {inputEvent.GetType().Name}");
	}
}