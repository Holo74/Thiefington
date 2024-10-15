using Enums;
using Godot;
using System;

public class StandardMovement : IMovement
{
	private MovementNode ParentNode { get; set; }
	private float CurrentSpeed { get; set; } = 10;
	private float StandingSpeed { get; set; } = 10;
	private float CrouchSpeed { get; set; } = 5;
	private float CrawlingSpeed { get; set; } = 2;
	private float JumpStrength { get; set; } = 10;

	public Vector3 BackwardMove(CharacterBody3D body)
	{
		return body.Transform.Basis.X * -CurrentSpeed;
	}

	public void CrouchSignal(EStandingType crouchType)
	{
		switch (crouchType)
		{
			case EStandingType.Standing:
				CurrentSpeed = StandingSpeed;
				break;
			case EStandingType.Crouching:
				CurrentSpeed = CrouchSpeed;
				break;
			case EStandingType.Prone:
				CurrentSpeed = CrawlingSpeed;
				break;
		}
	}

	public Vector3 ForwardMove(CharacterBody3D body)
	{
		return body.Transform.Basis.X * CurrentSpeed;
	}

	public void Init(MovementNode parent)
	{
		parent.TouchSurface += ChangedSurface;
		ParentNode = parent;
		ParentNode.PerservedMovement -= ParentNode.PerservedMovement * Vector3.Up;
	}

	private void ChangedSurface(int surface, bool status)
	{
		if (surface == (int)SurfacesTouched.Floor && status == false)
		{
			ParentNode.StateSwitcher(PlayerMovementState.Falling);
		}
	}

	public Vector3 Jump(CharacterBody3D body)
	{
		return Vector3.Up * JumpStrength;
	}

	public void Kill()
	{
		ParentNode.TouchSurface -= ChangedSurface;
	}

	public Vector3 LeftMove(CharacterBody3D body)
	{
		return body.Transform.Basis.Z * -CurrentSpeed;
	}

	public void Processing(double delta)
	{

	}

	public Vector3 RightMove(CharacterBody3D body)
	{
		return body.Transform.Basis.Z * CurrentSpeed;
	}
}
