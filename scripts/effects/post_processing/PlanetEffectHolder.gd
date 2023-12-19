@tool
extends Node

class_name PlanetEffectHolder

var ocean_effect : Node
var atmosphere_effect : Node

var generator : CelestialBodyGenerator

func _init(
	_generator : CelestialBodyGenerator,
	light : Node
):
	generator = _generator
	if (generator.body.Shading.OceanSettings != null and
		generator.body.Shading.OceanSettings.Enabled
	):
		ocean_effect = OceanEffect.new(light)
	if (generator.body.Shading.AtmosphereSettings != null and
		generator.body.Shading.AtmosphereSettings.Enabled
	):
		atmosphere_effect = AtmosphereEffect.new(light)

func dist_from_surface(view_pos : Vector3) -> float:
		return max(0.0, (generator.global_position - view_pos).length() - generator.body_scale())

