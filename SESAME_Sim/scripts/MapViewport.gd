extends SubViewportContainer

const drone_radius = 50

var camera = null
var drag = Vector2(0,0)
var dragging = false
var map_size = Vector2(0,0)
var mouse_pos = Vector2(0,0)
var mrs_state_machine = null
var selected_drone = null
var trigger_resize_check = false
var viewport = null

# Other nodes
var log_node = null
var fault_tree_display = null
var state_machine_display = null

# C# classes
var robot_class = load("res://csharp/Robot.cs")
var drone_class = load("res://csharp/Drone.cs")
var robot_arm_class = load("res://csharp/RobotArm.cs")
var config_class = load("res://csharp/Config.cs")
var fault_tree_class = load("res://csharp/FaultTree.cs")
var state_machine_class = load("res://csharp/StateMachine.cs")

# Called when the node enters the scene tree for the first time.
func _ready():
	camera = get_node("MapViewport/MapCamera2D")
	viewport = get_node("MapViewport")
	log_node = get_node("/root/Control/QuadGridContainer/LogPanel/VBoxContainer/LogScrollContainer/LogRichTextLabel")
	fault_tree_display = get_node("/root/Control/QuadGridContainer/FaultTreePanel/VBoxContainer/FaultTreeViewportContainer")
	state_machine_display = get_node("/root/Control/QuadGridContainer/StateMachinePanel/VBoxContainer/StateMachineViewportContainer")
	
	# Load the config file to set things up
	var config = config_class.LoadFromFile("config.json")
	
	# Setup the MRS state machine
	mrs_state_machine = state_machine_class.LoadFromODE(config.MRS_StateMachineName, null, log_node)
	#mrs_state_machine = state_machine_class.BuildExampleMRSStateMachine(log_node)
	
	# Set background
	get_node("MapViewport/MapBackground").texture = load("res://textures/"+config.MapName)
	map_size = get_node("MapViewport/MapBackground").texture.get_size()
	
	# Set base location
	get_node("MapViewport/HomeBase").position = config.HomeBasePosition
	
	# Create drones programmatically
	#var d1 = create_drone("Drone 1", Color(0.25, 0.5, 1.0), Vector2(100, 100))
	#var d2 = create_drone("Drone 2", Color(1.0, 1.0, 0.5), Vector2(-100, 100))
	var robots = config.CreateRobots(viewport, log_node)
	var d1 = robots[0]
	var d2 = robots[1]
	
	#var robotArm = robot_arm_class.new()
	#robotArm.Setup(viewport, "RobotArm 1", load("res://textures/robot_arm.png"), log_node)
	#robotArm.Colour = Color.ORANGE
	#robotArm.position = Vector2(250,50)
	
	# Assign each drone a SM and FT
	#fault_tree_class.BuildExampleODEFaultTree(d1, log_node)
	#fault_tree_class.BuildExampleDynamicFaultTree(d2, log_node)
	#state_machine_class.BuildExampleODEStateMachine(d1, log_node)
	#state_machine_class.BuildExampleStateMachine(d2, log_node)
	
	# Set active drone and update displays appropriately
	set_active_drone(d1)


func create_drone(name, colour, inposition):
	var droneClass = load("res://csharp/Drone.cs")
	var drone = droneClass.new()
	var rotor_positions = [Vector2(-31.5, -36.5),Vector2(31.5, -36.5),Vector2(31.5, 36.5),Vector2(-31.5, 36.5)]
	drone.Setup(viewport, name, load("res://textures/drone.png"), log_node, load("res://textures/drone_prop.png"), rotor_positions)
	drone.scale = Vector2(0.5, 0.5)
	drone.Colour = colour
	drone.position = inposition
	# Added to viewport inside Setup (because we also add the path there)
	return drone


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	if dragging:
		camera.offset -= drag / camera.zoom.x
		drag = Vector2(0,0)
		lock_to_background()
	# When we change window size, need to ensure the camera stays locked within the map -- even if that means zooming in
	if trigger_resize_check:
		var viewport_rect = viewport.get_visible_rect()
		while camera.zoom.x > 0.3 and viewport_rect.size.x / camera.zoom.x > map_size.x or viewport_rect.size.y / camera.zoom.x > map_size.y:
			camera.zoom.x += 0.1
			camera.zoom.y += 0.1
		lock_to_background()
		trigger_resize_check = false


