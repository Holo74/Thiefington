using Godot;
using System;

public class InputGetter
{
	public enum Directions
	{
		Left,
		Right,
		Forward,
		Backward
	}

	// If outside sources need to stop input.  Though, maybe don't use it.  
	private bool canInput { get; set; } = true;
	public bool CanInput()
	{
		// There is a strong posibility that there'll be a lot more conditions in the future that prevent input
		return canInput;
	}

	public bool PressedJump()
	{
		return CanInput() && Input.IsActionJustPressed("ui_select");
	}

	public bool PressingDirection(Directions directions)
	{
		switch (directions)
		{
			case Directions.Left:
				return CanInput() && Input.IsActionPressed("Left");
			case Directions.Right:
				return CanInput() && Input.IsActionPressed("Right");
			case Directions.Forward:
				return CanInput() && Input.IsActionPressed("Forward");
			case Directions.Backward:
				return CanInput() && Input.IsActionPressed("Backward");
			default:
				return false;
		}
	}
}
