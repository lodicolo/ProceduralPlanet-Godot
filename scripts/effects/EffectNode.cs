using Godot;

namespace ProceduralPlanet.scripts.effects;

[Tool]
public abstract partial class EffectNode : Node
{
    public override void _Process(double delta)
    {
        base._Process(delta);

        Process((float)delta);
    }

    protected abstract void Process(float deltaT);
}