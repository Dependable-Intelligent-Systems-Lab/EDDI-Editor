extends SubViewportContainer

const MAX_ZOOM = 0.1

# Declare member variables here. Examples:
var camera = null
var dragging = false
var drag = Vector2(0,0)
var log_node = null
var mouse_pos = Vector2(0,0)
var smnodes = []
var viewport = null

var ft_script = null
var start_state= null
var degraded_state= null
var failed_state = null
var state_machine = null

var state_class = load("res://csharp/SMState.cs")
var state_machine_class = load("res://csharp/StateMachine.cs")

# Called when the node enters the scene tree for the first time.
func _ready():
	viewport = get_node("StateMachineViewport")
	camera = get_node("StateMachineViewport/StateMachineCamera2D")
	log_node = get_node("/root/Control/QuadGridContainer/LogPanel/VBoxContainer/LogScrollContainer/LogRichTextLabel")
	ft_script = get_node("/root/Control/QuadGridContainer/FaultTreePanel/VBoxContainer/FaultTreeViewportContainer")

	#state_machine = state_machine_class.BuildExampleStateMachine()
	#viewport.add_child(state_machine)


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	if dragging:
		camera.offset -= drag / camera.zoom.x
		drag = Vector2(0,0)
	
	return


func _gui_input(event):
	if event is InputEventMouseButton:
		if event.button_index == MOUSE_BUTTON_WHEEL_DOWN and event.pressed == false:
			if camera.zoom.x >= MAX_ZOOM + 0.1: 
				camera.zoom.x -= 0.1
				camera.zoom.y -= 0.1
		elif event.button_index == MOUSE_BUTTON_WHEEL_UP and event.pressed == false:
			if camera.zoom.x < 1:
				camera.zoom.x += 0.1
				camera.zoom.y += 0.1
		elif event.button_index == MOUSE_BUTTON_LEFT:
			if event.pressed == true:
				dragging = true
			else:
				dragging = false
				
				# Clicked on something?
				var world_pos = camera.get_local_mouse_position()
				for state in get_tree().get_nodes_in_group("smstates"): # We add them programmatically on creation
					if not state.visible: continue
					var corners = state.GetCorners()
					if world_pos.x >= corners[0].x and world_pos.x <= corners[1].x:
						if world_pos.y >= corners[0].y and world_pos.y <= corners[3].y:
							print("Selected state '" + state.name + "'")
							log_node.text += "Selected state '" + state.name + "'\n"
							break
				
	if event is InputEventMouse:
		if dragging:
			drag = event.position - mouse_pos
		mouse_pos = event.position


func set_state_machine(new_sm):
	if state_machine == new_sm: return
	if state_machine != null:
		# Remove it from the scene graph
		get_node("StateMachineViewport").remove_child(state_machine)
	state_machine = new_sm
	if new_sm != null:
		get_node("StateMachineViewport").add_child(state_machine)
		get_parent().get_node("Label").text = "STATE MACHINE VIEW: " + new_sm.FullName
