using Godot;
using Godot.Collections;
using Microsoft.VisualBasic;
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
		{"get_properties", GET_ALL_PROPERTIES},
		{"get_properties_debug", GET_ALL_PROPERTIES_DEBUG},
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

	private static string GET_ALL_PROPERTIES_DEBUG(string[] args)
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
		return target.GetPropertyList().ToString();

	}

	private static string GET_ALL_PROPERTIES(string[] args)
	{
		string error = "";
		if (args.Length < 2 || args.Length > 4)
		{
			return "Usage: get_properties [target] <property>";
		}
		GodotObject target = GET_TARGET(args[1], out error);
		if (error.Length > 1)
		{
			return error;
		}
		if (args.Length == 3)
		{
			Variant holder = target.Get(args[2]);
			if (holder.VariantType == Variant.Type.Nil)
			{
				return string.Format("Property {0} returns null", args[2]);
			}

			if (target is null)
			{
				return string.Format("Path {0} could not be gotten", args[2]);
			}
			return holder.ToString();
		}
		string names = "";
		foreach (Dictionary prop in target.GetPropertyList())
		{
			names = names + prop["name"].ToString() + " " + ((Variant.Type)prop["type"].AsInt64()) + " | ";
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
		if (propertyPath.Contains('.'))
		{
			GodotObject holder = target.Get(propertyPath.Substring(0, propertyPath.LastIndexOf('.'))).AsGodotObject();
		}
		target.Set(propertyPath, propertySet);

		return string.Format("Set property: {0} to {1} on {2}", propertyPath, propertySet, target.Name);
	}

	private static bool CONTAINS_PROPERTY_NAME(string name, GodotObject target)
	{
		Variant v = target.Get("name");
		if (v.VariantType == Variant.Type.Nil)
		{
			return false;
		}
		return true;
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

	private static Variant CONVERT_VALUE_TO(string val)
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
			Variant single_out = CONVERT_VALUE_TO(args[0]);
			return single_out;
		}
		if (args.Length == 3)
		{
			float[] temp = new float[3];
			bool isNumb = true;

			for (int i = 0; i < args.Length && isNumb; i++)
			{
				isNumb = float.TryParse(args[i], out temp[i]);
			}
			if (isNumb)
			{
				Vector3 vec3 = new Vector3(temp[0], temp[1], temp[2]);
				return vec3;
			}
		}
		Godot.Collections.Array arr = new Godot.Collections.Array();
		foreach (string arg in args)
		{
			arr.Add(CONVERT_VALUE_TO(arg));
		}
		return arr;
	}

	private static Node GET_TARGET(string arg, out string error)
	{
		error = "";
		GodotObject targetted;
		if (arg.Equals("player"))
		{
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
