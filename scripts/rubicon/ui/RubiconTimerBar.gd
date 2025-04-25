class_name RubiconTimerBar extends GDHudElement

@export var label : Label
@export var fill : CanvasItem

var _length : float = 0.0
var _material : ShaderMaterial

func _ready() -> void:
	var audio : AudioStreamPlayer = play_field.Music
	_length = audio.stream.get_length()

func _process(_delta: float) -> void:
	var time : float = clampf(_length - Conductor.GetAudioTime(), 0, _length)
	var time_string : String = Time.get_time_string_from_unix_time(int(time))
	var times : PackedFloat64Array = time_string.split_floats(":")
	if times[0] == 0.0:
		label.text = "(" + time_string.substr(time_string.find(":") + 1) + ")"
	else:
		label.text = "(" + time_string + ")"

	if _material == null:
		_material = fill.material
		if _material == null:
			return

	_material.set_shader_parameter("value", time / _length)
