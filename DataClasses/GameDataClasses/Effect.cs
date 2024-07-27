using System.Diagnostics;

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

   public static class EffectFactory
   {
      public static Effect CreateSimpleEffect(string name, string value, EffectValueType type)
      {
         return new SimpleEffect(name, value, type);
      }

      public static ComplexEffect CreateComplexEffect(string name, string value, EffectValueType type)
      {
         return name.ToLower() switch
         {
            "revolt" => new RevoltEffect(name, value, type),
            "dummy" => new DummyComplexEffect(name, value, type),
            _ => new DummyComplexEffect(name, value, type)
         };
      }
   }

   public abstract class Effect(string name, string value, EffectValueType type)
   {
      public string Name { get; set; } = name.ToLower();
      public string Value { get; set; } = value;
      public EffectValueType ValueType { get; set; } = type;
      public virtual bool IsComplex => false;

      public virtual bool ExecuteProvince(Province province)
      {
         if (Globals.UniqueAttributeKeys.Contains(name) || Globals.Effects.Contains(name) || Globals.Buildings.Contains(new (name)))
         {
            province.SetAttribute(Name, Value);
            return true;
         }
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
   }

   public class SimpleEffect(string name, string value, EffectValueType type) : Effect(name, value, type);
   public class DummyComplexEffect(string name, string value, EffectValueType type) : ComplexEffect(name, value, type);

   public abstract class ComplexEffect(string name, string value, EffectValueType type) : Effect(name, value, type)
   {
      public List<Effect> Effects { get; set; } = [];
      //TODO trigger?
   }

   public class RevoltEffect(string name, string value, EffectValueType type) : ComplexEffect(name, value, type)
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
}