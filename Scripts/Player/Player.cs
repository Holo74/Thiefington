using Godot;
using System;

public partial class Player : CharacterBody3D
{
	public static Player PLAYER { get; private set; }

	private Vector2 MouseRotation { get; set; }


	[ExportGroup("Internal Variables")]
	[ExportSubgroup("Player Nodes")]
	[Export]
	public Camera3D PlayerHead { get; set; }

	[ExportSubgroup("Variables")]
	// Remember that these are in radians
	[Export]
	private RotationAndClamp YRotation { get; set; }
	[Export]
	private RotationAndClamp ZRotation { get; set; }

	[Export]
	private PlayerVariables PlayerVariables { get; set; }

	[Export]
	private double CrouchLongPress { get; set; } = 1.0;
	[Export]
	private CrouchingAssister crouchingAssister { get; set; }
	private double CrouchDuration { get; set; } = 0.0;

	[Export]
	private MovementNode MovementInstructions { get; set; }


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
		Vector2 finRotation = rotation * PlayerVariables.MouseRotationMult;
		return finRotation;
	}




	// Internal override functions

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
		if (Input.IsActionPressed("Left")) MovementInstructions.MoveDirection(MoveDirectionEnum.Left);
		if (Input.IsActionPressed("Backward")) MovementInstructions.MoveDirection(MoveDirectionEnum.Down);
		if (Input.IsActionPressed("Forward")) MovementInstructions.MoveDirection(MoveDirectionEnum.Forward);
		if (Input.IsActionPressed("Right")) MovementInstructions.MoveDirection(MoveDirectionEnum.Right);
		if (Input.IsActionPressed("Jump")) MovementInstructions.Jump();
		MovementInstructions.Lock();
		if (Input.IsActionPressed("Crouch"))
		{
			CrouchDuration += delta;
			if (CrouchDuration > CrouchLongPress)
			{
				crouchingAssister.LongPressCrouch();
				CrouchDuration = -800.0;
			}
		}
		else
		{
			if (CrouchDuration > 0.01 && CrouchDuration < CrouchLongPress)
			{
				crouchingAssister.Crouch();
			}
			CrouchDuration = 0;
		}


	}

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);

		// Get Player input stuff


		MouseRotation = MouseToRotation(MouseRotation);
		MouseRotation = RotationMods(MouseRotation);

		Velocity = MovementInstructions.RecieveMovement() + MovementInstructions.PerservedMovement;
		// This is for the final actions of the player stuff
		MoveAndSlide();

		PlayerHead.RotateZ(ZRotation.RotateAmount(MouseRotation.Y));
		RotateY(YRotation.RotateAmount(MouseRotation.X));


		// Resetting values that might not get reset
		MouseRotation = Vector2.Zero;
		Velocity = Vector3.Zero;
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
