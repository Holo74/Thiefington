using Godot;
using System;

public class FMaxAndMinTween
{
	public float Max { get; set; }
	public float Min { get; set; }
	private Tween ClassTween { get; set; }
	private string PropertyPath { get; set; }
	private GodotObject TweenObject { get; set; }
	private double MaxDuration { get; set; }

	public void StartTransition(bool ToMax)
	{
		double duration = MaxDuration;
		if (ClassTween.IsRunning())
		{
			duration -= ClassTween.GetTotalElapsedTime();
			ClassTween.Kill();
		}
		ClassTween = GameManager.GAME_MANAGER.GetTree().CreateTween();
		ClassTween.TweenProperty(TweenObject, PropertyPath, ToMax ? Max : Min, duration);
		ClassTween.SetEase(Tween.EaseType.InOut);
		ClassTween.SetTrans(Tween.TransitionType.Spring);
	}
}
