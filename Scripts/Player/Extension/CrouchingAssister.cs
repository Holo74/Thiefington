using Godot;
using System;

public partial class CrouchingAssister : Node
{

	[Export]
	private Player PlayerCharacter { get; set; }
	[Export]
	private float CrouchHeight { get; set; }
	[Export]
	private float StandingHeight { get; set; }
	[Export]
	private float ProneHeight { get; set; }
	[Export]
	private float CrouchingSpeedPerUnit { get; set; }
	[Export]
	private float CrouchToProneSpeed { get; set; }
	[Export]
	private ShapeCast3D HeadSpaceCast { get; set; }
	[Export]
	private CollisionShape3D BodyCollision { get; set; }

	public Enums.EStandingType CurrentStanding { get; private set; }
	private Enums.EStandingType AttemptingMove { get; set; }

	// The int is actually an enum.  Go figure
	[Signal]
	public delegate void FinishCrouchStateEventHandler(int currentStanding);

	private Tween CurrentRunningTween { get; set; }
	public bool DisableCrouch { get; set; }

	public override void _Ready()
	{
		base._Ready();
		Setup(Enums.EStandingType.Standing);
		HeadSpaceCast.ExcludeParent = true;
	}

	public override void _Process(double delta)
	{
		if (HeadSpaceCast.IsColliding() && CurrentRunningTween?.IsRunning() == true)
		{
			GD.Print("Head space is colliding");
			AttemptCancelCrouch();
		}

	}

	public void Setup(Enums.EStandingType standingType)
	{
		CurrentStanding = standingType;
		AttemptingMove = CurrentStanding;
	}

	private SeparationRayShape3D GetBodyRay(CollisionShape3D collision)
	{
		return collision.Shape as SeparationRayShape3D;
	}

	private Tween HeightChangeTween(Tween.TransitionType transitionType, float heightMovingTo, float speed, Tween adder = null)
	{
		if (adder is null)
		{
			adder = GameManager.GAME_MANAGER.CreateTween();
		}
		else
		{
			adder.Chain();
		}
		speed = Math.Abs(speed);
		adder.TweenProperty(GetBodyRay(BodyCollision), "length", heightMovingTo, speed);
		adder.SetTrans(transitionType);
		return adder;
	}

	private bool CanCrouch()
	{
		// This should probably be the last line to check as it allows us to move up.
		return !HeadSpaceCast.IsColliding();
	}

	private float GetCurrentHeight()
	{
		return GetBodyRay(BodyCollision).Length;
	}

	private float TimeToCompleteCrouch(float target, float current, float speed)
	{
		return (target - current) * speed;
	}

	private async void Crouch(Enums.EStandingType wantedStandingState)
	{
		if (!CanCrouch())
		{
			// This might be needed for other things, but not right now
			return;
		}

		// We needed the comparision because the bool is bool? which can't be directly used
		if (CurrentRunningTween?.IsRunning() == true)
		{
			CurrentRunningTween.Kill();
		}
		switch (AttemptingMove)
		{
			case Enums.EStandingType.Standing:
				AttemptingMove = Enums.EStandingType.Crouching;
				CurrentRunningTween = HeightChangeTween(Tween.TransitionType.Spring, CrouchHeight, TimeToCompleteCrouch(CrouchHeight, GetCurrentHeight(), CrouchingSpeedPerUnit));
				if (wantedStandingState == Enums.EStandingType.Prone)
				{
					CurrentRunningTween.StepFinished += (long step) => { AttemptingMove = Enums.EStandingType.Prone; CurrentStanding = Enums.EStandingType.Crouching; };
					HeightChangeTween(Tween.TransitionType.Expo, ProneHeight, TimeToCompleteCrouch(ProneHeight, CrouchHeight, CrouchToProneSpeed), CurrentRunningTween);
				}
				break;
			case Enums.EStandingType.Crouching:
				AttemptingMove = wantedStandingState;
				if (wantedStandingState == Enums.EStandingType.Standing)
				{
					CurrentRunningTween = HeightChangeTween(Tween.TransitionType.Spring, StandingHeight, TimeToCompleteCrouch(StandingHeight, GetCurrentHeight(), CrouchingSpeedPerUnit));
				}
				if (wantedStandingState == Enums.EStandingType.Prone)
				{
					CurrentRunningTween = HeightChangeTween(Tween.TransitionType.Expo, ProneHeight, TimeToCompleteCrouch(ProneHeight, GetCurrentHeight(), CrouchToProneSpeed));
				}
				break;
			case Enums.EStandingType.Prone:
				CurrentRunningTween = HeightChangeTween(Tween.TransitionType.Spring, CrouchHeight, TimeToCompleteCrouch(CrouchHeight, GetCurrentHeight(), CrouchToProneSpeed));
				AttemptingMove = Enums.EStandingType.Crouching;
				if (wantedStandingState == Enums.EStandingType.Standing)
				{
					CurrentRunningTween.StepFinished += (long step) => { AttemptingMove = Enums.EStandingType.Standing; CurrentStanding = Enums.EStandingType.Crouching; };
					HeightChangeTween(Tween.TransitionType.Spring, StandingHeight, TimeToCompleteCrouch(StandingHeight, CrouchHeight, CrouchingSpeedPerUnit), CurrentRunningTween);
				}
				break;
			default:
				break;
		}
		CurrentRunningTween.Finished +=
		() =>
		{
			CurrentStanding = wantedStandingState;
			AttemptingMove = CurrentStanding;
			EmitSignal(SignalName.FinishCrouchState, (int)CurrentStanding);
		};
	}

	public void Crouch()
	{
		switch (AttemptingMove)
		{
			case Enums.EStandingType.Standing:
				Crouch(Enums.EStandingType.Crouching);
				break;
			case Enums.EStandingType.Crouching:
				Crouch(Enums.EStandingType.Standing);
				break;
			case Enums.EStandingType.Prone:
				Crouch(Enums.EStandingType.Crouching);
				break;
		}
	}

	public void LongPressCrouch()
	{
		switch (AttemptingMove)
		{
			case Enums.EStandingType.Standing:
			case Enums.EStandingType.Crouching:
				if (PlayerCharacter.IsOnFloor())
					Crouch(Enums.EStandingType.Prone);
				break;
			case Enums.EStandingType.Prone:
				Crouch(Enums.EStandingType.Standing);
				break;
		}
	}

	public void CancelCrouch()
	{
		//This may work as it attempts to go to the prior crouch state.
		Crouch(CurrentStanding);
	}

	public void AttemptCancelCrouch()
	{
		if (AttemptingMove == Enums.EStandingType.Standing)
		{
			CancelCrouch();
			return;
		}
		if (AttemptingMove == Enums.EStandingType.Crouching && CurrentStanding == Enums.EStandingType.Prone)
		{
			CancelCrouch();
		}

	}

	public void BumperHit(Node3D hitThing)
	{
		GD.Print("Head bumped into something");
		if (AttemptingMove == Enums.EStandingType.Standing)
		{
			CancelCrouch();
			return;
		}
		if (AttemptingMove == Enums.EStandingType.Crouching && CurrentStanding == Enums.EStandingType.Prone)
		{
			CancelCrouch();
		}
	}
}
