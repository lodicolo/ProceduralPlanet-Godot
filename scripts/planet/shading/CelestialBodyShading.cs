using Godot;
using ProceduralPlanet.scripts.planet.settings;
using ProceduralPlanet.scripts.planet.shading.modules;
using ProceduralPlanet.Utilities;

namespace ProceduralPlanet.scripts.planet.shading;

[Tool]
public partial class CelestialBodyShading : Resource
{
	protected readonly RandomNumberGenerator _rng = new();

	private AtmosphereSettings? _atmosphereSettings;
	private OceanSettings? _oceanSettings;
	private bool _randomize;
	private int _seed;
	private ShadingDataModule? _shadingDataModule;
	private ShaderMaterial? _terrainMaterial;

	private Vector2[]? _cachedShadingData;

	[Export]
	public AtmosphereSettings? AtmosphereSettings
	{
		get => _atmosphereSettings;
		set
		{
			if (!PropertyHelper.SetIfChanged(ref _atmosphereSettings, value, out var lastValue))
			{
				return;
			}

			if (lastValue is not null)
			{
				lastValue.Changed -= EmitChanged;
			}

			if (_atmosphereSettings is not { } atmosphereSettings)
			{
				return;
			}

			atmosphereSettings.ResourceName = "Atmosphere Settings";
			EmitChanged();
			atmosphereSettings.Changed += EmitChanged;
		}
	}

	[Export] public OceanSettings? OceanSettings
	{
		get => _oceanSettings;
		set
		{
			if (!PropertyHelper.SetIfChanged(ref _oceanSettings, value, out var lastValue))
			{
				return;
			}

			if (lastValue is not null)
			{
				lastValue.Changed -= EmitChanged;
			}

			if (_oceanSettings is not { } oceanSettings)
			{
				return;
			}

			oceanSettings.ResourceName = "Ocean Settings";
			EmitChanged();
			oceanSettings.Changed += EmitChanged;
		}
	}

	[Export]
	public ShadingDataModule? ShadingDataModule
	{
		get => _shadingDataModule;
		set
		{
			if (!PropertyHelper.SetIfChanged(ref _shadingDataModule, value, out var lastValue))
			{
				return;
			}

			if (lastValue is not null)
			{
				lastValue.Changed -= EmitChanged;
			}

			if (_shadingDataModule is not { } shadingDataModule)
			{
				return;
			}

			shadingDataModule.ResourceName = "Shading Data Compute Shader";
			EmitChanged();
			shadingDataModule.Changed += EmitChanged;
		}
	}

	[Export]
	public bool Randomize
	{
		get => _randomize;
		set
		{
			if (!PropertyHelper.SetIfChanged(ref _randomize, value))
			{
				return;
			}

			EmitChanged();
		}
	}

	[Export]
	public int Seed
	{
		get => _seed;
		set
		{
			if (!PropertyHelper.SetIfChanged(ref _seed, value))
			{
				return;
			}

			EmitChanged();
		}
	}

	[Export]
	public ShaderMaterial? TerrainMaterial
	{
		get => _terrainMaterial;
		set
		{
			if (!PropertyHelper.SetIfChanged(ref _terrainMaterial, value))
			{
				return;
			}

			EmitChanged();
		}
	}

	public bool IsAtmosphereEnabled => _atmosphereSettings?.Enabled ?? false;

	public bool IsOceanEnabled => _oceanSettings?.Enabled ?? false;

	public virtual void SetOceanProperties(ShaderMaterial? material) =>
		_oceanSettings?.SetProperties(material, _seed, _randomize);

	public virtual void SetTerrainProperties(ShaderMaterial? material, Vector2 heightMinMax, float bodyScale) {}

	public virtual UVPairs? GenerateShadingData(Vector3[] vertexArray)
	{
		_rng.Seed = (ulong)_seed;
		return _shadingDataModule?.Run(_rng, vertexArray);
	}
}