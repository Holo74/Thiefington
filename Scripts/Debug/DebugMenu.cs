using Godot;
using System;

public partial class DebugMenu : Control
{
	public override void _Ready()
	{
		DEBUG_MENU = this;
	}
	public static DebugMenu DEBUG_MENU { get; private set; }
	[Export]
	private RichTextLabel Output { get; set; }
	// Try and make a prior command thing.  We can either use a linked list of an array.  Up to you.
	private string EnterCommand(string command)
	{
		return "Error: reason";
	}

	private void HandleCommandEnter(string text)
	{

	}

	public void ToggleConsole(bool state)
	{
		Visible = state;
	}
}
