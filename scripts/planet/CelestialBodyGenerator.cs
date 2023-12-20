using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Godot;
using ProceduralPlanet.scripts.planet.settings;
using ProceduralPlanet.scripts.planet.shape.mesh;
using static Godot.Extensions.PropertyHelper;

namespace ProceduralPlanet.scripts.planet;

[Tool]
public partial class CelestialBodyGenerator : StaticBody3D
{
	private readonly List<ArrayMesh> _lodMeshes = new();
	private readonly Dictionary<int, PlanetSphereMesh> _sphereGenerators = new();

	private bool _dirty;


	private int _activeLODLevelIndex = -1;
	private CelestialBodySettings? _body;
	private Camera3D? _camera;
	private NodePath? _cameraPath;
	private Mesh? _collisionMesh;
	private CollisionShape3D? _collisionShape3D;
	private bool _debugDoubleUpdate = true;
	private int _debugNumUpdates;
	private Vector2 _heightMinMax;
	private bool _isOcean;
	private MeshInstance3D? _meshInstance3D;
	private ArrayMesh? _previewMesh;
	private CelestialBodyPreviewMode _previewMode = CelestialBodyPreviewMode.LOD2;
	private ResolutionSettings? _resolutionSettings;
	private bool _shadingNoiseSettingsUpdated = true;
	private bool _shapeSettingsUpdated = true;
	private Vector3[]? _vertexBuffer;

	private int ActiveLODLevelIndex
	{
		get => _activeLODLevelIndex;
		set
		{
			if (value < 0 || _lodMeshes.Count <= value)
			{
				Debug.WriteLine($"Invalid LOD level index: {value}");
				return;
			}

			if (SetIfChanged(ref _activeLODLevelIndex, value))
			{
				CallDeferred(nameof(SetMesh), _lodMeshes[_activeLODLevelIndex]);
			}
		}
	}

	[Export]
	public CelestialBodySettings? Body
	{
		get => _body;
		set
		{
			if (!SetIfChanged(ref _body, value, out var lastBodySettings))
			{
				return;
			}

			if (lastBodySettings is not null)
			{
				lastBodySettings.ShadingChanged -= BodySettingsOnShadingChanged;
				lastBodySettings.ShapeChanged -= BodySettingsOnShapeChanged;
			}

			if (_body is not { } bodySettings)
			{
				return;
			}

			bodySettings.ResourceName = "Body Settings";
			bodySettings.ShapeChanged += BodySettingsOnShapeChanged;
			bodySettings.ShadingChanged += BodySettingsOnShadingChanged;
		}
	}

	public float BodyScale => Transform.Basis.X.Length();

	public Camera3D? Camera
	{
		get
		{
			if (_camera == default && _cameraPath is { } cameraPath)
			{
				_camera ??= GetNode<Camera3D>(cameraPath);
			}

			return _camera;
		}
	}

	[Export(PropertyHint.NodePathValidTypes, "Camera3D")]
	public NodePath? CameraPath
	{
		get => _cameraPath;
		set
		{
			if (!SetIfChanged(ref _cameraPath, value))
			{
				return;
			}

			_camera = default;
		}
	}

	[Export]
	public bool IsOcean
	{
		get => _isOcean;
		set => SetIfChanged(ref _isOcean, value, ref _dirty);
	}

	public float OceanRadius => _body?.Shading?.IsOceanEnabled ?? false ? OceanRadiusUnscaled * BodyScale : 0;

	public float OceanRadiusUnscaled =>
		Mathf.Lerp(_heightMinMax.X, 1, _body?.Shading?.OceanSettings?.Level ?? 0);

	[Export]
	public CelestialBodyPreviewMode PreviewMode
	{
		get => _previewMode;
		set => SetIfChanged(ref _previewMode, value, ref _dirty);
	}

