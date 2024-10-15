using Godot;
using System;

public interface IMovement
{
    void Init(MovementNode parent);
    void Kill();
    void Processing(double delta);
    Vector3 LeftMove(CharacterBody3D body);
    Vector3 RightMove(CharacterBody3D body);
    Vector3 ForwardMove(CharacterBody3D body);
    Vector3 BackwardMove(CharacterBody3D body);
    Vector3 Jump(CharacterBody3D body);
    void CrouchSignal(Enums.EStandingType crouchType);
}
