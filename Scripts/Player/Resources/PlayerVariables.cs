using Godot;
using System;

[GlobalClass]
public partial class PlayerVariables : Resource
{
    [Export]
    public float GravityValue { get; set; }
    [Export]
    public float JumpStrength { get; set; }
    [Export]
    public float Speed { get; set; }

    public override string ToString()
    {
        return string.Format("PlayerVariables");
    }

    public override Variant _Get(StringName property)
    {
        return string.Format("GravityValue: {0}, JumpStrength: {1}, Speed: {2}", GravityValue, JumpStrength, Speed);
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
