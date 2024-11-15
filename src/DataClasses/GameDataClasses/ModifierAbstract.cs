using System.Globalization;
using System.Text;
using Windows.Devices.Bluetooth.Advertisement;
using Editor.DataClasses.Misc;
using Editor.Helper;
using Editor.Saving;

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
      public string Name { get; protected init; } = string.Empty;
      public int Duration { get; set; }
      public string Icon { get; set; } = string.Empty;
      public List<KeyValuePair<string, string>> Effects { get; set; } = [];

      public abstract string GetModifierString();

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

      public override string GetModifierString()
      {
         var sb = new StringBuilder();
         sb.AppendLine("add_country_modifier = {");
         sb.AppendLine($"\tname = {Name}");
         sb.AppendLine($"\tduration = {Duration}");
         sb.AppendLine($"}}");
         return sb.ToString();
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

      public override string GetModifierString()
      {
         var sb = new StringBuilder();
         sb.AppendLine("add_trade_modifier = {");
         sb.AppendLine($"\twho = {Who}");
         sb.AppendLine($"\tduration = {Duration}");
         sb.AppendLine($"\tpower = {Power}");
         sb.AppendLine($"\tkey = {Key}");
         sb.AppendLine($"}}");
         return sb.ToString();
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
         if (IsHidden)
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

   public class EventModifier : NewSaveable
   {

      public EventModifier(string name, ObjEditingStatus status = ObjEditingStatus.Modified)
      {
         Name = name;
         base.EditingStatus = status;
      }

      public EventModifier(string name, ref NewPathObj path) : this(name, ObjEditingStatus.Unchanged)
      {
         SetPath(ref path);
      }
      
      public string Name { get; }
      public List<KeyValuePair<string, string>> TriggerAttribute { get; set; } = [];
      public List<Modifier> Modifiers { get; set; } = [];
      public string Picture { get; set; } = string.Empty;

      public string GetTitleLocKey => Name;
      public string GetDescriptionLocKey => $"desc_{Name}";

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
      public override SaveableType WhatAmI() => SaveableType.EventModifier;

      public override string[] GetDefaultFolderPath()
      {
         return ["common", "event_modifiers"];
      }

      public override string GetFileEnding()
      {
         return ".txt";
      }

      public override KeyValuePair<string, bool> GetFileName()
      {
         return new("event_modifier", false);
      }

      public override string SavingComment()
      {
         return Localisation.GetLoc(Name);
      }

      public override string GetSaveString(int tabs)
      {
         var sb = new StringBuilder();
         sb.AppendLine($"{Name} = {{");
         foreach (var attr in TriggerAttribute)
            sb.AppendLine($"\t{attr.Key} = yes");
         foreach (var mod in Modifiers)
            sb.AppendLine($"\t{Globals.ModifierKeys[mod.Name]} = {mod.Value}");
         sb.AppendLine("}");
         return sb.ToString();
      }

      public override string GetSavePromptString()
      {
         return $"event_modifier: \"{Name}\"";
      }
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

         if (float.TryParse(value.ToString(), NumberStyles.Float, CultureInfo.InvariantCulture, out var f))
         {
            if ((MarkDown & ValueMarkDown.Positive) == ValueMarkDown.Positive)
               switch (f)
               {
                  case > 0:
                     return ModifiersColorDefinition.Positive;
                  case < 0:
                     return ModifiersColorDefinition.Negative;
               }

            if ((MarkDown & ValueMarkDown.Negative) == ValueMarkDown.Negative)
               switch (f)
               {
                  case > 0:
                     return ModifiersColorDefinition.Negative;
                  case < 0:
                     return ModifiersColorDefinition.Positive;
               }
            return ModifiersColorDefinition.Neutral;
         }

         if (value is int i)
         {
            if ((MarkDown & ValueMarkDown.Positive) == ValueMarkDown.Positive && i > 0)
               return ModifiersColorDefinition.Positive;
            if (i < 0)
               return ModifiersColorDefinition.Negative;
            if ((MarkDown & ValueMarkDown.Negative) == ValueMarkDown.Negative && i > 0)
               return ModifiersColorDefinition.Negative;
            if (i > 0)
               return ModifiersColorDefinition.Positive;
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