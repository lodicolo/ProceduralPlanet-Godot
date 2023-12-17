using Godot;
using static ProceduralPlanet.Utilities.PropertyHelper;

namespace ProceduralPlanet.scripts.planet.shading.colors;

[Tool]
public partial class EarthColors : Resource
{
    private Color _flatHighA;
    private Color _flatHighB;
    private Color _flatLowA;
    private Color _flatLowB;
    private Color _shoreHigh;
    private Color _shoreLow;
    private Color _steepHigh;
    private Color _steepLow;

    [Export]
    public Color FlatHighA
    {
        get => _flatHighA;
        set
        {
            if (SetIfChanged(ref _flatHighA, value))
            {
                EmitChanged();
            }
        }
    }

    [Export]
    public Color FlatHighB
    {
        get => _flatHighB;
        set
        {
            if (SetIfChanged(ref _flatHighB, value))
            {
                EmitChanged();
            }
        }
    }

    [Export]
    public Color FlatLowA
    {
        get => _flatLowA;
        set
        {
            if (SetIfChanged(ref _flatLowA, value))
            {
                EmitChanged();
            }
        }
    }

    [Export]
    public Color FlatLowB
    {
        get => _flatLowB;
        set
        {
            if (SetIfChanged(ref _flatLowB, value))
            {
                EmitChanged();
            }
        }
    }

    [Export]
    public Color ShoreHigh
    {
        get => _shoreHigh;
        set
        {
            if (SetIfChanged(ref _shoreHigh, value))
            {
                EmitChanged();
            }
        }
    }

    [Export]
    public Color ShoreLow
    {
        get => _shoreLow;
        set
        {
            if (SetIfChanged(ref _shoreLow, value))
            {
                EmitChanged();
            }
        }
    }

    [Export]
    public Color SteepHigh
    {
        get => _steepHigh;
        set
        {
            if (SetIfChanged(ref _steepHigh, value))
            {
                EmitChanged();
            }
        }
    }

    [Export]
    public Color SteepLow
    {
        get => _steepLow;
        set
        {
            if (SetIfChanged(ref _steepLow, value))
            {
                EmitChanged();
            }
        }
    }
}