	[Export]
	public ResolutionSettings? ResolutionSettings
	{
		get => _resolutionSettings;
		set
		{
			if (!SetIfChanged(ref _resolutionSettings, value, out var lastResolutionSettings))
			{
				return;
			}

			if (lastResolutionSettings is not null)
			{
				lastResolutionSettings.Changed -= OnResolutionSettingsChanged;
			}

			if (_resolutionSettings is not { } resolutionSettings)
			{
				return;
			}

			resolutionSettings.ResourceName = "Resolution Settings";
			resolutionSettings.Changed += OnResolutionSettingsChanged;
		}
	}

	public int TerrainResolution
	{
		get
		{
			if (!Engine.IsEditorHint())
			{
				return 0;
			}

			if (_resolutionSettings is not { } resolutionSettings)
			{
				return 0;
			}

			if (resolutionSettings.LodParameters is not { Length: > 0 } lodParameters)
			{
				return 0;
			}

			switch (_previewMode)
			{
				case CelestialBodyPreviewMode.LOD0:
					return lodParameters[0].Resolution;
				case CelestialBodyPreviewMode.LOD1:
					if (lodParameters.Length > 1)
					{
						return lodParameters[1].Resolution;
					}
					break;
				case CelestialBodyPreviewMode.LOD2:
					if (lodParameters.Length > 2)
					{
						return lodParameters[2].Resolution;
					}
					break;
				case CelestialBodyPreviewMode.CollisionRes:
					return resolutionSettings.Collider;
				default:
					throw new ArgumentOutOfRangeException();
			}

			return 0;
		}
	}

	public Vector3 GetPointOnPlanet(Vector3 direction)
	{
		var normalizedDirection = direction.Normalized();
		var height = Body?.Shape?.CalculateHeights(new[] { normalizedDirection }).FirstOrDefault() ??
					direction.Length();
		return normalizedDirection * height * BodyScale;
	}

	private void BodySettingsOnShadingChanged(Resource? resource)
	{
		_shadingNoiseSettingsUpdated = true;
	}

	private void BodySettingsOnShapeChanged(Resource? resource)
	{
		_shapeSettingsUpdated = true;
	}

	private void OnResolutionSettingsChanged()
	{
	}

	public override void _Ready()
	{
		base._Ready();

		GenerateBodyInGame();
	}

	public override void _Process(double delta)
	{
		base._Process(delta);

		if (_collisionShape3D == default)
		{
			if (GetChildren().OfType<CollisionShape3D>().FirstOrDefault() is { } collisionShape3D)
			{
				collisionShape3D.TreeExiting += () => _collisionShape3D = default;
				_collisionShape3D = collisionShape3D;
			}
		}

		if (_meshInstance3D == default)
		{
			if (GetChildren().OfType<MeshInstance3D>().FirstOrDefault() is { } meshInstance3D)
			{
				meshInstance3D.TreeExiting += () => _meshInstance3D = default;
				_meshInstance3D = meshInstance3D;
			}
		}

		if (Engine.IsEditorHint())
		{
			GenerateBodyInEditor();
		}
		else
		{
			var distanceToCameraSquared = GlobalPosition.DistanceSquaredTo(Camera?.GlobalPosition ?? Vector3.Zero);

			for (var index = 0; index < _lodMeshes.Count; index++)
			{
				var lodParameter = _resolutionSettings?[index];
				if (lodParameter is null)
				{
					continue;
				}

				var lodDistance = BodyScale + lodParameter.MinDistance;
				if (distanceToCameraSquared > lodDistance * lodDistance)
				{
					continue;
				}

				ActiveLODLevelIndex = index;
				break;
			}
		}
	}

	private void SetCollisionMesh(Mesh mesh)
	{
		_collisionMesh = mesh;
		if (_collisionShape3D is { } collisionShape3D)
		{
			collisionShape3D.Shape = mesh.CreateTrimeshShape();
		}
		else
		{
			Debug.WriteLine("Unable to set collision mesh, no attached CollisionShape3D");
		}
	}

