using Godot;
using System;

public partial class Player : CharacterBody3D
{
	public static Player PLAYER { get; private set; }

	private Vector2 MouseRotation { get; set; }

	private float CurrentVerticalVelocity { get; set; } = 0;

	[ExportGroup("Internal Variables")]
	[ExportSubgroup("Player Nodes")]
	[Export]
	public Camera3D PlayerHead { get; set; }

	[ExportSubgroup("Variables")]
	[Export(PropertyHint.Range, ".1, 4, .1")]
	private float StandingFromGroundDistance { get; set; }

	// Remember that these are in radians
	[Export]
	private RotationAndClamp YRotation { get; set; }
	[Export]
	private RotationAndClamp ZRotation { get; set; }

	[Export]
	private PlayerVariables PlayerVariables { get; set; }
	private float defaultDownSpeed = -1;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		PLAYER = this;
	}

	public override void _ExitTree()
	{
		PLAYER = null;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);

		// Get Player movement stuff
		Velocity = GetMovement(Transform) * PlayerVariables.Speed;
		Velocity += Jump();
		Velocity += Gravity((float)(PlayerVariables.GravityValue * delta), this);

		MouseRotation = MouseToRotation(MouseRotation);
		MouseRotation = RotationMods(MouseRotation);

		// This is for the final actions of the player stuff
		MoveAndSlide();

		PlayerHead.RotateZ(ZRotation.RotateAmount(MouseRotation.Y));
		RotateY(YRotation.RotateAmount(MouseRotation.X));



		// Resetting values that might not get reset
		MouseRotation = Vector2.Zero;
		Velocity = Vector3.Zero;
	}

	private Vector3 Gravity(float gravityStrength, CharacterBody3D player)
	{
		CurrentVerticalVelocity += -gravityStrength;
		if (player.IsOnFloor() && CurrentVerticalVelocity < 0)
			CurrentVerticalVelocity = defaultDownSpeed;
		return Vector3.Up * CurrentVerticalVelocity;
	}

	private Vector3 Jump()
	{
		if (Input.IsActionJustPressed("ui_select"))
		{
			// Pulling the current vertical velocity because if you're falling and jump, then nothing happens at all pretty much
			// The plus one is because the default vertical speed on the floor is negative 1
			CurrentVerticalVelocity = PlayerVariables.JumpStrength;
			// return (JumpStrength - defaultDownSpeed - Math.Clamp(CurrentVerticalVelocity, float.NegativeInfinity, 0)) * Vector3.Up;
		}
		return Vector3.Zero;
	}

	private Vector3 GetMovement(Transform3D playerTransform)
	{
		Vector3 movement = Vector3.Zero;
		movement += (Input.IsActionPressed("Forward") ? 1.0f : 0.0f) * playerTransform.Basis.X;
		movement += (Input.IsActionPressed("Backward") ? 1.0f : 0.0f) * -playerTransform.Basis.X;
		movement += (Input.IsActionPressed("Right") ? 1.0f : 0.0f) * playerTransform.Basis.Z;
		movement += (Input.IsActionPressed("Left") ? 1.0f : 0.0f) * -playerTransform.Basis.Z;
		return movement;
	}

	private Vector2 MouseToRotation(Vector2 MouseMovement)
	{
		// Flipping mouse rotation to get "normal" rotation
		Vector2 rotation = MouseMovement;
		// Got to translate it into 
		rotation *= MathF.PI / 180.0f;

		return rotation;
	}

	private Vector2 RotationMods(Vector2 rotation)
	{
		Vector2 finRotation = rotation;
		return finRotation;
	}

	public override void _Input(InputEvent @event)
	{
		base._Input(@event);
		// Don't mind me, just getting the mouse movement over here
		if (@event is InputEventMouseMotion mouseMotion)
		{
			MouseRotation = -mouseMotion.Relative * MathF.PI / 180.0f;

		}
	}
}
