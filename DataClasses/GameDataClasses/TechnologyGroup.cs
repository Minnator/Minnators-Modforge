﻿namespace Editor.DataClasses.GameDataClasses
{
   public class TechnologyGroup(string name)
   {
      public string Name { get; } = name;
      public int StartLevel {get; set; } = 0;
      public float StarCostModifier { get; set; } = 0f;

      public static TechnologyGroup Empty => new(string.Empty);

      public override int GetHashCode()
      {
         return Name.GetHashCode();
      }

      public override bool Equals(object? obj)
      {
         if (obj is not TechnologyGroup group)
            return false;
         return group.Name == Name;
      }

      public override string ToString()
      {
         return Name;
      }
   }
}