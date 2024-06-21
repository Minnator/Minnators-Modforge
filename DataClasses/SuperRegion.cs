using System.Collections.Generic;

namespace Editor.DataClasses;

public class SuperRegion(string name)
{
   public string Name { get; } = name;
   public List<string> Regions { get; set; } = [];

   public SuperRegion (string name, List<string> regions) : this(name)
   {
      Regions = regions;
   }

   public override bool Equals(object? obj)
   {
      if (obj is SuperRegion other)
         return Name == other.Name;
      return false;
   }

   public override int GetHashCode()
   {
      return Name.GetHashCode();
   }
}