	private void SetMesh(Mesh mesh)
	{
		if (_meshInstance3D is { } meshInstance3D)
		{
			meshInstance3D.Mesh = mesh;
		}
		else
		{
			Debug.WriteLine("Unable to set mesh, no attached MeshInstance3D");
		}
	}

	private void SetCollisionMesh(ArrayMesh mesh, ArrayMesh arrayMesh)
	{
		_collisionMesh = arrayMesh;
	}

	private void GenerateBodyInEditor()
	{
		if (!Engine.IsEditorHint())
		{
			return;
		}

		if (_body is not { } body ||
			body.Shape is not { } shape ||
			shape.HeightMapCompute is not { } heightMapCompute)
		{
			return;
		}


	}

	private void GenerateBodyInGame()
	{
		if (Engine.IsEditorHint())
		{
			return;
		}

		if (_resolutionSettings is not { } resolutionSettings || resolutionSettings.LodLevels < 1)
		{
			return;
		}

		_lodMeshes.Clear();
		_lodMeshes.EnsureCapacity(resolutionSettings.LodLevels);

		if (_body is { Shape: not null } body)
		{
			for (var lodLevel = 0; lodLevel < resolutionSettings.LodLevels; ++lodLevel)
			{
				ArrayMesh lodMesh = new();
				var resolution = resolutionSettings.LodParameters[lodLevel].Resolution;
				var lodTerrainHeightMinMax = GenerateTerrainMesh(lodMesh, resolution);
				if (lodLevel == 0)
				{
					_heightMinMax = lodTerrainHeightMinMax;
				}
				_lodMeshes.Add(lodMesh);
			}

			CallDeferred(nameof(SetCollisionMesh), GenerateCollisionMesh(resolutionSettings.Collider));

			if (body.Shading is { } shading)
			{
				var materialOverride = shading.TerrainMaterial?.Duplicate() as ShaderMaterial;
				if (_meshInstance3D is { } meshInstance3D)
				{
					meshInstance3D.MaterialOverride = materialOverride;
				}
				shading.SetTerrainProperties(materialOverride, _heightMinMax, BodyScale);
			}

			ActiveLODLevelIndex = 2;
		}
		else if (IsOcean)
		{
			for (var lodLevel = 0; lodLevel < resolutionSettings.LodLevels; ++lodLevel)
			{
				var resolution = resolutionSettings.LodParameters[lodLevel].Resolution;
				GenerateOceanMesh(_lodMeshes[lodLevel], resolution);
			}

			CallDeferred(nameof(SetCollisionMesh), GenerateCollisionMesh(resolutionSettings.Collider), true);
		}
	}

	private (Vector3[] Vertices, int[] Indices) CreateSphereVertices(int resolution)
	{
		if (!_sphereGenerators.TryGetValue(resolution, out var mesh))
		{
			mesh = new PlanetSphereMesh(resolution);
			_sphereGenerators[resolution] = mesh;
		}

		var indices = mesh.Indices.ToArray();
		var vertices = mesh.Vertices.ToArray();
		return (vertices, indices);
	}

