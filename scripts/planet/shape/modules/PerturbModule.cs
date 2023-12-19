using Godot;

namespace ProceduralPlanet.scripts.planet.shape.modules;

[Tool]
public abstract partial class PerturbModule : ComputeResource
{
    public abstract Vector3[] Run(Vector3[] vertices, float maxPerturbStrength);
}