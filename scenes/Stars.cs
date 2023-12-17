using Godot;
using System;

public partial class Stars : Node3D
{
	private bool _dirty;

	private Camera3D? _camera;
	private Node3D? _generatorPath;
	private Viewport? _sourceViewportPath;
	private ShaderMaterial? _material;

	private int _seed;
	private int _numStars;

	private static void SetAndMarkDirty<T>(ref T variable, T value, ref bool dirty)
	{
		if (Equals(variable, value))
		{
			return;
		}

		variable = value;
		dirty = true;
	}

	[Export]
	public int Seed
	{
		get => _seed;
		set => SetAndMarkDirty(ref _seed, value, ref _dirty);
	}

	[Export]
	public int NumStars
	{
		get => _numStars;
		set => SetAndMarkDirty(ref _numStars, value, ref _dirty);
	}

	[Export]
	public Vector2 SizeMax { get; set; }

	[Export] public float MinBrightness { get; set; } = 0;

	[Export] public float MaxBrightness { get; set; } = 1;

	[Export] public float Distance { get; set; } = 10;

	[Export] public float DayTimeFade { get; set; } = 4;

	[Export]
	public ShaderMaterial? Material
	{
		get => _material;
		set => SetAndMarkDirty(ref _material, value, ref _dirty);
	}

	[Export] public GradientTexture2D? ColorSpectrum { get; set; }

	[Export] public GradientTexture2D? ClusterColorSpectrum { get; set; }

	[Export] public FastNoiseLite? StatsClusteringNoise { get; set; }

	[Export(PropertyHint.Range, "0,10")] public float StarsClusteringAmplitude { get; set; } = 1;

	[Export] public Node3D? GeneratorPath { get; set; }

	[Export] public Viewport? SourceViewportPath { get; set; }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_dirty = true;
		_generatorPath = GeneratorPath;
		_sourceViewportPath = SourceViewportPath;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		_dirty = !TryInitialize(_dirty);
	}

	private bool TryInitialize(bool regenerateMesh)
	{
		var result = !regenerateMesh || TryGenerateMesh();

		_camera = GetViewport().GetCamera3D();
		_material?.SetShaderParameter("MainTex", _sourceViewportPath?.GetTexture());
		_material?.SetShaderParameter("Spectrum", ColorSpectrum);
		_material?.SetShaderParameter("ClusterSpectrum", ClusterColorSpectrum);
		_material?.SetShaderParameter("daytimeFade", DayTimeFade);
		// _material?.SetShaderParameter("OceanRadius", _generatorPath?);
		_material?.SetShaderParameter("PlanetCentre", _generatorPath?.GlobalPosition ?? default(Vector3));


		return result;
	}

	private bool TryGenerateMesh()
	{
		return true;
	}
}
