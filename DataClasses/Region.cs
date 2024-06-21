using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Editor.DataClasses;

public class Region(string name)
{
   public string Name { get; } = name;
   public List<string> Areas { get; set; } = [];
   public List<Monsoon> Monsoon { get; set; } = [];

   public Region (string name, List<string> areas) : this(name)
   {
      Areas = areas;
   }

   public Region (string name, List<string> areas, List<Monsoon> monsoon) : this(name, areas)
   {
      Monsoon = monsoon;
   }

   public override bool Equals(object? obj)
   {
      if (obj is Region other)
         return Name == other.Name;
      return false;
   }

   public override int GetHashCode()
   {
      return Name.GetHashCode();
   }
}

public class Monsoon(string start, string end)
{
   public string Start { get; set; } = start;
   public string End { get; set; } = end;
}