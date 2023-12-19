using Godot;

namespace ProceduralPlanet.scripts.effects;

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

    protected virtual void OnUpdateSettings(Viewport source_viewport, CelestialBodyGenerator generator, Shader shader)
    {
    }

    protected override void Process(float deltaT)
    {

    }
}