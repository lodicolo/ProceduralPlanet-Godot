using Godot;

namespace ProceduralPlanet.scripts.planet.settings;

public abstract partial class SettingsResource : Resource
{
    [Export] public bool Enabled { get; set; } = true;
}