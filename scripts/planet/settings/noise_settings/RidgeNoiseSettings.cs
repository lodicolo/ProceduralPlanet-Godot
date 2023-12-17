using Godot;
using ProceduralPlanet.Utilities;

namespace ProceduralPlanet.scripts.planet.settings.noise_settings;

[Tool]
public partial class RidgeNoiseSettings : Resource
{
    private float _elevation = 1;
    private float _gain = 1;
    private float _lacunarity = 2;
    private int _layers = 5;
    private Vector3 _offset;
    private float _peakSmoothing;
    private float _persistence = 0.5f;
    private float _power = 2;
    private float _scale = 1;
    private float _verticalShift;

    [Export]
    public float Elevation
    {
        get => _elevation;
        set
        {
            if (PropertyHelper.SetIfChanged(ref _elevation, value))
            {
                EmitChanged();
            }
        }
    }

    [Export]
    public float Gain
    {
        get => _gain;
        set
        {
            if (PropertyHelper.SetIfChanged(ref _gain, value))
            {
                EmitChanged();
            }
        }
    }

    [Export]
    public float Lacunarity
    {
        get => _lacunarity;
        set
        {
            if (PropertyHelper.SetIfChanged(ref _lacunarity, value))
            {
                EmitChanged();
            }
        }
    }

    [Export]
    public int Layers
    {
        get => _layers;
        set
        {
            if (PropertyHelper.SetIfChanged(ref _layers, value))
            {
                EmitChanged();
            }
        }
    }

    [Export]
    public Vector3 Offset
    {
        get => _offset;
        set
        {
            if (PropertyHelper.SetIfChanged(ref _offset, value))
            {
                EmitChanged();
            }
        }
    }

    [Export]
    public float PeakSmoothing
    {
        get => _peakSmoothing;
        set
        {
            if (PropertyHelper.SetIfChanged(ref _peakSmoothing, value))
            {
                EmitChanged();
            }
        }
    }

    [Export]
    public float Persistence
    {
        get => _persistence;
        set
        {
            if (PropertyHelper.SetIfChanged(ref _persistence, value))
            {
                EmitChanged();
            }
        }
    }

    [Export]
    public float Power
    {
        get => _power;
        set
        {
            if (PropertyHelper.SetIfChanged(ref _power, value))
            {
                EmitChanged();
            }
        }
    }

    [Export]
    public float Scale
    {
        get => _scale;
        set
        {
            if (PropertyHelper.SetIfChanged(ref _scale, value))
            {
                EmitChanged();
            }
        }
    }

    [Export]
    public float VerticalShift
    {
        get => _verticalShift;
        set
        {
            if (PropertyHelper.SetIfChanged(ref _verticalShift, value))
            {
                EmitChanged();
            }
        }
    }

    public float[] GetParams(RandomNumberGenerator rng)
    {
        var seededOffset = new Vector3(rng.Randf(), rng.Randf(), rng.Randf()) * rng.Randf() * 10_000;
        var offset = seededOffset + Offset;
        return new[]
        {
            offset.X,
            offset.Y,
            offset.Z,
            Layers,
            Persistence,
            Lacunarity,
            Scale,
            Elevation,
            Power,
            Gain,
            VerticalShift,
            PeakSmoothing,
        };
    }
}