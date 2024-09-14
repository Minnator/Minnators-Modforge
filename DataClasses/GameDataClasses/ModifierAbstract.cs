﻿namespace Editor.DataClasses.GameDataClasses
{

   public enum ModifierType
   {
      ProvinceModifier,
      ProvinceTriggeredModifier,
      PermanentProvinceModifier,
      CountryModifier,
      TriggeredModifier,
   }


   public abstract class ModifierAbstract()
   {
      public string Name { get; protected init; }
      public int Duration { get; set; }
      public string Icon { get; set; } = string.Empty;
      public List<KeyValuePair<string, string>> Effects { get; set; } = [];

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

   public partial class Modifier : ModifierAbstract
   {
      public Modifier(string name, int duration)
      {
         Name = name;
         Duration = duration;
      }
      
      public override bool Equals(object? obj)
      {
         if (obj is Modifier modifier)
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


}