using Godot;
using System;

public partial class HandHolder : Node3D
{
	private bool IsGrabbingHolds { get; set; } = false;
	[Signal]
	public delegate void GrabStateChangedEventHandler(bool IsGrabbing);

	[Export]
	private RayCast3D LeftHold { get; set; }

	[Export]
	private RayCast3D MiddleHold { get; set; }

	[Export]
	private RayCast3D RightHold { get; set; }

	public void GrabWall()
	{
		if (!IsGrabbingHolds)
		{
			return;
		}
		if (LeftHold.IsColliding() || RightHold.IsColliding() && MiddleHold.IsColliding())
		{
			IsGrabbingHolds = true;
			EmitSignal(SignalName.GrabStateChanged, true);
		}
	}

	public void DropGrab()
	{
		if (IsGrabbingHolds)
		{
			IsGrabbingHolds = false;
			EmitSignal(SignalName.GrabStateChanged, false);
		}
	}

	public Vector3 GetDirectionOfSlope(bool toLeft)
	{
		if (toLeft)
		{
			if (!LeftHold.IsColliding())
			{
				return Vector3.Zero;
			}
			Vector3 leftHoldPoint = LeftHold.GetCollisionPoint();
			Vector3 rightHoldPoint = leftHoldPoint;
			if (RightHold.IsColliding())
			{
				rightHoldPoint = RightHold.GetCollisionPoint();
			}
			if (MiddleHold.IsColliding())
			{
				rightHoldPoint = MiddleHold.GetCollisionPoint();
			}
			return leftHoldPoint - rightHoldPoint;
		}
		{
			if (!RightHold.IsColliding())
			{
				return Vector3.Zero;
			}
			Vector3 rightHoldPoint = RightHold.GetCollisionPoint();
			Vector3 leftHoldPoint = rightHoldPoint;
			if (LeftHold.IsColliding())
			{
				leftHoldPoint = LeftHold.GetCollisionPoint();
			}
			if (MiddleHold.IsColliding())
			{
				leftHoldPoint = MiddleHold.GetCollisionPoint();
			}

			return rightHoldPoint - leftHoldPoint;
		}
	}
}
