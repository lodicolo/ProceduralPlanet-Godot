using Godot;

namespace ProceduralPlanet.scripts.effects;

[Tool]
public partial class ShakeEffect : Node
{
    private readonly RandomNumberGenerator _rng = new();

    private Vector3 _previousMotionDelta;

    [Export] public bool Enabled { get; set; } = true;

    [Export] public Vector3 Amount { get; set; } = Vector3.Right + Vector3.Up;

    [Export] public bool IsShaking { get; set; }

    [Export] public float Speed { get; set; }

    public override void _Process(double delta)
    {
        base._Process(delta);

        var parent = GetParent();
        if (!Enabled || parent is not Camera3D { Current: true } camera)
        {
            return;
        }

        camera.Position -= _previousMotionDelta;
        _previousMotionDelta = default;

        if (!IsShaking)
        {
            return;
        }

        Vector3 perturbation = new(_rng.RandfRange(-1, 1), _rng.RandfRange(-1, 1), _rng.RandfRange(-1, 1));

        var deltaT = (float)delta;
        _previousMotionDelta = camera.GlobalTransform.Basis * Amount * perturbation * Speed * deltaT;
        camera.Position += _previousMotionDelta;
    }
}