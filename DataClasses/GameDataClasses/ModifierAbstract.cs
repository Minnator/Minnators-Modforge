using System.Text;
using Editor.Savers;

namespace Editor.DataClasses.GameDataClasses
{

   public enum ModifierType
   {
      ProvinceModifier,
      ProvinceTriggeredModifier,
      PermanentProvinceModifier,
      CountryModifier,
      TriggeredModifier,
   }
   public enum ModifierValueType
   {
      Bool,
      Int,
      Float,
      Percentile
   }
   [Flags]
   public enum ValueMarkDown
   {
      Positive = 1,
      Negative = 2,
      Neutral = 4
   }

   public static class ModifiersColorDefinition
   {
      public static Color Positive = Color.Green;
      public static Color Negative = Color.Red;
      public static Color Neutral = Color.DarkGoldenrod;
   }


   public abstract class ModifierAbstract : ISaveModifier
   {
      public string Name { get; protected init; }
      public int Duration { get; set; }
      public string Icon { get; set; } = string.Empty;
      public List<KeyValuePair<string, string>> Effects { get; set; } = [];

      public string GetModifierString()
      {
         return ToString();
      }

      public override bool Equals(object? obj)
      {
         if (obj is ModifierAbstract modifier)
            return Name == modifier.Name && Duration == modifier.Duration;
         return false;
      }

      public override int GetHashCode()
      {
         return Name.GetHashCode();
      }

      public override string ToString()
      {
         return $"{Name}";
      }
   }

   public partial class ApplicableModifier : ModifierAbstract
   {
      public ApplicableModifier(string name, int duration)
      {
         Name = name;
         Duration = duration;
      }
      
      public override bool Equals(object? obj)
      {
         if (obj is ApplicableModifier modifier)
            return Name == modifier.Name && Duration == modifier.Duration ;
         return false;
      }

      public override int GetHashCode()
      {
         return Name.GetHashCode();
      }

      public override string ToString()
      {
         return $"{Name}";
      }
   }

   public class TradeModifier : ModifierAbstract
   {
      public Tag Who { get; set; }
      public int Power { get; set; }
      public string Key { get; set; } = string.Empty;

      public TradeModifier()
      {
      }

      public TradeModifier(Tag who, int power, string key, int duration) : this()
      {
         Who = who;
         Power = power;
         Key = key;
         Duration = duration;
      }
      
      public override bool Equals(object? obj)
      {
         if (obj is TradeModifier modifier)
            return Name == modifier.Name && Duration == modifier.Duration ;
         return false;
      }

      public override int GetHashCode()
      {
         return Name.GetHashCode();
      }

      public override string ToString()
      {
         return $"{Name}";
      }
   }

   public class RulerModifier : ISaveModifier
   {
      public string Name { get; set; } = string.Empty;
      public bool IsHidden = false;

      public override string ToString()
      {
         return $"{Name}";
      }

      public string GetModifierString()
      {
         var sb = new StringBuilder();
         sb.AppendLine("add_ruler_modifier = {");
         sb.AppendLine($"\tname = {Name}");
         sb.AppendLine($"\thidden = {SavingUtil.GetYesNo(IsHidden)}");
         sb.AppendLine($"}}");
         return sb.ToString();
      }

      public override bool Equals(object? obj)
      {
         if (obj is RulerModifier modifier)
            return Name == modifier.Name;
         return false;
      }

      public override int GetHashCode()
      {
         return Name.GetHashCode();
      }
   }

   public class EventModifier(string name)
   {
      public string Name { get; } = name;
      public List<KeyValuePair<string, string>> TriggerAttribute { get; set; } = [];
      public List<Modifier> Modifiers { get; set; } = [];
      public string Picture { get; set; } = string.Empty;

      public string GetFormattedString()
      {
         var sb = new StringBuilder();
         sb.AppendLine($"{Name} = {{");
         foreach (var attr in TriggerAttribute)
            sb.AppendLine($"\t{attr} = yes");
         foreach (var mod in Modifiers)
            sb.AppendLine($"\t{mod.Name} = {mod.Value}");
         sb.AppendLine("}");
         return sb.ToString();
      }

      public override bool Equals(object? obj)
      {
         if (obj is EventModifier modifier)
            return Name == modifier.Name;
         return false;
      }

      public override int GetHashCode()
      {
         return Name.GetHashCode();
      }

      public override string ToString()
      {
         return $"\"{Name}\" ({Modifiers.Count})";
      }

      public static EventModifier Empty => new("Empty");
   }

   public struct ModifierDefinition(string codeName, int index, ModifierValueType type, ValueMarkDown markDown) : IEquatable<ModifierValueType>
   {
      public string CodeName = codeName;
      public int Index = index;
      public ModifierValueType Type = type;
      public ValueMarkDown MarkDown = markDown;

      public Color GetColor(object value)
      {
         if (value is bool b)
         {
            if ((MarkDown & ValueMarkDown.Positive) == ValueMarkDown.Positive && b)
               return ModifiersColorDefinition.Positive;
            if ((MarkDown & ValueMarkDown.Negative) == ValueMarkDown.Negative && !b)
               return ModifiersColorDefinition.Negative;
            return ModifiersColorDefinition.Neutral;
         }

         if (value is float f)
         {
            if ((MarkDown & ValueMarkDown.Positive) == ValueMarkDown.Positive && f > 0)
               return ModifiersColorDefinition.Positive;
            if ((MarkDown & ValueMarkDown.Negative) == ValueMarkDown.Negative && f < 0)
               return ModifiersColorDefinition.Negative;
            return ModifiersColorDefinition.Neutral;
         }

         if (value is int i)
         {
            if ((MarkDown & ValueMarkDown.Positive) == ValueMarkDown.Positive && i > 0)
               return ModifiersColorDefinition.Positive;
            if ((MarkDown & ValueMarkDown.Negative) == ValueMarkDown.Negative && i < 0)
               return ModifiersColorDefinition.Negative;
            return ModifiersColorDefinition.Neutral;
         }

         return ModifiersColorDefinition.Neutral;
      }
      public override string ToString()
      {
         return $"{CodeName}";
      }
      public bool Equals(ModifierValueType other)
      {
         return Type == other;
      }
      public override bool Equals(object? obj)
      {
         return obj is ModifierDefinition other && Equals(other);
      }
      public override int GetHashCode()
      {
         return Index;
      }
      public static bool operator ==(ModifierDefinition a, ModifierDefinition b)
      {
         return a.Index == b.Index;
      }
      public static bool operator !=(ModifierDefinition a, ModifierDefinition b)
      {
         return !(a == b);
      }
   }

   public class Modifier(int nameIndex, object value) 
   {
      public readonly int Name = nameIndex;
      public object Value = value;
      public readonly Scope Scope = Scope.Country;


      public Modifier(int nameIndex, object value, Scope scope) : this(nameIndex, value)
      {
         Scope = scope;
      }

      public override string ToString()
      {
         return $"{Name} : {Value}";
      }

      public override bool Equals(object? obj)
      {
         if (obj is Modifier other)
            return this == other;
         return false;
      }

      public override int GetHashCode()
      {
         return Name.GetHashCode();
      }

      public static bool operator ==(Modifier a, Modifier b)
      {
         return a.Name == b.Name && a.Value == b.Value;
      }

      public static bool operator !=(Modifier a, Modifier b)
      {
         return !(a == b);
      }

      public bool TryGetValue<T>(out T value)
      {
         if (Value is T typedValue)
         {
            value = typedValue;
            return true;
         }
         value = default!;
         return false;
      }

   }

   public interface ISaveModifier
   {
      public string GetModifierString();
   }
}