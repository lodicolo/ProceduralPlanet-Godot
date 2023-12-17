using Godot;
using System;

public partial class FreeLookCamera3D : Camera3D
{
	[Export(PropertyHint.Range, "0,10,0.01")] public float Sensitivity { get; set; } = 3;

	[Export(PropertyHint.Range, "0,1000,0.1")]
	public float DefaultSpeed { get; set; } = 10;

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

			case InputEventKey keyEvent:
			{
				switch (keyEvent.Keycode)
				{
					case Key.Q:
					case Key.E:
					case Key.Shift:
					case Key.Ctrl:
					case Key.W:
					case Key.A:
					case Key.S:
					case Key.D:
					case Key.Up:
					case Key.Left:
					case Key.Down:
					case Key.Right:
					{
						int x = 0, y = 0, z = 0;

						if (Input.IsPhysicalKeyPressed(Key.W) || Input.IsPhysicalKeyPressed(Key.Up))
						{
							z -= 1;
						}

						if (Input.IsPhysicalKeyPressed(Key.S) || Input.IsPhysicalKeyPressed(Key.Down))
						{
							z += 1;
						}

						if (Input.IsPhysicalKeyPressed(Key.A) || Input.IsPhysicalKeyPressed(Key.Left))
						{
							x -= 1;
						}

						if (Input.IsPhysicalKeyPressed(Key.D) || Input.IsPhysicalKeyPressed(Key.Right))
						{
							x += 1;
						}

						if (Input.IsPhysicalKeyPressed(Key.Q))
						{
							y -= 1;
						}

						if (Input.IsPhysicalKeyPressed(Key.E))
						{
							y += 1;
						}

						_inputDirection = new Vector3(x, y, z);
						_sprint = Input.IsPhysicalKeyPressed(Key.Shift);
						break;
					}
				}
				break;
			}
		}

		Console.WriteLine($"{Name} [{GetType().Name}] {nameof(_Input)}: {inputEvent.GetType().Name}");
	}
}
