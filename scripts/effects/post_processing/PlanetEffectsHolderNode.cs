using System;
using Godot;

namespace ProceduralPlanet.scripts.effects.post_processing;

[Tool]
public partial class PlanetEffectsHolderNode : Node
{
    public PlanetEffectsHolderNode(
        CelestialBodyGenerator generator,
        Node3D? light
    )
    {
        Generator = generator;

        if (Generator.BodySettings?.Shading?.IsAtmosphereEnabled ?? false)
        {
            AtmosphereEffect = new AtmosphereEffectNode(light);
        }

        if (Generator.BodySettings?.Shading?.IsOceanEnabled ?? false)
        {
            OceanEffect = new OceanEffectNode(light);
        }
    }
    
    public AtmosphereEffectNode? AtmosphereEffect { get; }

    public CelestialBodyGenerator Generator { get; }
    
    public OceanEffectNode? OceanEffect { get; }

    public float GetDistanceFromSurface(Vector3 viewPos)
    {
        var deltaP = Generator.GlobalPosition - viewPos;
        return Math.Max(0, deltaP.Length() - Generator.BodyScale);
    }
}