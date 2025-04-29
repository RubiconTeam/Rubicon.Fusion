@tool
extends EditorPlugin

var addon_path:String = get_script().get_path().get_base_dir()+"/"
var popup:Window = load(addon_path+"Warning.tscn").instantiate()

func _enter_tree() -> void:
	# VERY temporary
	if FileAccess.file_exists(addon_path+"built"):
		return
	
	var file:FileAccess = FileAccess.open(addon_path+"built", FileAccess.WRITE)
	
	add_child(popup)

func _exit_tree() -> void:
	# avoiding orphan node
	if popup != null:
		popup.queue_free()
