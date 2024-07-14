using System.Collections.Generic;
using System.Drawing;
using Editor.Interfaces;

namespace Editor.DataClasses.GameDataClasses;
#nullable enable
public class Continent(string name, List<int> provinces) : IProvinceCollection
{
   public string Name { get; } = name;
   public List<int> Provinces { get; set; } = provinces;
   public Color Color { get; set; }

   public override bool Equals(object? obj)
   {
      if (obj is Continent other)
         return Name == other.Name;
      return false;
   }

   public override int GetHashCode()
   {
      return Name.GetHashCode();
   }

   public int[] GetProvinceIds()
   {
      return Provinces.ToArray();
   }

   public List<IProvinceCollection> ScopeIn()
   {
      var provs = new List<IProvinceCollection>();
      foreach (var province in Provinces)
      {
         provs.Add(Globals.Provinces[province]);
      }
      return provs;
   }

   public IProvinceCollection? ScopeOut()
   {
      return null;
   }
}