func lock_to_background():
	# Lock to background map texture size
	var viewport_rect = viewport.get_visible_rect()
	var camera_rect = Rect2(camera.offset, viewport_rect.size / camera.zoom)

	if camera_rect.position.x - camera_rect.size.x/2 < -map_size.x/2:
		camera.offset = Vector2(camera_rect.size.x/2 - map_size.x/2, camera.offset.y)
	if camera_rect.position.x + camera_rect.size.x/2 > map_size.x/2:
		camera.offset = Vector2(map_size.x/2 - camera_rect.size.x/2, camera.offset.y)
	if camera_rect.position.y - camera_rect.size.y/2 < -map_size.y/2:
		camera.offset = Vector2(camera.offset.x, camera_rect.size.y/2 - map_size.y/2)
	if camera_rect.position.y + camera_rect.size.y/2 > map_size.y/2:
		camera.offset = Vector2(camera.offset.x, map_size.y/2 - camera_rect.size.y/2)


func _gui_input(event):
	if event is InputEventMouseButton:
		var viewport_rect = viewport.get_visible_rect()
		if event.button_index == MOUSE_BUTTON_WHEEL_DOWN and event.pressed == false:
			if camera.zoom.x > 0.3 and viewport_rect.size.x / (camera.zoom.x - 0.1) < map_size.x and viewport_rect.size.y / (camera.zoom.x - 0.1) < map_size.y:
				camera.zoom.x -= 0.1
				camera.zoom.y -= 0.1
				lock_to_background()
		elif event.button_index == MOUSE_BUTTON_WHEEL_UP and event.pressed == false:
			if camera.zoom.x < 4:
				camera.zoom.x += 0.1
				camera.zoom.y += 0.1
				lock_to_background()
		elif event.button_index == MOUSE_BUTTON_LEFT:
			if event.pressed == true:
				dragging = true
			else:
				dragging = false
				
				# Selected a drone?
				var world_pos = camera.get_local_mouse_position()
				var found = false
				for drone in get_tree().get_nodes_in_group("drones"): # We add the drone to the group in inspector->node->groups
					var distance = (drone.position - world_pos).length()
					if distance < (50 * drone.scale.x):
						set_active_drone(drone)
						found = true
				if not found:
					set_active_drone(null)
				
		if event.button_index == MOUSE_BUTTON_RIGHT and event.pressed == false:
			if selected_drone != null:
				# Set destination for selected drone to move to
				var world_pos = camera.get_local_mouse_position()
				if Input.is_key_pressed(KEY_SHIFT):
					selected_drone.AddDestination(world_pos)
					print("Queuing move order for drone '" + selected_drone.RobotName + "' to ", world_pos)
					log_node.text += "Queuing move order for drone '" + selected_drone.RobotName + "' to " + str(world_pos) +"\n"
				else:
					selected_drone.SetDestination(world_pos)
					print("Sending drone '" + selected_drone.RobotName + "' to ", world_pos)
					log_node.text += "Sending drone '" + selected_drone.RobotName + "' to " + str(world_pos) + "\n"
	if event is InputEventMouse:
		if dragging:
			drag = event.position - mouse_pos
		mouse_pos = event.position


func _on_control_resized():
	# Called when the window is resized
	if viewport != null:
		trigger_resize_check = true


func set_active_drone(drone):
	if drone == selected_drone: return
	if selected_drone != null:
		# Unselect it
		selected_drone.IsSelected = false
	
	selected_drone = drone
	if selected_drone != null:
		drone.IsSelected = true
		print("Selected drone '" + drone.RobotName + "'")
		log_node.text += "Selected drone '" + drone.RobotName + "'\n"
		# Set the SM/FT
		fault_tree_display.set_fault_tree(selected_drone.FaultTree)
		state_machine_display.set_state_machine(selected_drone.StateMachine)
	else:
		# Set the generic SM and wipe the FT
		fault_tree_display.set_fault_tree(null)
		state_machine_display.set_state_machine(mrs_state_machine)
