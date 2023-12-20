using System.Collections.Generic;
using Godot;
using static Godot.Extensions.PropertyHelper;

namespace ProceduralPlanet.scripts.planet.monitors;

[Tool]
public partial class AtmosphereEntryExitMonitor : Node3D
{
    private readonly List<Area3D> _previousIntersectingAreas = new();

    private Area3D? _atmosphereInner;
    private Area3D? _atmosphereOuter;

    private CelestialBodyGenerator? _planet;
    private NodePath? _planetPath;

    private CelestialBodyGenerator? Planet
    {
        get
        {
            if (_planet == default  && _planetPath is {} planetPath)
            {
                _planet ??= GetNode<CelestialBodyGenerator>(planetPath);
            }

            return _planet;
        }
    }

    [Export(PropertyHint.NodePathValidTypes, "CelestialBodyGenerator")]
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

    [Export] public float ShakeStrength { get; set; } = 20;

    public override void _Ready()
    {
        base._Ready();

        _atmosphereInner ??= FindChild("AtmosphereInner") as Area3D;
        _atmosphereOuter ??= FindChild("AtmosphereOuter") as Area3D;

        if (Planet is not { Body.Shading.AtmosphereSettings.AtmosphereScale: var atmosphereScale } planet)
        {
            return;
        }

        var atmosphereRadius = (1f + atmosphereScale) * planet.BodyScale;
        if (_atmosphereInner is { } atmosphereInner)
        {
            var collider = atmosphereInner.GetChild<CollisionShape3D>(0);
            if (collider.Shape is SphereShape3D sphereShape3D)
            {
                sphereShape3D.Radius = planet.BodyScale + 0.3f * (atmosphereRadius - planet.BodyScale);
            }
        }

        if (_atmosphereOuter is { } atmosphereOuter)
        {
            var collider = atmosphereOuter.GetChild<CollisionShape3D>(0);
            if (collider.Shape is SphereShape3D sphereShape3D)
            {
                sphereShape3D.Radius = planet.BodyScale + 1.3f * (atmosphereRadius - planet.BodyScale);
            }
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
    }
}