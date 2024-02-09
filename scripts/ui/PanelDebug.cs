using System;
using Godot;
using ProceduralPlanet.scripts.planet;
using ProceduralPlanet.scripts.planet.shape.modules;
using ProceduralPlanet.scripts.stars;

[Tool]
public partial class PanelDebug : Panel
{
	private SpinBox? _positionX;
	private SpinBox? _positionY;
	private SpinBox? _positionZ;
	private SpinBox? _rotationX;
	private SpinBox? _rotationY;
	private SpinBox? _rotationZ;
	private HSlider? _atmosphereStrength;
	private HSlider? _mountainHeight;
	private HSlider? _oceanDepth;
	private HSlider? _starCount;
	private CheckButton? _atmosphereEnabled; 
	private CheckButton? _oceanEnabled;
	private CheckButton? _starsEnabled;

	[Export] public Camera3D? Camera { get; set; }
	[Export] public CelestialBodyGenerator? Generator { get; set; }
	[Export] public Stars? Stars { get; set; }

	private void UpdateOptionsFromCamera()
	{
		if (Camera is not { } camera)
		{
			return;
		}

		if (_positionX is { } positionX)
		{
			positionX.Value = camera.Position.X;
		}

		if (_positionY is { } positionY)
		{
			positionY.Value = camera.Position.Y;
		}

		if (_positionZ is { } positionZ)
		{
			positionZ.Value = camera.Position.Z;
		}

		if (_rotationX is { } rotationX)
		{
			rotationX.Value = camera.Rotation.X * 180f / MathF.PI;
		}

		if (_rotationY is { } rotationY)
		{
			rotationY.Value = camera.Rotation.Y * 180f / MathF.PI;
		}

		if (_rotationZ is { } rotationZ)
		{
			rotationZ.Value = camera.Rotation.Z * 180f / MathF.PI;
		}
	}

	private void UpdateCameraFromOptions()
	{
		if (Camera is not { } camera)
		{
			return;
		}

		camera.GlobalPosition = new Vector3(
			(float)_positionX!.Value,
			(float)_positionY!.Value,
			(float)_positionZ!.Value
		);

		camera.Rotation = new Vector3(
			(float)_rotationX!.Value * MathF.PI / 180f,
			(float)_rotationY!.Value * MathF.PI / 180f,
			(float)_rotationZ!.Value * MathF.PI / 180f
		);
	}

	public override void _Ready()
	{
		base._Ready();

		_positionX = GetNode<SpinBox>("GridDebug/PositionComponents/ComponentX") ??
					throw new InvalidOperationException("Missing GridDebug/PositionComponents/ComponentX");
		_positionY = GetNode<SpinBox>("GridDebug/PositionComponents/ComponentY") ??
					throw new InvalidOperationException("Missing GridDebug/PositionComponents/ComponentY");
		_positionZ = GetNode<SpinBox>("GridDebug/PositionComponents/ComponentZ") ??
					throw new InvalidOperationException("Missing GridDebug/PositionComponents/ComponentZ");
		_rotationX = GetNode<SpinBox>("GridDebug/RotationComponents/ComponentX") ??
					throw new InvalidOperationException("Missing GridDebug/RotationComponents/ComponentX");
		_rotationY = GetNode<SpinBox>("GridDebug/RotationComponents/ComponentY") ??
					throw new InvalidOperationException("Missing GridDebug/RotationComponents/ComponentY");
		_rotationZ = GetNode<SpinBox>("GridDebug/RotationComponents/ComponentZ") ??
					throw new InvalidOperationException("Missing GridDebug/RotationComponents/ComponentZ");

		_atmosphereStrength = GetNode<HSlider>("GridDebug/AtmosphereStrengthSlider") ??
							throw new InvalidOperationException("Missing GridDebug/AtmosphereStrengthSlider");
		_mountainHeight = GetNode<HSlider>("GridDebug/MountainHeightSlider") ??
						throw new InvalidOperationException("Missing GridDebug/MountainHeightSlider");
		_oceanDepth = GetNode<HSlider>("GridDebug/OceanDepthSlider") ??
					throw new InvalidOperationException("Missing GridDebug/OceanDepthSlider");
		_starCount = GetNode<HSlider>("GridDebug/StarCountSlider") ??
					throw new InvalidOperationException("Missing GridDebug/StarCountSlider");
		_atmosphereEnabled = GetNode<CheckButton>("GridDebug/AtmosphereEnabledToggle") ??
							throw new InvalidOperationException("Missing GridDebug/AtmosphereEnabledToggle");
		_oceanEnabled = GetNode<CheckButton>("GridDebug/OceanEnabledToggle") ??
						throw new InvalidOperationException("Missing GridDebug/OceanEnabledToggle");
		_starsEnabled = GetNode<CheckButton>("GridDebug/StarsEnabledToggle") ??
						throw new InvalidOperationException("Missing GridDebug/StarsEnabledToggle");

		_positionX.Changed += UpdateCameraFromOptions;
		_positionY.Changed += UpdateCameraFromOptions;
		_positionZ.Changed += UpdateCameraFromOptions;
		_rotationX.Changed += UpdateCameraFromOptions;
		_rotationY.Changed += UpdateCameraFromOptions;
		_rotationZ.Changed += UpdateCameraFromOptions;

		_atmosphereStrength.Changed += () =>
		{
			if (Generator?.Body?.Shading?.AtmosphereSettings is { } atmosphereSettings)
			{
				atmosphereSettings.Intensity = (float)_atmosphereStrength.Value;
			}
		};

		_mountainHeight.Changed += () =>
		{
			if (Generator?.Body?.Shape?.HeightMapCompute is EarthHeightModule earthHeightModule)
			{
				if (earthHeightModule.MountainsNoise is { } mountainsNoise)
				{
					mountainsNoise.Elevation = (float)_mountainHeight.Value;
				}
			}
		};

		_oceanDepth.Changed += () =>
		{
			if (Generator?.Body?.Shading?.OceanSettings is { } oceanSettings)
			{
				oceanSettings.Level = (float)_oceanDepth.Value;
			}
		};

		_starCount.Changed += () =>
		{
			if (Stars is { } stars)
			{
				stars.Count = Math.Max(0, (int)_starCount.Value);
			}
		};

		_atmosphereEnabled.Pressed += () =>
		{
			if (Generator?.Body?.Shading?.AtmosphereSettings is { } atmosphereSettings)
			{
				atmosphereSettings.Enabled = _atmosphereEnabled.ButtonPressed;
			}
		};

		_oceanEnabled.Pressed += () =>
		{
			if (Generator?.Body?.Shading?.OceanSettings is { } oceanSettings)
			{
				oceanSettings.Enabled = _oceanEnabled.ButtonPressed;
			}
		};

		_starsEnabled.Pressed += () =>
		{
			if (Stars is { } stars)
			{
				stars.Enabled = _starsEnabled.ButtonPressed;
			}
		};
	}

	public override void _Process(double delta)
	{
		base._Process(delta);

		UpdateOptionsFromCamera();
		// Console.WriteLine($"LP={Camera?.Position}, GP={Camera?.GlobalPosition}");
	}
}
