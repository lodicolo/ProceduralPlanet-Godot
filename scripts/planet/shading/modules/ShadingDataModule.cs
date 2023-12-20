using Godot;
using ProceduralPlanet.scripts.planet.shape.modules;

namespace ProceduralPlanet.scripts.planet.shading.modules;

[Tool]
public abstract partial class ShadingDataModule : ComputeResource
{
    public abstract UVPairs Run(RandomNumberGenerator rng, Vector3[] vertices);
}