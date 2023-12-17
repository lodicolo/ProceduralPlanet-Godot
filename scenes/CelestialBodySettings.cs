using System;
using Godot;
using ProceduralPlanet.scripts.planet.settings;
using ProceduralPlanet.Utilities;

[Tool]
public partial class CelestialBodyShading : Resource
{
    private bool _randomize;
    private int _seed;
    private ShaderMaterial? _terrainMaterial;
    private ProceduralPlanet.scripts.planet.settings.AtmosphereSettings? _atmosphereSettings;
    private OceanSettings? _oceanSettings;

    [Export]
    public ProceduralPlanet.scripts.planet.settings.AtmosphereSettings? AtmosphereSettings
    {
        get => _atmosphereSettings;
        set => throw new NotImplementedException();
    }

    [Export] public OceanSettings? OceanSettings
    {
        get => _oceanSettings;
        set => throw new NotImplementedException();
    }

    public bool IsAtmosphereEnabled => _atmosphereSettings?.Enabled ?? false;

    public bool IsOceanEnabled => _oceanSettings?.Enabled ?? false;
}

[Tool]
public partial class CelestialBodySettings : Resource
{
    private const string SignalShadingChanged = "shading_changed";
    private const string SignalShapeChanged = "shape_changed";

    private CelestialBodyShading? _shading;
    private Resource? _shape;

    private Action? _shadingChangedSignalHandler;
    private Action? _shapeChangedSignalHandler;

    public event Action<Resource> ShadingChanged
    {
        add => Connect(SignalShadingChanged, Callable.From(value));
        remove => Disconnect(SignalShadingChanged, Callable.From(value));
    }

    public event Action<Resource> ShapeChanged
    {
        add => Connect(SignalShapeChanged, Callable.From(value));
        remove => Disconnect(SignalShapeChanged, Callable.From(value));
    }

    [Export]
    public CelestialBodyShading? Shading
    {
        get => _shading;
        set
        {
            if (!PropertyHelper.SetIfChanged(ref _shading, value, out var lastShading))
            {
                return;
            }

            if (lastShading is not null && _shadingChangedSignalHandler is not null)
            {
                lastShading.Changed -= _shadingChangedSignalHandler;
            }

            _shadingChangedSignalHandler = default;

            if (_shading is { } shading)
            {
                _shadingChangedSignalHandler = () => EmitSignal(SignalShadingChanged, shading);
                shading.Changed += _shadingChangedSignalHandler;
            }

            EmitSignal(SignalShadingChanged);
        }
    }

    [Export]
    public Resource? Shape
    {
        get => _shape;
        set
        {
            if (!PropertyHelper.SetIfChanged(ref _shape, value, out var lastShape))
            {
                return;
            }

            if (lastShape is not null && _shapeChangedSignalHandler is not null)
            {
                lastShape.Changed -= _shadingChangedSignalHandler;
            }

            _shapeChangedSignalHandler = default;

            if (_shape is { } shape)
            {
                _shapeChangedSignalHandler = () => EmitSignal(SignalShapeChanged, shape);
                shape.Changed += _shapeChangedSignalHandler;
            }

            EmitSignal(SignalShapeChanged);
        }
    }
}