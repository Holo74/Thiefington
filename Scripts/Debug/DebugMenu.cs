using Godot;
using System;
using System.Collections.Generic;

public partial class DebugMenu : Control
{
	public override void _Ready()
	{
		DEBUG_MENU = this;
	}
	public static DebugMenu DEBUG_MENU { get; private set; }
	[Export]
	private RichTextLabel Output { get; set; }
	[Export]
	private LineEdit InputCommand { get; set; }

	// Now we just need to set up commands
	// The first should be a setter and getter
	private string EnterCommand(string command)
	{
		string[] command_args = command.Split(' ');
		if (command_args.Length < 1)
		{
			return "No command entered";
		}

		// Switch table default 
		if (!DebugMenuParser.PARSING.ContainsKey(command_args[0].ToLower()))
		{
			return string.Format("There is no command named: {0}", command_args[0]);
		}

		// Pretty much a switch, but it might be faster cause this is just a hash table
		return DebugMenuParser.PARSING[command_args[0].ToLower()](command_args);
	}

	private void HandleCommandEnter(string text)
	{
		GD.Print(text);
		InputCommand.Clear();
		Output.AppendText(text);
		Output.AppendText("\n");
		Output.AppendText(EnterCommand(text));
		Output.AppendText("\n");
	}

	public void ToggleConsole(bool state)
	{
		Visible = state;
		if (Visible)
		{
			InputCommand.GrabFocus();
		}
	}

	public override void _Process(double delta)
	{
		CommandConsole();
	}

	private void CommandConsole()
	{
		if (Input.IsActionJustPressed("CommandLine"))
		{
			GetTree().Paused = !GetTree().Paused;
			ToggleConsole(GetTree().Paused);
		}
	}
}
