using System.Collections.Generic;
using Editor.Interfaces;

namespace Editor.DataClasses;

public class SuperRegion(string name) : IProvinceCollection
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

   public int[] GetProvinceIds()
   {
      var provinces = new List<int>();
      foreach (var region in Regions) 
         provinces.AddRange(Data.Regions[region].GetProvinceIds());
      return provinces.ToArray();
   }

   public IProvinceCollection? ScopeOut()
   {
      return null;
   }

   public List<IProvinceCollection> ScopeIn()
   {
      var regions = new List<IProvinceCollection>();
      foreach (var region in Regions)
      {
         regions.Add(Data.Regions[region]);
      }
      return regions;
   }
}