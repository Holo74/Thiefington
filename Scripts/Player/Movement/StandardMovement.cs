using Enums;
using Godot;
using System;

public class StandardMovement : IMovement
{
	private MovementNode ParentNode { get; set; }
	private float CurrentSpeed { get; set; } = 10;



	public Vector3 BackwardMove(CharacterBody3D body)
	{
		return body.Transform.Basis.X * -CurrentSpeed;
	}

	public void CrouchSignal(EStandingType crouchType)
	{
		switch (crouchType)
		{
			case EStandingType.Standing:
				CurrentSpeed = ParentNode.PlayerCharacter.PlayerVariables.StandingSpeed;
				break;
			case EStandingType.Crouching:
				CurrentSpeed = ParentNode.PlayerCharacter.PlayerVariables.CrouchingSpeed;
				break;
			case EStandingType.Prone:
				CurrentSpeed = ParentNode.PlayerCharacter.PlayerVariables.CrawlingSpeed;
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
		ParentNode.PerservedMovement = Vector3.Zero;
		CrouchSignal(ParentNode.PlayerCharacter.crouchingAssister.CurrentStanding);
		ParentNode.PlayerCharacter.crouchingAssister.DisableCrouch = false;
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
		ParentNode.PerservedMovement = Vector3.Up * ParentNode.PlayerCharacter.PlayerVariables.JumpStrength;
		return Vector3.Zero;
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
