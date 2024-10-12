using Godot;
using System;

public partial class GameManager : Node
{
	public static GameManager GAME_MANAGER { get; private set; }
	public override void _EnterTree()
	{
		base._EnterTree();
		GAME_MANAGER = this;
	}
}
