extends SubViewportContainer

const MAX_ZOOM = 0.1

# Declare member variables here. Examples:
var camera = null
var dragging = false
var drag = Vector2(0,0)
var log_node = null
var mouse_pos = Vector2(0,0)
var fault_tree = null
var viewport = null

var fault_tree_class = load("res://csharp/FaultTree.cs")
var ftnode_class = load("res://csharp/FTNode.cs")

# Called when the node enters the scene tree for the first time.
func _ready():
	viewport = get_node("FaultTreeViewport")
	camera = get_node("FaultTreeViewport/FauitTreeCamera2D")
	camera.zoom = Vector2(0.5, 0.5)
	camera.offset = Vector2(200, 300)
	log_node = get_node("/root/Control/QuadGridContainer/LogPanel/VBoxContainer/LogScrollContainer/LogRichTextLabel")

	#fault_tree = fault_tree_class.BuildExampleFaultTree()
	#viewport.add_child(fault_tree)
	

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	if dragging:
		camera.offset -= drag / camera.zoom.x
		drag = Vector2(0,0)
	pass


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
				
				# Clicked on an FTNode?
				var world_pos = camera.get_local_mouse_position()
				var found = false
				for node in get_tree().get_nodes_in_group("ftnodes"): # We add them programmatically on creation
					if not node.visible: continue
					var distance = (node.position - world_pos).length()
					if distance < 75:
						print("Selected ftnode '" + node.NodeName + "'")
						log_node.text += "Selected ftnode '" + node.NodeName + "'\n"
						found = true
						break
		elif event.button_index == MOUSE_BUTTON_RIGHT:
			if event.pressed == false:
				# Clicked on an FTNode?
				var world_pos = camera.get_local_mouse_position()
				for node in get_tree().get_nodes_in_group("ftnodes"): # We add them programmatically on creation
					if not node.visible: continue
					var distance = (node.position - world_pos).length()
					if distance < 75:
						print("Activated ftnode '" + node.NodeName + "'")
						log_node.text += "[color=red]Activated ftnode '" + node.NodeName + "'\n[/color]"
						node.Activate()
						break
	if event is InputEventMouse:
		if dragging:
			drag = event.position - mouse_pos
		mouse_pos = event.position
	

func set_fault_tree(new_ft):
	if fault_tree == new_ft: return
	if fault_tree != null:
		# Remove it from the scene graph
		get_node("FaultTreeViewport").remove_child(fault_tree)
	fault_tree = new_ft
	if new_ft != null:
		get_node("FaultTreeViewport").add_child(fault_tree)
		var text = "FAULT TREE VIEW: " + new_ft.FaultTreeName
		if new_ft.Robot != null: text += " @ " + new_ft.Robot.RobotName
		get_parent().get_node("Label").text = text
