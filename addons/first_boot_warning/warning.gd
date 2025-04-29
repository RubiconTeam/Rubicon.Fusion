@tool
extends Window

func _ready() -> void:
	connect("close_requested", _on_close_requested, CONNECT_DEFERRED)
	%"3".connect("meta_clicked", _on_meta_clicked, CONNECT_DEFERRED)
	%"Button".connect("pressed", _on_close_requested, CONNECT_DEFERRED)

func _on_close_requested() -> void:
	queue_free()

func _on_meta_clicked(meta: Variant) -> void:
	OS.shell_open(str(meta))
