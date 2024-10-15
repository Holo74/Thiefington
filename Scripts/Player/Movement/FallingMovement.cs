using Enums;
using Godot;
using System;

public class FallingMovement : IMovement
{
	private MovementNode ParentNode { get; set; }
	private float GravityStrength { get; set; } = 5;
	public void Init(MovementNode parent)
	{
		ParentNode = parent;
		ParentNode.TouchSurface += LandOnSurface;
	}

	private void LandOnSurface(int surface, bool onSurface)
	{
		if (surface == (int)SurfacesTouched.Floor && onSurface)
		{
			ParentNode.StateSwitcher(PlayerMovementState.Standing);
		}
		if (surface == (int)SurfacesTouched.Ceiling && onSurface)
		{
			Vector3 holder = ParentNode.PerservedMovement;
			holder.Y = -1;
			ParentNode.PerservedMovement = holder;
		}
	}

	public void Kill()
	{
		ParentNode.TouchSurface -= LandOnSurface;
	}

	public void Processing(double delta)
	{
		ParentNode.PerservedMovement += Vector3.Down * GravityStrength * (float)delta;
	}

	public Vector3 LeftMove(CharacterBody3D body)
	{
		return Vector3.Zero;
	}

	public Vector3 RightMove(CharacterBody3D body)
	{
		return Vector3.Zero;
	}

	public Vector3 ForwardMove(CharacterBody3D body)
	{
		return Vector3.Zero;
	}

	public Vector3 BackwardMove(CharacterBody3D body)
	{
		return Vector3.Zero;
	}

	public Vector3 Jump(CharacterBody3D body)
	{
		return Vector3.Zero;
	}

	public void CrouchSignal(EStandingType crouchType)
	{

	}
}