	private ArrayMesh GenerateCollisionMesh(int resolution, bool isOcean = false)
	{
		ArrayMesh collisionMesh = new();

		var arrays = new Godot.Collections.Array();
		arrays.Resize((int)Mesh.ArrayType.Max);

		var (vertices, indices) = CreateSphereVertices(resolution);

		if (!isOcean && _body is { Shape: {} shape })
		{
			var edgeLength = (vertices[indices[0]] - vertices[indices[1]]).Length();
			var heights = shape.CalculateHeights(vertices);
			if (shape is { PerturbVertices: true, PerturbCompute: { } perturbCompute })
			{
				var maxPerturbStrength = shape.PerturbStrength * edgeLength / 2f;
				vertices = perturbCompute.Run(vertices, maxPerturbStrength);
			}

			for (var index = 0; index < heights.Length; ++index)
			{
				vertices[index] *= heights[index];
			}
		}

		arrays[(int)Mesh.ArrayType.Vertex] = vertices;
		arrays[(int)Mesh.ArrayType.Index] = indices;

		collisionMesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, arrays);
		return collisionMesh;
	}

	private void GenerateOceanMesh(ArrayMesh mesh, int resolution)
	{
		var arrays = new Godot.Collections.Array();
		arrays.Resize((int)Mesh.ArrayType.Max);

		var (vertices, indices) = CreateSphereVertices(resolution);
		arrays[(int)Mesh.ArrayType.Vertex] = vertices;
		arrays[(int)Mesh.ArrayType.Index] = indices;

		var normals = RecalculateNormals(vertices, indices);
		arrays[(int)Mesh.ArrayType.Normal] = normals;

		mesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, arrays);
	}

	private Vector2 GenerateTerrainMesh(ArrayMesh mesh, int resolution)
	{
		var arrays = new Godot.Collections.Array();
		arrays.Resize((int)Mesh.ArrayType.Max);

		var (vertices, indices) = CreateSphereVertices(resolution);

		var edgeLength = (vertices[indices[0]] - vertices[indices[1]]).Length();

		var heights = _body?.Shape?.CalculateHeights(vertices) ?? Array.Empty<float>();

		if (_body is { Shape: { PerturbCompute: { } perturbCompute, PerturbVertices: true } shape })
		{
			var maxPerturbStrength = shape.PerturbStrength * edgeLength / 2f;
			vertices = perturbCompute.Run(vertices, maxPerturbStrength);
		}

		var minHeight = float.PositiveInfinity;
		var maxHeight = float.NegativeInfinity;
		for (var index = 0; index < heights.Length; ++index)
		{
			var height = heights[index];
			vertices[index] *= height;
			minHeight = Math.Min(minHeight, height);
			maxHeight = Math.Max(maxHeight, height);
		}

		arrays[(int)Mesh.ArrayType.Vertex] = vertices;
		arrays[(int)Mesh.ArrayType.Index] = indices;

		var normals = RecalculateNormals(vertices, indices);
		arrays[(int)Mesh.ArrayType.Normal] = normals;

		if (_body?.Shading is { } shading)
		{
			var shadingData = shading.GenerateShadingData(vertices);
			if (shadingData is not null)
			{
				arrays[(int)Mesh.ArrayType.TexUV] = shadingData.A;
				arrays[(int)Mesh.ArrayType.TexUV2] = shadingData.B;
			}
		}

		var crudeTangents = new float[vertices.Length << 2];
		for (var index = 0; index < vertices.Length; ++index)
		{
			var normal = normals[index];
			var offset = index << 2;
			crudeTangents[offset + 0] = -normal.Z;
			crudeTangents[offset + 1] = 0;
			crudeTangents[offset + 2] = normal.X;
			crudeTangents[offset + 3] = 0;
		}

		arrays[(int)Mesh.ArrayType.Tangent] = crudeTangents;

		mesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, arrays);

		return new Vector2(minHeight, maxHeight);
	}

	private static Vector3[] RecalculateNormals(IReadOnlyList<Vector3> vertices, IReadOnlyList<int> indices)
	{
		var normals = new Vector3[vertices.Count];
		for (var a = 0; a < indices.Count; a += 3)
		{
			var b = a + 1;
			var c = a + 2;

			var ia = indices[a];
			var ib = indices[b];
			var ic = indices[c];

			var va = vertices[ia];
			var vb = vertices[ib];
			var vc = vertices[ic];

			var ab = vb - va;
			var bc = vc - vb;
			var ca = va - vc;

			var abXbc = -ab.Cross(bc);
			var bcXca = -bc.Cross(ca);
			var caXab = -ca.Cross(ab);

			var normal = abXbc + bcXca + caXab;
			normals[ia] += normal;
			normals[ib] += normal;
			normals[ic] += normal;
		}

		for (var index = 0; index < normals.Length; ++index)
		{
			normals[index] = normals[index].Normalized();
		}

		return normals;
	}
}
