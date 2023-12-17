using System;
using Godot;
using ProceduralPlanet.Utilities;

namespace ProceduralPlanet.scripts.planet.settings;

public partial class LODParameter : Resource
{
    internal const int MaxAllowedResolution = 500;

    private float _minDistance;
    private int _resolution;

    [Export]
    public float MinDistance
    {
        get => _minDistance;
        set
        {
            if (PropertyHelper.SetIfChanged(ref _minDistance, value))
            {
                EmitChanged();
            }
        }
    }

    [Export]
    public int Resolution
    {
        get => _resolution;
        set
        {
            if (PropertyHelper.SetIfChanged(ref _resolution, Math.Min(MaxAllowedResolution, value)))
            {
                EmitChanged();
            }
        }
    }
}