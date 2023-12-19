using System;
using System.Collections.Generic;
using System.Diagnostics;
using Godot;
using ProceduralPlanet.scripts.planet.settings;
using static ProceduralPlanet.Utilities.PropertyHelper;

public partial class CelestialBodyGenerator : StaticBody3D
{
	private bool _dirty;

	private bool _isOcean;
	private CelestialBodyPreviewMode _previewMode = CelestialBodyPreviewMode.LOD2;
	private ResolutionSettings? _resolutionSettings;

	[Export]
	public bool IsOcean
	{
		get => _isOcean;
		set => SetIfChanged(ref _isOcean, value, ref _dirty);
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

	[Export]
	public CelestialBodyPreviewMode PreviewMode
	{
		get => _previewMode;
		set => SetIfChanged(ref _previewMode, value, ref _dirty);
	}

	private Camera3D? _camera;
	private NodePath? _cameraPath;

	[Export]
	public NodePath? Camera
	{
		get => _cameraPath;
		set
		{
			if (!SetIfChanged(ref _cameraPath, value))
			{
				return;
			}

			_camera = default;
			if (_cameraPath != default)
			{
				_camera = GetNode<Camera3D>(_cameraPath);
			}
		}
	}

	private ProceduralPlanet.scripts.planet.CelestialBodySettings? _bodySettings;

	[Export]
	public ProceduralPlanet.scripts.planet.CelestialBodySettings? BodySettings
	{
		get => _bodySettings;
		set
		{
			if (!SetIfChanged(ref _bodySettings, value, out var lastBodySettings))
			{
				return;
			}

			if (lastBodySettings is not null)
			{
				lastBodySettings.ShadingChanged -= BodySettingsOnShadingChanged;
				lastBodySettings.ShapeChanged -= BodySettingsOnShapeChanged;
			}

			if (_bodySettings is not { } bodySettings)
			{
				return;
			}

			bodySettings.ResourceName = "Body Settings";
			bodySettings.ShapeChanged += BodySettingsOnShapeChanged;
			bodySettings.ShadingChanged += BodySettingsOnShadingChanged;
		}
	}

	public float BodyScale => Transform.Basis.X.Length();

	public float OceanRadius => _bodySettings?.Shading?.IsOceanEnabled ?? false ? OceanRadiusUnscaled * BodyScale : 0;

	public float OceanRadiusUnscaled =>
		Mathf.Lerp(_heightMinMax.X, 1, _bodySettings?.Shading?.OceanSettings?.Level ?? 0);

	public Vector3 GetPointOnPlanet(Vector3 direction)
	{
		throw new NotImplementedException();
	}

	private bool _debugDoubleUpdate = true;
	private int _debugNumUpdates;

	private ArrayMesh? _previewMesh;
	private Mesh? _collisionMesh;
	private readonly List<ArrayMesh> _lodMeshes = new();

	private Vector3[] _vertexBuffer;
	private bool _shadingNoiseSettingsUpdated = true;
	private bool _shapeSettingsUpdated = true;
	private Vector2 _heightMinMax;
	private int _activeLODLevelIndex = -1;
	private Dictionary<int, PlanetSphereMesh> _sphereGenerators = new();

	private void BodySettingsOnShadingChanged(Resource obj)
	{
		_shadingNoiseSettingsUpdated = true;
	}

	private void BodySettingsOnShapeChanged(Resource obj)
	{
		_shapeSettingsUpdated = true;
	}

	private void OnResolutionSettingsChanged()
	{
	}

	public override void _Ready()
	{
		base._Ready();

		_camera = GetNode<Camera3D>(_cameraPath);
		
		GenerateBodyInGame();
	}

	public override void _Process(double delta)
	{
		base._Process(delta);

		if (Engine.IsEditorHint())
		{
			GenerateBodyInEditor();
		}
		else
		{
			var distanceToCameraSquared = GlobalPosition.DistanceSquaredTo(_camera?.GlobalPosition ?? Vector3.Zero);

			for (var index = 0; index < _lodMeshes.Count; index++)
			{
				var lodMesh = _lodMeshes[index];
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

				_activeLODLevelIndex = index;
				CallDeferred(nameof(SetMesh), lodMesh);
				break;
			}
		}
	}

	private void SetMesh(Mesh mesh)
	{
		if (FindChild("MeshInstance3D", recursive: false) is not MeshInstance3D meshInstance3D)
		{
			Debug.WriteLine($"{GetType().Name} does not have a {nameof(MeshInstance3D)}");
			return;
		}

		meshInstance3D.Mesh = mesh;
	}

	private void GenerateBodyInEditor()
	{
		if (!Engine.IsEditorHint())
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
	}
}
