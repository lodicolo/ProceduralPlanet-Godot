using Godot;

namespace ProceduralPlanet.scripts.planet.shape.modules;

[Tool]
public abstract partial class HeightModule : ComputeResource
{
    public abstract float[] Run(RandomNumberGenerator rng, Vector3[] vertices);
}