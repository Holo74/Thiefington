using Godot;
using System;
using System.Diagnostics.Tracing;

public partial class MovementNode : Node
{
	private IMovement CurrentState { get; set; }

	public Vector3 PerservedMovement { get; set; }
	private Vector3 PriorMovement { get; set; }

	[Export]
	public Player PlayerCharacter { get; private set; }

	private Vector3 Movement { get; set; }
	private bool MovementLock { get; set; }

	[Signal]
	public delegate void TouchSurfaceEventHandler(int surfaceTouched, bool status);

	private bool[] TouchingSurfaces = new bool[3];

	public override void _Ready()
	{
		base._Ready();
		PerservedMovement = Vector3.Zero;
		SetTouchingSurfaces();
		if (TouchingSurfaces[(int)SurfacesTouched.Floor])
		{
			CurrentState = new StandardMovement();
		}
		else
		{
			CurrentState = new FallingMovement();
		}
		CurrentState.Init(this);
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
		CurrentState.Processing(delta);
	}

	private void SetTouchingSurfaces()
	{
		TouchingSurfaces[(int)SurfacesTouched.Ceiling] = PlayerCharacter.IsOnCeiling();
		TouchingSurfaces[(int)SurfacesTouched.Wall] = PlayerCharacter.IsOnWall();
		TouchingSurfaces[(int)SurfacesTouched.Floor] = PlayerCharacter.IsOnFloor();
	}

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);

		bool[] surfaceComparision = new bool[] { TouchingSurfaces[0], TouchingSurfaces[1], TouchingSurfaces[2] };
		SetTouchingSurfaces();
		for (int i = 0; i < 3; i++)
		{
			if (TouchingSurfaces[i] != surfaceComparision[i])
			{
				EmitSignal(SignalName.TouchSurface, i, TouchingSurfaces[i]);
			}
		}
	}

	public void MoveDirection(MoveDirectionEnum direction)
	{
		if (MovementLock)
			return;
		switch (direction)
		{
			case MoveDirectionEnum.Forward:
				Movement += CurrentState.ForwardMove(PlayerCharacter);
				break;
			case MoveDirectionEnum.Down:
				Movement += CurrentState.BackwardMove(PlayerCharacter);
				break;
			case MoveDirectionEnum.Left:
				Movement += CurrentState.LeftMove(PlayerCharacter);
				break;
			case MoveDirectionEnum.Right:
				Movement += CurrentState.RightMove(PlayerCharacter);
				break;
		}
	}

	public void Lock()
	{
		MovementLock = true;
	}

	public Vector3 RecieveMovement()
	{
		MovementLock = false;
		Vector3 returningMove = Movement;
		PriorMovement = Movement;
		Movement = Vector3.Zero;
		return returningMove + PerservedMovement;
	}

	public void Jump()
	{
		if (MovementLock)
			return;
		Movement += CurrentState.Jump(PlayerCharacter);
	}

	public void CrouchReceiver(int Receiver)
	{
		CurrentState.CrouchSignal((Enums.EStandingType)Receiver);
	}

	public void StateSwitcher(PlayerMovementState state)
	{
		GD.Print("State switched to: " + state);
		CurrentState.Kill();
		switch (state)
		{
			case PlayerMovementState.Falling:
				GD.Print(PriorMovement);
				PerservedMovement += PriorMovement;
				CurrentState = new FallingMovement();
				break;
			case PlayerMovementState.Climbing:
				break;
			case PlayerMovementState.Standing:
				CurrentState = new StandardMovement();
				break;
			case PlayerMovementState.Swimming:
				break;
		}
		CurrentState.Init(this);
	}
}

public enum MoveDirectionEnum
{
	Forward,
	Down,
	Left,
	Right
}

public enum PlayerMovementState
{
	Standing,
	Falling,
	Swimming,
	Climbing
}

public enum SurfacesTouched
{
	Ceiling,
	Wall,
	Floor
}