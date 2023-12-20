using System;
using Godot;
using Godot.Extensions;
using static Godot.Extensions.PropertyHelper;

namespace ProceduralPlanet.scripts.sun;

[Tool]
public partial class Sun : Node3D
{
    private Node3D? _planet;
    private NodePath? _planetPath;

    private Node3D? Planet => this.GetNode(PlanetPath, ref _planet);

    [Export(PropertyHint.NodePathValidTypes, "Node3D")]
    public NodePath? PlanetPath
    {
        get => _planetPath;
        set
        {
            if (!SetIfChanged(ref _planetPath, value))
            {
                return;
            }

            (var planet, _planet) = (_planet, default);
            planet?.Dispose();
        }
    }

    [Export] public float SunDst { get; set; } = 1;
    [Export] public float TimeOfDay { get; set; }
    [Export] public float TimeSpeed { get; set; } = 0.01f;

    public override void _Process(double delta)
    {
        base._Process(delta);

        var deltaT = (float)delta;

        if (!Engine.IsEditorHint())
        {
            TimeOfDay += deltaT * TimeSpeed;
        }

        var (sin, cos) = MathF.SinCos(TimeOfDay);
        GlobalTransform = GlobalTransform with
        {
            Origin = new Vector3(cos, sin, 0) * SunDst
        };

        if (Planet is { } planet)
        {
            var planetToSun = GlobalPosition - planet.GlobalPosition;
            var planetToSunNormal = planetToSun.Normalized();
            var upVector = new Vector3(-planetToSun.Y, planetToSun.X, 0);
            LookAt(planet.GlobalPosition, upVector);
        }
    }
}