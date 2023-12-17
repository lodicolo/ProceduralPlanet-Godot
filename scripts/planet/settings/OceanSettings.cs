using Godot;

namespace ProceduralPlanet.scripts.planet.settings;

[Tool]
public partial class OceanSettings : SettingsResource
{
    [Export(PropertyHint.Range, "0,1")] public float Level { get; set; }
}