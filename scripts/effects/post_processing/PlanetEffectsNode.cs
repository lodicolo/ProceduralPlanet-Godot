using System.Diagnostics;
using Godot;
using Godot.Extensions;
using ProceduralPlanet.scripts.planet;

namespace ProceduralPlanet.scripts.effects.post_processing;

[Tool]
public partial class PlanetEffectsNode : Node
{
	private MeshInstance3D? _atmosphereTargetMesh;
	private MeshInstance3D? _oceanTargetMesh;

	private CelestialBodyGenerator? _generator;
	private Node3D? _light;

	private Viewport? _atmosphereViewport;
	private Viewport? _oceanViewport;
	private Viewport? _sourceViewport;

	private PlanetEffectsHolderNode? _effectsHolder;

	[Export] public Shader? AtmosphereShader { get; set; }

	private MeshInstance3D? AtmosphereTargetMesh => this.GetNode(AtmosphereTargetMeshPath, ref _atmosphereTargetMesh);

	[Export(PropertyHint.NodePathValidTypes, "MeshInstance3D")] public NodePath? AtmosphereTargetMeshPath { get; set; }

	private Viewport? AtmosphereViewport => this.GetNode(AtmosphereViewportPath, ref _oceanViewport);

	[Export(PropertyHint.NodePathValidTypes, "Viewport")] public NodePath? AtmosphereViewportPath { get; set; }

	[Export] public bool DisplayAtmosphere { get; set; }

	[Export] public bool DisplayOceans { get; set; } = true;

	[Export(PropertyHint.NodePathValidTypes, "CelestialBodyGenerator")] public NodePath? GeneratorPath { get; set; }

	[Export(PropertyHint.NodePathValidTypes, "Node3D")] public NodePath? LightPath { get; set; }

	[Export] public Shader? OceanShader { get; set; }

	private MeshInstance3D? OceanTargetMesh => this.GetNode(OceanTargetMeshPath, ref _oceanTargetMesh);

	[Export(PropertyHint.NodePathValidTypes, "MeshInstance3D")] public NodePath? OceanTargetMeshPath { get; set; }

	private Viewport? OceanViewport => this.GetNode(OceanViewportPath, ref _oceanViewport);

	[Export(PropertyHint.NodePathValidTypes, "Viewport")] public NodePath? OceanViewportPath { get; set; }

	private Viewport? SourceViewport => this.GetNode(SourceViewportPath, ref _sourceViewport);

	[Export(PropertyHint.NodePathValidTypes, "Viewport")] public NodePath? SourceViewportPath { get; set; }

	public override void _Process(double delta)
	{
		base._Process(delta);

		_generator ??= GeneratorPath == default ? default : GetNode<CelestialBodyGenerator>(GeneratorPath);
		_light ??= LightPath == default ? default : GetNode<Node3D>(LightPath);

		if (_generator != default)
		{
			_effectsHolder ??= new PlanetEffectsHolderNode(_generator, _light);
		}
		else
		{
			Debug.WriteLine("No CelestialBodyGenerator");
		}

		if (SourceViewport is not { } sourceViewport)
		{
			return;
		}

		var nextViewport = sourceViewport;

		if (DisplayOceans &&
			_effectsHolder?.OceanEffect is { } oceanEffect &&
			OceanShader is { } oceanShader)
		{
			oceanEffect.UpdateSettings(nextViewport, _effectsHolder.Generator, oceanShader);
			if (OceanTargetMesh is { } oceanTargetMesh)
			{
				oceanTargetMesh.MaterialOverride = oceanEffect.ShaderMaterial;
			}

			nextViewport = OceanViewport ?? sourceViewport;
		}

		if (DisplayAtmosphere &&
			_effectsHolder?.AtmosphereEffect is { } atmosphereEffect &&
			AtmosphereShader is { } atmosphereShader)
		{
			atmosphereEffect.UpdateSettings(nextViewport, _effectsHolder.Generator, atmosphereShader);
			if (AtmosphereTargetMesh is { } atmosphereTargetMesh)
			{
				atmosphereTargetMesh.MaterialOverride = atmosphereEffect.ShaderMaterial;
			}

			nextViewport = AtmosphereViewport ?? sourceViewport;
		}
	}
}
