using System;
using Godot;

using static Godot.Extensions.PropertyHelper;

namespace ProceduralPlanet.scripts.planet.settings;

[Tool]
public partial class ResolutionSettings : Resource
{
    private int _collider = 100;
    private LODParameter[] _lodParameters = Array.Empty<LODParameter>();

    public ResolutionSettings()
    {
        if (_lodParameters.Length < 1)
        {
            _lodParameters = new[]
            {
                new LODParameter
                {
                    MinDistance = 300,
                    Resolution = 300
                },
                new LODParameter
                {
                    MinDistance = 1000,
                    Resolution = 100,
                },
                new LODParameter
                {
                    MinDistance = float.PositiveInfinity,
                    Resolution = 50,
                },
            };
        }
    }

    [Export]
    public int Collider
    {
        get => _collider;
        set
        {
            if (SetIfChanged(ref _collider, Math.Min(LODParameter.MaxAllowedResolution, value)))
            {
                EmitChanged();
            }
        }
    }

    [Export]
    public LODParameter[] LodParameters
    {
        get => _lodParameters;
        set
        {
            if (!SetIfChanged(ref _lodParameters, value))
            {
                return;
            }

            EmitChanged();
            foreach (var lodParameter in _lodParameters)
            {
                // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
                if (lodParameter is null)
                {
                    continue;
                }

                lodParameter.Changed += EmitChanged;
            }
        }
    }

    public int LodLevels => LodParameters.Length;

    public LODParameter? this[int lodLevel]
    {
        get
        {
            var lodParameters = LodParameters;
            var lodLevelCount = lodParameters.Length;
            if (lodLevelCount < 1)
            {
                return default;
            }

            var lodLevelInRange = (lodLevel % lodLevelCount + lodLevelCount) % lodLevelCount;
            return lodParameters[lodLevelInRange];
        }
    }
}