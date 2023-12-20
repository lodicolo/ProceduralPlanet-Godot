using System;
using Godot;
using ProceduralPlanet.scripts.planet;

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
        Name = nameof(PlanetEffectsHolderNode);

        if (Generator.Body?.Shading?.IsAtmosphereEnabled ?? false)
        {
            AtmosphereEffect = new AtmosphereEffectNode(light);
            AtmosphereEffect.Name = "AtmosphereEffect";
        }

        if (Generator.Body?.Shading?.IsOceanEnabled ?? false)
        {
            OceanEffect = new OceanEffectNode(light);
            OceanEffect.Name = "OceanEffect";
        }
    }
    
    public AtmosphereEffectNode? AtmosphereEffect { get; private set; }

    public CelestialBodyGenerator Generator { get; }
    
    public OceanEffectNode? OceanEffect { get; private set; }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (!disposing)
        {
            return;
        }

        (var atmosphereEffect, AtmosphereEffect) = (AtmosphereEffect, default);
        atmosphereEffect?.QueueFree();

        (var oceanEffect, OceanEffect) = (OceanEffect, default);
        oceanEffect?.QueueFree();
    }

    public float GetDistanceFromSurface(Vector3 viewPos)
    {
        var deltaP = Generator.GlobalPosition - viewPos;
        return Math.Max(0, deltaP.Length() - Generator.BodyScale);
    }
}