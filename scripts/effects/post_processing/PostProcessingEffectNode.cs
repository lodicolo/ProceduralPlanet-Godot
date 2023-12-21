using Godot;
using ProceduralPlanet.scripts.planet;

namespace ProceduralPlanet.scripts.effects.post_processing;

[Tool]
public abstract partial class PostProcessingEffectNode : EffectNode
{
    private ShaderMaterial? material;

    protected PostProcessingEffectNode(Node3D? light)
    {
        Light = light;
    }

    public Node3D? Light { get; }

    public ShaderMaterial? ShaderMaterial => material;

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing)
        {
            material?.Dispose();
        }
    }

    public void UpdateSettings(Viewport sourceViewport, CelestialBodyGenerator generator, Shader shader)
    {
        if (material?.Shader != shader)
        {
            material?.Dispose();
            material = new ShaderMaterial
            {
                RenderPriority = -1,
                Shader = shader,
            };
        }

        OnUpdateSettings(sourceViewport, generator, shader);
    }

    protected virtual void OnUpdateSettings(Viewport sourceViewport, CelestialBodyGenerator generator, Shader shader)
    {
    }

    protected override void Process(float deltaT)
    {

    }
}