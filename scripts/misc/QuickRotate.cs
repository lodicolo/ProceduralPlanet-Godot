using System;
using Godot;

namespace ProceduralPlanet.scripts.misc;

[Tool]
public partial class QuickRotate : Node3D
{
    [Export] public float Speed { get; set; }

    public override void _Process(double delta)
    {
        var deltaT = (float)delta;
        Rotate(Vector3.Up, deltaT * Speed * MathF.PI / 180f);
    }
}