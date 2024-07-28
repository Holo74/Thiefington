using Godot;
using System;

[GlobalClass]
public partial class RotationAndClamp : Resource
{

    public float CurrentRotation { get; set; } = 0f;
    public float maxRotation = 0f;
    public float minRotation = 0f;
    [Export]
    public bool RotationLockOn { get; set; }
    [Export(PropertyHint.Range, "1, 100, .1")]
    public float Sensitivity { get; set; }
    [Export]
    public float MaxRotation
    {
        get
        {
            return maxRotation * 180.0f / MathF.PI;
        }
        set
        {
            maxRotation = value / 180.0f * MathF.PI;
        }
    }
    [Export]
    public float MinRotation
    {
        get
        {
            return minRotation * 180.0f / MathF.PI;
        }
        set
        {
            minRotation = value / 180.0f * MathF.PI;
        }
    }

    public float RotateAmount(float attemptRotateAmount)
    {
        attemptRotateAmount *= Sensitivity;
        if (!RotationLockOn)
            return attemptRotateAmount;
        float rotateAmount = Math.Clamp(CurrentRotation + attemptRotateAmount, minRotation, maxRotation) - CurrentRotation;
        CurrentRotation += rotateAmount;
        return rotateAmount;
    }
}
