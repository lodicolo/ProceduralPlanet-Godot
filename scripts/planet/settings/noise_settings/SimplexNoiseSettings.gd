@tool
extends Resource

class_name SimplexNoiseSettings

@export var num_layers : int = 4 :
	set(val):
		num_layers = val
		emit_changed()

@export var lacunarity : float = 2.0 :
	set(val):
		lacunarity = val
		emit_changed()

@export var persistence : float = 0.5 :
	set(val):
		persistence = val
		emit_changed()

@export var scale : float = 1.0 :
	set(val):
		scale = val
		emit_changed()

@export var elevation : float = 1.0 :
	set(val):
		elevation = val
		emit_changed()

@export var vertical_shift : float = 0.0 :
	set(val):
		vertical_shift = val
		emit_changed()

@export var offset : Vector3 = Vector3.ZERO :
	set(val):
		offset = val
		emit_changed()

func get_noise_params(rng : RandomNumberGenerator) -> Array[float]:
	var seeded_offset : Vector3 = \
		Vector3(rng.randf(), rng.randf(), rng.randf()) * rng.randf() * 10000.0
	
	var noise_params : Array[float] = [
		# [0]
		seeded_offset.x + offset.x,
		seeded_offset.y + offset.y,
		seeded_offset.z + offset.z,
		float(num_layers),
		# [1]
		persistence,
		lacunarity,
		scale,
		elevation,
		# [2]
		vertical_shift,
		0.0, 0.0, 0.0
	]

	return noise_params
