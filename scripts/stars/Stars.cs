using Godot;
using ProceduralPlanet.scripts.planet;
using static Godot.Extensions.PropertyHelper;

namespace ProceduralPlanet.scripts.stars;

[Tool]
public partial class Stars : Node3D
{
	private bool _dirty;

	private Camera3D? _camera;
	private CelestialBodyGenerator? _generator;
	private NodePath? _generatorPath;
	private Viewport? _sourceViewport;
	private NodePath? _sourceViewportPath;
	private ShaderMaterial? _material;

	private int _seed;
	private int _numStars;

	[Export] public GradientTexture2D? ColorSpectrum { get; set; }

	[Export] public GradientTexture2D? ClusterColorSpectrum { get; set; }

	[Export] public float DayTimeFade { get; set; } = 4;

	[Export] public float Distance { get; set; } = 10;

	[Export(PropertyHint.NodePathValidTypes, nameof(Node3D))]
	public NodePath? GeneratorPath
	{
		get => _generatorPath;
		set
		{
			if (!SetIfChanged(ref _generatorPath, value, ref _dirty))
			{
				return;
			}

			(var generator, _generator) = (_generator, default);
			if (generator is { NativeInstance: not default(nint) })
			{
				generator.Dispose();
			}
		}
	}

	[Export]
	public ShaderMaterial? Material
	{
		get => _material;
		set => SetIfChanged(ref _material, value, ref _dirty);
	}

	[Export] public float MaxBrightness { get; set; } = 1;

	[Export] public float MinBrightness { get; set; }

	[Export]
	public int NumStars
	{
		get => _numStars;
		set => SetIfChanged(ref _numStars, value, ref _dirty);
	}

	[Export]
	public int Seed
	{
		get => _seed;
		set => SetIfChanged(ref _seed, value, ref _dirty);
	}

	[Export]
	public Vector2 SizeMinMax { get; set; }

	[Export(PropertyHint.NodePathValidTypes, nameof(Viewport))]
	public NodePath? SourceViewportPath
	{
		get => _sourceViewportPath;
		set
		{
			if (!SetIfChanged(ref _sourceViewportPath, value, ref _dirty))
			{
				return;
			}

			(var sourceViewport, _sourceViewport) = (_sourceViewport, default);
			if (sourceViewport is { NativeInstance: not default(nint) })
			{
				sourceViewport.Dispose();
			}
		}
	}

	[Export] public FastNoiseLite? StarsClusteringNoise { get; set; }

	[Export(PropertyHint.Range, "0,10")] public float StarsClusteringAmplitude { get; set; } = 1;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_dirty = true;

		if (_generatorPath is { } generatorPath)
		{
			_generator ??= GetNode<CelestialBodyGenerator>(generatorPath);
		}

		if (_sourceViewportPath is { } sourceViewportPath)
		{
			_sourceViewport ??= GetNode<Viewport>(sourceViewportPath);
		}
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
		_material?.SetShaderParameter("MainTex", _sourceViewport?.GetTexture() ?? default(Variant));
		_material?.SetShaderParameter("Spectrum", ColorSpectrum ?? default(Variant));
		_material?.SetShaderParameter("ClusterSpectrum", ClusterColorSpectrum ?? default(Variant));
		_material?.SetShaderParameter("daytimeFade", DayTimeFade);
		_material?.SetShaderParameter("OceanRadius", GetNode(_generatorPath));
		_material?.SetShaderParameter("PlanetCentre", _generator?.GlobalPosition ?? default(Variant));


		return result;
	}

	private bool TryGenerateMesh()
	{
		return true;
	}
}