class_name FunkinHealthBar extends GDHudElement

## A classic Funkin' styled health bar.

static var NEUTRAL : StringName = &"neutral" ## The neutral animation
static var WINNING : StringName = &"win" ## The winning animation
static var LOSING : StringName = &"lose" ## The losing animation
static var TO_WINNING : StringName = &"toWin" ## The transition from neutral to winning animation.
static var TO_LOSING : StringName = &"toLose" ## The transition from neutral to losing animation.
static var FROM_WINNING : StringName = &"fromWin" ## The transition from winning to neutral animation.
static var FROM_LOSING : StringName = &"fromLose" ## The transition from losing to neutral animation.

@export var oppponent_data : CharacterIconData: ## The opponent's icon data to reference.
	get:
		return _opponent_data
	set(val):
		_opponent_data = val
		update_data()

@export var player_data : CharacterIconData: ## The player's icon data to reference.
	get:
		return _player_data
	set(val):
		_player_data = val
		update_data()

@export var size_lerp_weight : float = 9.0 ## How fast the icon sizes go back to normal.

@export_group("Time")
@export_enum("Measure", "Beat", "Step") var time_type : int: ## The time of type to go by with [member BounceTime]
	get:
		return _time_type
	set(val):
		_time_type = val
		if beat_syncer != null:
			beat_syncer.Type = _time_type

@export var bounce_time : float: ## How often to bounce.
	get:
		if beat_syncer != null:
			return beat_syncer.Value

		return _bounce_value
	set(val):
		_bounce_value = val
		if beat_syncer != null:
			beat_syncer.Value = _bounce_value

@export_group("References")
@export var progress_bar : ProgressBar ## The [ProgressBar] visualizing the amount of health left.
@export var left_icon : AnimatedSprite2D ## The icon on the left side.
@export var right_icon : AnimatedSprite2D ## The icon on the right side.
@export var icon_container : PathFollow2D ## The container for both icons.

var beat_syncer : BeatSyncer ## The node that controls when the icons bounce.

var _bounce_value : float = 1.0 / 4.0
var _time_type : int = TimeValue.MEASURE

var _previous_health : float = 0.0
var _previous_inverse : bool = false

var _fill_style : StyleBoxFlat
var _under_style : StyleBoxFlat

var _opponent_data : CharacterIconData
var _player_data : CharacterIconData

func initialize() -> void:
	super.initialize()

	beat_syncer = BeatSyncer.new()
	beat_syncer.Type = _time_type
	beat_syncer.Value = _bounce_value
	beat_syncer.name = "Bumper"
	beat_syncer.Bumped.connect(bump)
	add_child(beat_syncer)

	_fill_style = progress_bar.get_theme_stylebox("fill").duplicate()
	_under_style = progress_bar.get_theme_stylebox("background").duplicate()

	progress_bar.add_theme_stylebox_override("fill", _fill_style)
	progress_bar.add_theme_stylebox_override("background", _under_style)
	
	left_icon.animation_finished.connect(func()->void: _on_icon_animation_finished(left_icon))
	right_icon.animation_finished.connect(func()->void: _on_icon_animation_finished(right_icon))

	if not RubiconGame.Active or play_field.Metadata.Characters.size() == 0:
		return

	_initialize_characters()

func _process(_delta: float) -> void:
	if play_field == null:
		return
		
	var is_inverse : bool = is_inverse_fill()	
	if _previous_inverse != is_inverse:
		update_data()
		_previous_inverse = is_inverse

	var progress_ratio : float = float(play_field.Health) / play_field.MaxHealth
	var value : float = 1.0 - progress_ratio if is_inverse_fill() else progress_ratio
	progress_bar.ratio = progress_ratio
	icon_container.progress_ratio = value

	var player_winning : bool = play_field.Health > floori(play_field.MaxHealth * 0.8)
	var player_losing : bool = play_field.Health < floori(play_field.MaxHealth * 0.2)

	var player_icon : AnimatedSprite2D = get_player_icon()
	var opponent_icon : AnimatedSprite2D = get_opponent_icon()

	process_icon_animation(player_icon, player_losing, player_winning)
	process_icon_animation(opponent_icon, player_winning, player_losing)
	
	process_icon_interpolation(left_icon, get_left_icon_data().Scale, _delta)
	process_icon_interpolation(right_icon, get_right_icon_data().Scale * Vector2(-1, 1), _delta)

	_previous_health = play_field.Health

func bump() -> void: ## Bounces the icons a bit. Used mainly to sync with the beat.
	left_icon.scale = get_left_icon_data().Scale * 1.2
	right_icon.scale = get_right_icon_data().Scale * Vector2(-1.2, 1.2)
	
