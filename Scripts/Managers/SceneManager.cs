using Godot;
using System;
using System.Collections.Generic;

public partial class SceneManager : Node
{



	public void LoadScene(string path)
	{
		GetTree().ChangeSceneToFile(path);
	}

}
