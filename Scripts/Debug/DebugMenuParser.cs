using Godot;
using Godot.Collections;
using System;
using System.ComponentModel;

public partial class DebugMenuParser : Node
{
	public static DebugMenuParser PARSER { get; private set; }
	public override void _EnterTree()
	{
		base._EnterTree();
		PARSER = this;
	}
	public static System.Collections.Generic.Dictionary<string, Func<string[], string>> PARSING = new System.Collections.Generic.Dictionary<string, Func<string[], string>>()
	{
		{"set" , SETTING_PROPERTY},
		{"get_all_properties", GET_ALL_PROPERTIES},
		{"get_property", GET_PROPERTY_VALUE}
	};

	private static string GET_PROPERTY_VALUE(string[] args)
	{
		string error = "";
		if (args.Length != 3)
		{
			return "Usage: get_property [target] [property]";
		}
		Node target = GET_TARGET(args[1], out error);
		if (error.Length > 1)
		{
			return error;
		}
		if (!CONTAINS_PROPERTY_NAME(args[2], target))
		{
			return string.Format("{0} does not contain property path {1}", target.Name, args[2]);
		}
		return target.Get(args[2]).ToString();
	}

	private static string GET_ALL_PROPERTIES(string[] args)
	{
		string error = "";
		if (args.Length != 2)
		{
			return "Usage: get_all_properties [target]";
		}
		Node target = GET_TARGET(args[1], out error);
		if (error.Length > 1)
		{
			return error;
		}
		string names = "";
		foreach (Dictionary prop in target.GetPropertyList())
		{
			names = names + prop["name"].ToString() + " | ";
		}
		if (error.Length > 1)
		{
			return error;
		}
		return names.ToString();

	}

	// the format for this would be set [target] [property] [value]
	private static string SETTING_PROPERTY(string[] args)
	{
		if (args.Length < 4)
		{
			return "Usage: set [target] [property] [value]";
		}
		string targetErrorReport = "";
		Node target = GET_TARGET(args[1], out targetErrorReport);
		string propertyPath = args[2];
		Variant propertySet = CONVERT_ARGS_TO_VARIANT(GET_ARGUMENTS_ALONE(args, 3, out targetErrorReport));

		if (targetErrorReport.Length > 1)
		{
			return targetErrorReport;
		}

		if (!CONTAINS_PROPERTY_NAME(propertyPath, target))
		{
			return string.Format("{0} does not contain property path {1}", target.Name, propertyPath);
		}

		target.Set(propertyPath, propertySet);

		return string.Format("Set property: {0} to {1} on {2}", propertyPath, propertySet, target.Name);
	}

	private static bool CONTAINS_PROPERTY_NAME(string name, Node target)
	{
		foreach (Dictionary property in target.GetPropertyList())
		{
			if (property["name"].ToString().Equals(name))
			{
				return true;
			}
		}
		return false;
	}

	// Getting to the end would require you to just leave length alone.  I don't think we should ever reach that amount of arguments
	private static string[] GET_ARGUMENTS_ALONE(string[] args, int start, out string error, int length = 1000)
	{
		error = "";
		length = Math.Min(args.Length - start, length);
		if (length < 1)
		{
			error = "No arguments provided";
			return null;
		}
		string[] arguments = new string[length];
		for (int i = 0; i < length; i++)
		{
			arguments[i] = args[i + start];
		}
		return arguments;
	}

	private static object CONVERT_VALUE_TO(string val)
	{
		float outputF;
		if (float.TryParse(val, out outputF))
		{
			return outputF;
		}
		return val;
	}

	private static Variant CONVERT_ARGS_TO_VARIANT(string[] args)
	{
		if (args.Length == 1)
		{
			Variant single_out = (Variant)CONVERT_VALUE_TO(args[0]);
		}
		Variant outputing = new Variant();
		return outputing;
	}

	private static Node GET_TARGET(string arg, out string error)
	{
		error = "";
		GodotObject targetted;
		if (arg.Equals("player"))
		{
			GD.Print(Player.PLAYER.GetInstanceId());
			return Player.PLAYER;
		}
		try
		{
			targetted = InstanceFromId(ulong.Parse(arg));
		}
		catch (System.OverflowException)
		{
			error = "ID Overflow";
			return null;
		}
		catch (System.FormatException)
		{
			error = "ID Not formatted correctly";
			return null;
		}
		catch (System.ArgumentNullException)
		{
			error = "ID is null";
			return null;
		}
		if (targetted is null)
		{
			error = "Target is null";
			return null;
		}
		Node ouputingTarget = (Node)targetted;
		if (ouputingTarget is null)
		{
			error = "Conversion of target to node failed";
			return null;
		}
		return (Node)targetted;

	}
}
