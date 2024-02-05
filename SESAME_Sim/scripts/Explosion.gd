extends AnimatedSprite2D


# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	# Make the explosion only play once
	if visible:
		if is_playing() and frame == 15:
			stop()
			visible = false
