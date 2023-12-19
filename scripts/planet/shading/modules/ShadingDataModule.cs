using Godot;

namespace ProceduralPlanet.scripts.planet.shading.modules;

[Tool]
public abstract partial class ShadingDataModule : Resource
{
    public abstract UVPairs Run(RandomNumberGenerator rng, Vector3[] vertices);
}