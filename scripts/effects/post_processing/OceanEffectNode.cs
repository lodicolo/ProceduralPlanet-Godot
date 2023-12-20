using System.Diagnostics;
using Godot;

namespace ProceduralPlanet.scripts.effects.post_processing;

[Tool]
public partial class OceanEffectNode : PostProcessingEffectNode
{
    public OceanEffectNode(Node3D? light) : base(light)
    {
    }

    protected override void OnUpdateSettings(Viewport sourceViewport, planet.CelestialBodyGenerator generator, Shader shader)
    {
        base.OnUpdateSettings(sourceViewport, generator, shader);

        if (ShaderMaterial is not { } shaderMaterial)
        {
            return;
        }

        generator.Body?.Shading?.AtmosphereSettings?.SetProperties(shaderMaterial, generator.BodyScale);

        var center = generator.GlobalPosition;
        shaderMaterial.SetShaderParameter("OceanCentre", center);

        var oceanRadius = generator.OceanRadius;
        shaderMaterial.SetShaderParameter("OceanRadius", oceanRadius);

        var sourceTexture = sourceViewport.GetTexture();
        shaderMaterial.SetShaderParameter("MainTex", sourceTexture);

        shaderMaterial.SetShaderParameter("PlanetScale", generator.BodyScale);

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

        generator.Body?.Shading?.SetOceanProperties(shaderMaterial);
    }
}