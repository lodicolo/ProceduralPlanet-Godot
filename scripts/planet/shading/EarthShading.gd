@tool
extends CelestialBodyShading

class_name EarthShading

@export var customized_colors : Resource :
	set(val):
		if (customized_colors == val):
			return
		customized_colors = val
		customized_colors.resource_name = "Customized Colors"
		emit_signal("changed")
		customized_colors.changed.connect(on_data_changed)

@export var randomized_colors : Resource :
	set(val):
		if (randomized_colors == val):
			return
		randomized_colors = val
		randomized_colors.resource_name = "Randomized Colors"
		emit_signal("changed")
		randomized_colors.changed.connect(on_data_changed)

func on_data_changed():
	emit_signal("changed")

func set_terrain_properties(material : Material, height_min_max : Vector2, body_scale : float):
	material.set_shader_parameter("HeightMinMax", height_min_max)
	material.set_shader_parameter("OceanLevel", ocean_level)
	material.set_shader_parameter("BodyScale", body_scale)

	if randomize_:
		set_random_colors(material)
		apply_colors(material, randomized_colors)
	else:
		apply_colors(material, customized_colors)

func apply_colors(material : Material, colors : Resource):
	material.set_shader_parameter("ShoreLow", colors.ShoreLow)
	material.set_shader_parameter("ShoreHigh", colors.ShoreHigh)

	material.set_shader_parameter("FlatLowA", colors.FlatLowA)
	material.set_shader_parameter("FlatHighA", colors.FlatHighA)

	material.set_shader_parameter("FlatLowB", colors.FlatLowB)
	material.set_shader_parameter("FlatHighB", colors.FlatHighB)

	material.set_shader_parameter("SteepLow", colors.SteepLow)
	material.set_shader_parameter("SteepHigh", colors.SteepHigh)

func set_random_colors(_material : Material):
	rng.randomize()
	randomized_colors.FlatLowA = ColorUtils.random_color(rng, 0.45, 0.6, 0.7, 0.8)
	randomized_colors.FlatHighA = ColorUtils.tweak_hsv(
		randomized_colors.FlatLowA,
		MathUtils.rand_signed(rng) * 0.2,
		MathUtils.rand_signed(rng) * 0.15,
		rng.randf_range(-0.25, 0.2)
	)

	randomized_colors.FlatLowB = ColorUtils.random_color(rng, 0.45, 0.6, 0.7, 0.8)
	randomized_colors.FlatHighB = ColorUtils.tweak_hsv(
		randomized_colors.FlatLowB,
		MathUtils.rand_signed(rng) * 0.2,
		MathUtils.rand_signed(rng) * 0.15,
		rng.randf_range(-0.25, 0.2)
	)

	randomized_colors.ShoreLow = ColorUtils.random_color(rng, 0.2, 0.3, 0.9, 1.0)
	randomized_colors.ShoreHigh = ColorUtils.tweak_hsv(
		randomized_colors.ShoreLow,
		MathUtils.rand_signed(rng) * 0.2,
		MathUtils.rand_signed(rng) * 0.2,
		rng.randf_range(-0.3, 0.2)
	)

	randomized_colors.SteepLow = ColorUtils.random_color(rng, 0.3, 0.7, 0.4, 0.6)
	randomized_colors.SteepHigh = ColorUtils.tweak_hsv(
		randomized_colors.SteepLow,
		MathUtils.rand_signed(rng) * 0.2,
		MathUtils.rand_signed(rng) * 0.2,
		rng.randf_range(-0.35, 0.2)
	)
