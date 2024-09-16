using System.Runtime.CompilerServices;
using System.Text;
using ABI.Windows.Media.Playback;
using Editor.Parser;

namespace Editor.DataClasses.GameDataClasses
{
   public enum EffectValueType
   {
      Int,
      Float,
      String,
      Bool,
      Complex
   }

   public enum Scope
   {
      Country,
      Province,
      Ruler
   }

   public static class EffectFactory
   {
      public static Effect CreateSimpleEffect(string name, string value, EffectValueType type, Scope scope)
      {
         return new SimpleEffect(name, value, type, scope);
      }

      public static ComplexEffect CreateComplexEffect(string name, string value, EffectValueType type)
      {
         return name.ToLower() switch
         {
            "revolt" => new RevoltEffect(name, value, type, Scope.Province),
            "dummy" => new DummyComplexEffect(name, value, type, Scope.Country),
            _ => new DummyComplexEffect(name, value, type, Scope.Country)
         };
      }
   }

   public abstract class Effect(string name, string value, EffectValueType type, Scope scope)
   {
      public string Name { get; set; } = name.ToLower();
      public string Value { get; set; } = value;
      public EffectValueType ValueType { get; set; } = type;
      public virtual bool IsComplex => false;
      public Scope Scope { get; set; } = scope;

      public virtual string GetEffectString(int tabs)
      {
         var str = string.Empty;
         for (var i = 0; i < tabs; i++) 
            str += "\t";
         return $"{str}{Name} = {Value}";
      }

      public virtual bool ExecuteProvince(Province province)
      {
         if (EffectParser.IsAnyEffect(name) || Globals.Buildings.Contains(new (name)) || Globals.UniqueAttributeKeys.Contains(name))
         {
            // TODO: This is a weird fix and should be removed in the future
            if (name.Equals("city"))
            {
               province.SetAttribute(ProvAttrSet.is_city, Value);
               return true;
            }
            province.SetAttribute(Name, Value);
            return true;
         }
         Globals.ErrorLog.Write($"Could not execute effect: {Name}");
         return false;
      }

      public virtual bool ExecuteCountry(Country country)
      {
         //TODO implement this
         return false;
      }

      public override string ToString()
      {
         return $"{Name} : {Value}";
      }

      //create an effect.Empty
      public static Effect Empty => new SimpleEffect(string.Empty, string.Empty, EffectValueType.String, Scope.Country);

      public bool IsEmpty()
      {
         return this == Empty;
      }
   }

   public class SimpleEffect(string name, string value, EffectValueType type, Scope scope) : Effect(name, value, type, scope);
   public class DummyComplexEffect(string name, string value, EffectValueType type, Scope scope) : ComplexEffect(name, value, type, scope);


   public abstract class ComplexEffect(string name, string value, EffectValueType type, Scope scope) : Effect(name, value, type, scope)
   {
      public List<Effect> Effects { get; set; } = [];
      public override bool IsComplex => true;

      public override string GetEffectString(int tabs)
      {
         var sb = new StringBuilder();
         sb.AppendLine($"\t{Name} = {{");
         foreach (var effect in Effects)
         {
            sb.AppendLine($"\t{effect.GetEffectString(1)}");
         }
         sb.AppendLine("\t}");
         return sb.ToString();
      }
      //TODO trigger?
   }

   public class ScriptedEffect(string name, string value, EffectValueType type, Scope scope) : ComplexEffect(name, value, type, scope)
   {
      public List<KeyValuePair<string, string>> AttributesList { get; set; } = [];
      public override bool ExecuteProvince(Province province)
      {
         //TODO implement this
         return true;
      }

      public override string ToString()
      {
         return $"{Name} : {Value} : {Effects.Count}";
      }
   }

   /// <summary>
   /// This is used to represent all effects that are parsed from common/scripted_effects/*.txt files but only their names are truly relevant
   /// </summary>
   /// <param name="name"></param>
   /// <param name="value"></param>
   /// <param name="type"></param>
   public class DummyScriptedEffect(string name, string value, EffectValueType type) : Effect(name, value, type, Scope.Country)
   {
      public override string GetEffectString(int tabs)
      {
         return $"{name} = {{\n\t{value}\n}}";
      }

      public override string ToString()
      {
         return $"{Name} : {Value}";
      }
   }

   public class RevoltEffect(string name, string value, EffectValueType type, Scope scope) : ComplexEffect(name, value, type, scope)
   {
      public bool RemovesRevolt => string.IsNullOrWhiteSpace(value);
      // TODO parse remaingin parameters
      public override bool ExecuteProvince(Province province)
      {
         if (RemovesRevolt)
         {
            province.HasRevolt = false;
            return true;
         }
         province.HasRevolt = true;
         return true;
      }
   }

   public class SpawnRebelsEffect(string name, string value, EffectValueType type, Scope scope)
      : ComplexEffect(name, value, type, scope)
   {

      public string RebelType { get; init; } = string.Empty;
      public int RebelSize { get; init; }

      // optionals
      public string Culture { get; set; } = string.Empty;
      public string Religion { get; set; } = string.Empty;
      public string Leader { get; set; } = string.Empty;
      public string LeaderDynasty { get; set; } = string.Empty;
      public string Estate {get; set; } = string.Empty;
      public int Unrest {get; set; }
      public bool Win { get; set; } = false;
      public bool Female { get; set; } = false;
      public bool UseHeirAsLeader { get; set; } = false;
      public bool UseConsortAsLeader { get; set; } = false;
      public bool AsIfFaction { get; set; } = false;
      public bool ShouldTakeCapital { get; set; } = false;
      public Tag SeparatistTarget { get; set; } = Tag.Empty; // TODO improve the TAG implementation to allow for scopes
      public Tag Friend { get; set; } = Tag.Empty;

      public override string ToString()
      {
         return $"{RebelType} : {RebelSize}";
      }

      public override bool Equals(object? obj)
      {
         if (obj is SpawnRebelsEffect other)
            return RebelType == other.RebelType && RebelSize == other.RebelSize;
         return false;
      }

      public override int GetHashCode()
      {
         return RebelType.GetHashCode() ^ RebelSize.GetHashCode();
      }
   }
}