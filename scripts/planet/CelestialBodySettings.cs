using System;
using Godot;
using ProceduralPlanet.scripts.planet.shading;
using ProceduralPlanet.scripts.planet.shape;
using ProceduralPlanet.Utilities;

namespace ProceduralPlanet.scripts.planet;

[Tool]
public partial class CelestialBodySettings : Resource
{
    [Signal]
    public delegate void ShadingChangedEventHandler(CelestialBodyShading? resource);

    [Signal]
    public delegate void ShapeChangedEventHandler(CelestialBodyShape? resource);

    private CelestialBodyShading? _shading;
    private CelestialBodyShape? _shape;

    private Action? _shadingChangedSignalHandler;
    private Action? _shapeChangedSignalHandler;

    public CelestialBodySettings()
    {

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
                _shadingChangedSignalHandler = () => EmitSignal(SignalName.ShadingChanged, shading);
                shading.Changed += _shadingChangedSignalHandler;
            }

            EmitSignal(SignalName.ShadingChanged);
        }
    }

    [Export]
    public CelestialBodyShape? Shape
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
                _shapeChangedSignalHandler = () => EmitSignal(SignalName.ShapeChanged, shape);
                shape.Changed += _shapeChangedSignalHandler;
            }

            EmitSignal(SignalName.ShapeChanged);
        }
    }
}