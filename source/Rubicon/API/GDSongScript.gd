class_name GDSongScript extends Node

func _ready() -> void:
	Conductor.MeasureHit.connect(measure_hit)
	Conductor.BeatHit.connect(beat_hit)
	Conductor.StepHit.connect(step_hit)
	RubiconGame.PlayField.NoteHit.connect(note_hit)
	
func measure_hit(_measure : int) -> void:
	pass
	
func beat_hit(_beat : int) -> void:
	pass
	
func step_hit(_step : int) -> void:
	pass
	
func note_hit(_bar_line_name : StringName, _result : NoteResult):
	pass

func wait_for_second(time : float) -> void:
	while Conductor.Time < time:
		await get_tree().process_frame

func wait_for_measure(measure : float) -> void:
	while Conductor.GetCurrentMeasure() < measure:
		await get_tree().process_frame
	
func wait_for_beat(beat : float) -> void: 
	while Conductor.GetCurrentBeat() < beat:
		await get_tree().process_frame

func wait_for_steps(step : float) -> void:
	while Conductor.GetCurrentStep() < step:
		await get_tree().process_frame	