func update_data() -> void: ## Updates the bar with any changes were made to it.
	if progress_bar == null:
		return
	
	var left_style : StyleBoxFlat = get_left_style_box()
	var right_style : StyleBoxFlat = get_right_style_box()
	
	var left_icon_data : CharacterIconData = get_left_icon_data()
	var right_icon_data : CharacterIconData = get_right_icon_data()
	
	if left_icon_data != null:
		left_style.bg_color = left_icon_data.Color
		left_icon.sprite_frames = left_icon_data.Icon
		left_icon.offset = left_icon_data.Offset
		left_icon.scale = left_icon_data.Scale
		left_icon.texture_filter = left_icon_data.Filter as TextureFilter
		
	if right_icon_data != null:
		right_style.bg_color = right_icon_data.Color
		right_icon.sprite_frames = right_icon_data.Icon
		right_icon.offset = right_icon_data.Offset
		right_icon.scale = right_icon_data.Scale
		right_icon.texture_filter = right_icon_data.Filter as TextureFilter
	
func is_inverse_fill() -> bool: ## Whether the bar is filling backwards or not.
	var bar_direction : ProgressBar.FillMode = progress_bar.fill_mode as ProgressBar.FillMode
	return bar_direction == ProgressBar.FILL_END_TO_BEGIN or bar_direction == ProgressBar.FILL_TOP_TO_BOTTOM
	
func get_player_icon() -> AnimatedSprite2D: ## Gets the icon referencing the player.
	return left_icon if not is_inverse_fill() else right_icon

func get_opponent_icon() -> AnimatedSprite2D: ## Gets the icon referencing the opponent.
	return right_icon if not is_inverse_fill() else left_icon

func get_left_icon_data() -> CharacterIconData: ## Gets the left icon's [class CharacterIconData].
	return player_data if not is_inverse_fill() else oppponent_data
	
func get_right_icon_data() -> CharacterIconData: ## Gets the right icon's [class CharacterIconData].
	return oppponent_data if not is_inverse_fill() else player_data
	
func get_left_style_box() -> StyleBoxFlat: ## Gets the left style box.
	return _fill_style if not is_inverse_fill() else _under_style
	
func get_right_style_box() -> StyleBoxFlat: ## Gets the right style box.
	return _under_style if not is_inverse_fill() else _fill_style

func process_icon_interpolation(icon : AnimatedSprite2D, to : Vector2, delta : float) -> void: ## Processes an icon's interpolation back to its normal size.
	var _scale : Vector2 = icon.scale
	if _scale.is_equal_approx(to):
		return
		
	var next_scale : Vector2 = _scale.lerp(to, size_lerp_weight * delta) 
	_scale = to if next_scale.is_equal_approx(to) else next_scale 
	icon.scale = _scale

func process_icon_animation(icon : AnimatedSprite2D, losing : bool = false, winning : bool = false) -> void: ## Determines the animation to play for a certain icon.
	var current_anim : StringName = icon.animation
	if current_anim == TO_WINNING or current_anim == TO_LOSING or current_anim == FROM_WINNING or current_anim == FROM_LOSING:
		return

	var anim : StringName = NEUTRAL
	var frames : SpriteFrames = icon.sprite_frames
	if losing:
		if frames.has_animation(TO_LOSING):
			anim = TO_LOSING
		elif frames.has_animation(LOSING):
			anim = LOSING
	elif winning:
		if frames.has_animation(TO_WINNING):
			anim = TO_WINNING
		elif frames.has_animation(WINNING):
			anim = WINNING
	else:
		if current_anim == WINNING and frames.has_animation(FROM_WINNING):
			anim = FROM_WINNING
		elif current_anim == LOSING	and frames.has_animation(FROM_LOSING):
			anim = FROM_LOSING

	if not frames.has_animation(current_anim):
		return

	icon.play(anim)

func _on_icon_animation_finished(icon : AnimatedSprite2D) -> void: ## Called when either one of the icons finish an animation.
	var anim : StringName = icon.animation
	if anim == NEUTRAL or anim == WINNING or anim == LOSING:
		return

	var next_anim : StringName = NEUTRAL
	match anim:
		TO_WINNING:
			next_anim = WINNING
		TO_LOSING:
			next_anim = LOSING

	if not icon.sprite_frames.has_animation(next_anim):
		return

	icon.play(next_anim)		

func _initialize_characters() -> void: ## Used initially when setting up the health bar.
	if not RubiconGame.Active:
		return

	var space : RubiconEnvironmentSpace	= RubiconGame.Space
	var player_bar_line : StringName = play_field.TargetBarLine
	var first_player_character : RubiconCharacterController = space.GetCharacterGroup(player_bar_line).Controllers[0]
	_player_data = first_player_character.Data.Icon

	var first_opponent_barline : StringName = (play_field.BarLines.filter(func(b:BarLine)->bool:return b.name != player_bar_line).front() as BarLine).name
	var first_opponent_character : RubiconCharacterController = space.GetCharacterGroup(first_opponent_barline).Controllers[0]
	_opponent_data = first_opponent_character.Data.Icon

	update_data()	
