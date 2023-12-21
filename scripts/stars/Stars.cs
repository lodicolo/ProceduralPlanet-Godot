using System;
using Godot;
using Godot.Extensions;
using ProceduralPlanet.scripts.planet;
using static Godot.Extensions.PropertyHelper;
using Array = Godot.Collections.Array;

namespace ProceduralPlanet.scripts.stars;

[Tool]
public partial class Stars : Node3D
{
	private readonly RandomNumberGenerator _rng = new();

	private bool _dirty;

	private Camera3D? _camera;
	private GradientTexture2D? _colorSpectrum;
	private int _count;
	private GradientTexture2D? _clusterColorSpectrum;
	private float _dayTimeFade = 4;
	private float _distance = 10;
	private bool _enabled = true;
	private CelestialBodyGenerator? _generator;
	private NodePath? _generatorPath;
	private ShaderMaterial? _material;
	private float _maxBrightness = 1;
	private float _minBrightness;
	private int _seed;
	private Vector2 _sizeMinMax;
	private Viewport? _sourceViewport;
	private NodePath? _sourceViewportPath;
	private FastNoiseLite? _starsClusteringNoise;
	private float _starsClusteringAmplitude = 1;

	[Export]
	public GradientTexture2D? ColorSpectrum
	{
		get => _colorSpectrum;
		set => SetIfChanged(ref _colorSpectrum, value, ref _dirty);
	}

	[Export(PropertyHint.Range, "0,100000")]
	public int Count
	{
		get => _count;
		set => SetIfChanged(ref _count, value, ref _dirty);
	}

	[Export]
	public GradientTexture2D? ClusterColorSpectrum
	{
		get => _clusterColorSpectrum;
		set => SetIfChanged(ref _clusterColorSpectrum, value, ref _dirty);
	}

	[Export]
	public float DayTimeFade
	{
		get => _dayTimeFade;
		set => SetIfChanged(ref _dayTimeFade, value, ref _dirty);
	}

	[Export]
	public float Distance
	{
		get => _distance;
		set => SetIfChanged(ref _distance, value, ref _dirty);
	}

	[Export]
	public bool Enabled
	{
		get => _enabled;
		set => SetIfChanged(ref _enabled, value, ref _dirty);
	}

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
			generator?.Dispose();
		}
	}

	[Export]
	public ShaderMaterial? Material
	{
		get => _material;
		set => SetIfChanged(ref _material, value, ref _dirty);
	}

	[Export]
	public float MaxBrightness
	{
		get => _maxBrightness;
		set => SetIfChanged(ref _maxBrightness, value, ref _dirty);
	}

	[Export]
	public float MinBrightness
	{
		get => _minBrightness;
		set => SetIfChanged(ref _minBrightness, value, ref _dirty);
	}

	[Export]
	public int Seed
	{
		get => _seed;
		set => SetIfChanged(ref _seed, value, ref _dirty);
	}

	[Export]
	public Vector2 SizeMinMax
	{
		get => _sizeMinMax;
		set => SetIfChanged(ref _sizeMinMax, value, ref _dirty);
	}

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
			sourceViewport?.Dispose();
		}
	}

	[Export]
	public FastNoiseLite? StarsClusteringNoise
	{
		get => _starsClusteringNoise;
		set => SetIfChanged(ref _starsClusteringNoise, value, ref _dirty);
	}

	[Export(PropertyHint.Range, "0,10")]
	public float StarsClusteringAmplitude
	{
		get => _starsClusteringAmplitude;
		set => SetIfChanged(ref _starsClusteringAmplitude, value, ref _dirty);
	}

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
		if (!Enabled)
		{
			SetMesh(default);
			return true;
		}

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

	private void SetMesh(Mesh? mesh)
	{
		var starsMeshInstance = GetNodeOrNull<MeshInstance3D>("StarsMesh");
		if (starsMeshInstance == default)
		{
			return;
		}

		starsMeshInstance.Mesh = mesh;
		starsMeshInstance.MaterialOverride = _material?.Duplicate() as ShaderMaterial;
	}

	private bool TryGenerateMesh()
	{
		ArrayMesh mesh = new();

		Array arrays = new();
		arrays.Resize((int)Mesh.ArrayType.Max);

		var vertexCount = _count * 3;
		var vertices = new Vector3[vertexCount];
		var indices = new int[vertexCount];
		var uvs = new Vector2[vertexCount];
		var colors = new Color[vertexCount];

		_rng.Seed = (ulong)Seed;
		for (var vertexIndex = 0; vertexIndex < vertexCount; vertexIndex += 3)
		{
			var direction = _rng.RandomUnitSphereVector();
			var clusterNoise = StarsClusteringAmplitude * (StarsClusteringNoise?.GetNoise3Dv(direction) ?? 1);
			direction += Vector3.One * clusterNoise;
			direction = direction.Normalized();

			var (verticesTri, indicesTri, uvsTri) = GenerateTriangle(direction, vertexIndex);
			for (var offset = 0; offset < 3; ++offset)
			{
				vertices[vertexIndex + offset] = verticesTri[offset];
				indices[vertexIndex + offset] = indicesTri[offset];
				uvs[vertexIndex + offset] = uvsTri[offset];
				colors[vertexIndex + offset] = new Color(Math.Clamp(Math.Abs(clusterNoise), 0, 1), 0, 0);
			}
		}

		arrays[(int)Mesh.ArrayType.Vertex] = vertices;
		arrays[(int)Mesh.ArrayType.Index] = indices;
		arrays[(int)Mesh.ArrayType.TexUV] = uvs;
		arrays[(int)Mesh.ArrayType.Color] = colors;

		mesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, arrays);
		mesh.ResourceName = "Star Mesh";

		CallDeferred(nameof(SetMesh), mesh);
		return true;
	}

	private (Vector3[], int[], Vector2[]) GenerateTriangle(Vector3 direction, int indexOffset)
	{
		var size = _rng.RandfRange(SizeMinMax.X, SizeMinMax.Y);
		var brightness = _rng.RandfRange(MinBrightness, MaxBrightness);
		var spectrumT = _rng.Randf();

		var axisA = direction.Cross(Vector3.Up);
		if (axisA.LengthSquared() < 0.001)
		{
			axisA = direction.Cross(Vector3.Forward);
		}

		axisA = axisA.Normalized();

		var axisB = direction.Cross(axisA);
		var center = direction * Distance;

		Vector3[] vertices =
		{
			center,
			center,// + (axisA * Sin60 + axisB * Cos60) * size * 10,
			center,// + (axisA * Sin0 + axisB * Cos0) * size * 10,
		};

		int[] indices =
		{
			indexOffset,
			indexOffset + 2,
			indexOffset + 1,
		};

		Vector2[] uvs =
		{
			new(brightness, spectrumT),
			new(brightness, spectrumT),
			new(brightness, spectrumT),
		};

		return (vertices, indices, uvs);
	}
}