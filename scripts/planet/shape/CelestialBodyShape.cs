using System;
using Godot;
using ProceduralPlanet.scripts.planet.shape.modules;
using static Godot.Extensions.PropertyHelper;

namespace ProceduralPlanet.scripts.planet.shape;

[Tool]
public partial class CelestialBodyShape : Resource
{
    private readonly RandomNumberGenerator _rng = new();

    private HeightModule? _heightMapCompute;
    private PerturbModule? _perturbCompute;
    private float _perturbStrength = 0.7f;
    private bool _perturbVertices;
    private bool _randomize;
    private int _seed;

    [Export]
    public HeightModule? HeightMapCompute
    {
        get => _heightMapCompute;
        set
        {
            if (!SetIfChanged(ref _heightMapCompute, value, out var lastValue))
            {
                return;
            }

            if (lastValue is not null)
            {
                lastValue.Changed -= EmitChanged;
            }

            EmitChanged();

            if (_heightMapCompute is { } currentValue)
            {
                currentValue.ResourceName = "Height Module";
                currentValue.Changed += EmitChanged;
            }
        }
    }

    [Export]
    public PerturbModule? PerturbCompute
    {
        get => _perturbCompute;
        set
        {
            if (!SetIfChanged(ref _perturbCompute, value, out var lastValue))
            {
                return;
            }

            if (lastValue is not null)
            {
                lastValue.Changed -= EmitChanged;
            }

            EmitChanged();

            if (_perturbCompute is { } currentValue)
            {
                currentValue.ResourceName = "Perturb Module";
                currentValue.Changed += EmitChanged;
            }
        }
    }

    [Export(PropertyHint.Range, "0,1")]
    public float PerturbStrength
    {
        get => _perturbStrength;
        set => SetIfChanged(ref _perturbStrength, value, EmitChanged);
    }

    [Export]
    public bool PerturbVertices
    {
        get => _perturbVertices;
        set => SetIfChanged(ref _perturbVertices, value, EmitChanged);
    }

    [Export]
    public bool Randomize
    {
        get => _randomize;
        set => SetIfChanged(ref _randomize, value, EmitChanged);
    }

    [Export]
    public int Seed
    {
        get => _seed;
        set => SetIfChanged(ref _seed, value, EmitChanged);
    }

    public float[] CalculateHeights(Vector3[] vertices)
    {
        if (_heightMapCompute is not { } heightModule)
        {
            Console.WriteLine("No height module attached");
            return Array.Empty<float>();
        }

        _rng.Seed = (ulong)Seed;
        return heightModule.Run(_rng, vertices);
    }
}