using System.Diagnostics;
using Godot;

namespace ProceduralPlanet.scripts.effects;

[Tool]
public partial class AtmosphereEffectNode : PostProcessingEffectNode
{
    public AtmosphereEffectNode(Node3D? light) : base(light)
    {
    }

    protected override void OnUpdateSettings(Viewport source_viewport, CelestialBodyGenerator generator, Shader shader)
    {
        base.OnUpdateSettings(source_viewport, generator, shader);

        if (ShaderMaterial is not { } shaderMaterial)
        {
            return;
        }

        generator.BodySettings?.Shading?.AtmosphereSettings?.SetProperties(shaderMaterial, generator.BodyScale);

        var center = generator.GlobalPosition;
        shaderMaterial.SetShaderParameter("PlanetCentre", center);

        var oceanRadius = generator.OceanRadius;
        shaderMaterial.SetShaderParameter("OceanRadius", oceanRadius);

        var sourceTexture = source_viewport.GetTexture();
        shaderMaterial.SetShaderParameter("MainTex", sourceTexture);

        var sourceSize = source_viewport.GetVisibleRect().Size;
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