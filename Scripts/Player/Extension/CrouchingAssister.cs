using Godot;
using System;

public partial class CrouchingAssister : Node
{

	[Export]
	private Player PlayerCharacter { get; set; }
	private float CrouchHeight { get; set; }
	private float StandingHeight { get; set; }
	private float ProneHeight { get; set; }
	[Export]
	private float CrouchingSpeedPerUnit { get; set; }
	[Export]
	private float CrouchToProneSpeed { get; set; }
	[Export]
	private Node3D HeadNode { get; set; }
	[Export]
	private CollisionShape3D BodyCollision { get; set; }
	[Export]
	private CollisionShape3D LegsCollision { get; set; }

	public Enums.EStandingType CurrentStanding { get; private set; }
	private Enums.EStandingType AttemptingMove { get; set; }

	// The int is actually an enum.  Go figure
	[Signal]
	public delegate void FinishCrouchStateEventHandler(int currentStanding);

	private Tween CurrentRunningTween { get; set; }
	public bool DisableCrouch { get; set; }

	private int areaBit { get; set; }
	private float HeadRestingSpace = 0.875f;

	public override void _Ready()
	{
		base._Ready();
		Setup(Enums.EStandingType.Standing);
		areaBit = 3;
		StandingHeight = HeadRestingSpace;
		CrouchHeight = StandingHeight - .5f;
		ProneHeight = CrouchHeight - .5f;
	}

	public override void _Process(double delta)
	{

	}

	public void Setup(Enums.EStandingType standingType)
	{
		CurrentStanding = standingType;
		AttemptingMove = CurrentStanding;
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
		adder.TweenProperty(HeadNode, "position:y", heightMovingTo, speed);
		adder.SetTrans(transitionType);
		return adder;
	}

	private bool CanCrouch(Enums.EStandingType standingType)
	{
		// This should probably be the last line to check as it allows us to move up.
		if (standingType > CurrentStanding)
		{
			return true;
		}
		switch (CurrentStanding)
		{
			case Enums.EStandingType.Crouching:
				return areaBit >= 2;
			case Enums.EStandingType.Prone:
				return areaBit >= 3;
			default:
				break;
		}
		return false;
	}

	private float GetCurrentHeight()
	{
		return HeadNode.Position.Y;
	}

	private float TimeToCompleteCrouch(float target, float current, float speed)
	{
		return (target - current) * speed;
	}

	private void JumpNode(bool up, Node3D node, float amount = .5f)
	{
		Vector3 jumper = Vector3.Up * amount * (up ? 1 : -1);
		node.Position += jumper;
	}

	private async void Crouch(Enums.EStandingType wantedStandingState)
	{
		if (!CanCrouch(wantedStandingState))
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
				CurrentRunningTween.StepFinished += (long step) =>
				{
					JumpNode(true, HeadNode);
					JumpNode(false, PlayerCharacter);
					LegsCollision.Disabled = true;
				};
				if (wantedStandingState == Enums.EStandingType.Prone)
				{
					// Weird black magic is going on right here where the final prone state will cause the camera to jitter a fuck ton
					CurrentRunningTween.StepFinished += (long step) => { AttemptingMove = Enums.EStandingType.Prone; CurrentStanding = Enums.EStandingType.Crouching; };
					HeightChangeTween(Tween.TransitionType.Expo, CrouchHeight, TimeToCompleteCrouch(CrouchHeight, HeadRestingSpace, CrouchToProneSpeed), CurrentRunningTween);
					CurrentRunningTween.Finished += () =>
					{
						// JumpNode(true, HeadNode);
						JumpNode(false, PlayerCharacter);
						BodyCollision.Disabled = true;
					};
				}
				break;
			case Enums.EStandingType.Crouching:
				AttemptingMove = wantedStandingState;
				if (wantedStandingState == Enums.EStandingType.Standing)
				{
					JumpNode(true, PlayerCharacter);
					JumpNode(false, HeadNode);
					LegsCollision.Disabled = false;
					CurrentRunningTween = HeightChangeTween(Tween.TransitionType.Spring, StandingHeight, TimeToCompleteCrouch(StandingHeight, GetCurrentHeight(), CrouchingSpeedPerUnit));
				}
				if (wantedStandingState == Enums.EStandingType.Prone)
				{
					CurrentRunningTween = HeightChangeTween(Tween.TransitionType.Expo, CrouchHeight, TimeToCompleteCrouch(ProneHeight, GetCurrentHeight(), CrouchToProneSpeed));
					CurrentRunningTween.Finished += () =>
					{
						JumpNode(false, PlayerCharacter);
						JumpNode(true, HeadNode);
						BodyCollision.Disabled = true;
						GD.Print("Finished into prone");
					};
				}
				break;
			case Enums.EStandingType.Prone:
				JumpNode(true, PlayerCharacter);
				JumpNode(false, HeadNode);
				CurrentRunningTween = HeightChangeTween(Tween.TransitionType.Spring, HeadRestingSpace, TimeToCompleteCrouch(HeadRestingSpace, CrouchHeight, CrouchToProneSpeed));
				AttemptingMove = Enums.EStandingType.Crouching;

				BodyCollision.Disabled = false;
				if (wantedStandingState == Enums.EStandingType.Standing)
				{
					CurrentRunningTween.StepFinished += (long step) =>
					{

					};
					CurrentRunningTween.StepFinished += (long step) => { AttemptingMove = Enums.EStandingType.Standing; CurrentStanding = Enums.EStandingType.Crouching; };
					HeightChangeTween(Tween.TransitionType.Spring, StandingHeight, TimeToCompleteCrouch(StandingHeight, CrouchHeight, CrouchingSpeedPerUnit), CurrentRunningTween);
				}
				break;
			default:
				break;
		}
		CurrentRunningTween.SetProcessMode(Tween.TweenProcessMode.Physics);
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
}
