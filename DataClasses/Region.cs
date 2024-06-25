using System.Collections.Generic;
using System.Data;
using System.Runtime.InteropServices;
using Editor.Interfaces;

namespace Editor.DataClasses;

public class Region(string name) : IProvinceCollection
{
   public string Name { get; } = name;
   public List<string> Areas { get; set; } = [];
   public List<Monsoon> Monsoon { get; set; } = [];
   public string SuperRegion { get; set; } = string.Empty;

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

   public int[] GetProvinceIds()
   {
      var provinces = new List<int>();
      foreach (var area in Areas) 
         provinces.AddRange(Data.Areas[area].GetProvinceIds());
      return provinces.ToArray();
   }

   public IProvinceCollection ScopeOut()
   {
      return Data.SuperRegions[SuperRegion];
   }

   public List<IProvinceCollection> ScopeIn()
   {
      var areas = new List<IProvinceCollection>();
      foreach (var area in Areas)
      {
         areas.Add(Data.Areas[area]);
      }
      return areas;
   }
}

public class Monsoon(string start, string end)
{
   public string Start { get; set; } = start;
   public string End { get; set; } = end;
}