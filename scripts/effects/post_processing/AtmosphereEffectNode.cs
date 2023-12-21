using System.Diagnostics;
using Godot;
using ProceduralPlanet.scripts.planet;

namespace ProceduralPlanet.scripts.effects.post_processing;

[Tool]
public partial class AtmosphereEffectNode : PostProcessingEffectNode
{
    public AtmosphereEffectNode(Node3D? light) : base(light)
    {
    }

    protected override void OnUpdateSettings(Viewport sourceViewport, CelestialBodyGenerator generator, Shader shader)
    {
        base.OnUpdateSettings(sourceViewport, generator, shader);

        if (ShaderMaterial is not { } shaderMaterial || !(generator.Body?.Shading?.IsAtmosphereEnabled ?? false))
        {
            return;
        }

        generator.Body?.Shading?.AtmosphereSettings?.SetProperties(shaderMaterial, generator.BodyScale);

        var center = generator.GlobalPosition;
        shaderMaterial.SetShaderParameter("PlanetCentre", center);

        var oceanRadius = generator.OceanRadius;
        shaderMaterial.SetShaderParameter("OceanRadius", oceanRadius);

        var sourceTexture = sourceViewport.GetTexture();
        shaderMaterial.SetShaderParameter("MainTex", sourceTexture);

        var sourceSize = sourceViewport.GetVisibleRect().Size;
        shaderMaterial.SetShaderParameter("ScreenWidth", sourceSize.X);
        shaderMaterial.SetShaderParameter("ScreenHeight", sourceSize.Y);

        var dirToSun = Vector3.Up;
        if (Light is { } light)
        {
            dirToSun = (light.GlobalPosition - center).Normalized();
        }
        else
        {
            Debug.WriteLine("No directional light");
        }
        shaderMaterial.SetShaderParameter("DirToSun", dirToSun);
    }
}