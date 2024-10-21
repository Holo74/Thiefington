using Godot;
using System;
using System.Reflection;

[GlobalClass]
public partial class PlayerVariables : Resource
{
    [Export]
    public float GravityValue { get; set; }
    [Export]
    public float JumpStrength { get; set; }
    [Export]
    public float StandingSpeed { get; set; }
    [Export]
    public float CrouchingSpeed { get; set; }
    [Export]
    public float CrawlingSpeed { get; set; }
    [Export]
    public Vector2 MouseRotationMult { get; set; }

    public override string ToString()
    {
        string outputString = "Player Variables:\n";
        foreach (var prop in GetType().GetProperties())
        {
            outputString += string.Format("{0}: {1}, ", prop.Name, prop.GetValue(this));
        }
        GetType().GetProperties();
        outputString = outputString.Substring(0, outputString.Length - 2);
        GD.Print(outputString);
        return outputString;
    }

    public override Variant _Get(StringName property)
    {
        string outputString = "";
        foreach (var prop in GetType().GetProperties())
        {
            outputString += string.Format("{0}: {1}, ", prop.Name, prop.GetValue(this));
        }
        GetType().GetProperties();
        outputString = outputString.Substring(0, outputString.Length - 2);
        GD.Print(outputString);
        return outputString;
    }

    public override bool _Set(StringName property, Variant value)
    {
        GD.Print(property);
        foreach (var prop in this.GetType().GetProperties())
        {
            if (prop.Name.Equals(property))
            {
                GD.Print(prop.Name);
                prop.SetValue(this, value);
                return true;
            }
        }
        return false;
    }
}
