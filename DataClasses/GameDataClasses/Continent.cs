using System.Collections.Generic;
using System.Drawing;
using Editor.Interfaces;

namespace Editor.DataClasses.GameDataClasses;
#nullable enable
public class Continent(string name, List<Province> provinces) : IProvinceCollection
{
   public string Name { get; } = name;
   public List<Province> Provinces { get; set; } = provinces;
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
      var ids = new int[Provinces.Count];
      for (var i = 0; i < Provinces.Count; i++)
      {
         ids[i] = Provinces[i].Id;
      }
      return ids;
   }

   public ICollection<Province> GetProvinces()
   {
      return Provinces;
   }

   public List<IProvinceCollection> ScopeIn()
   {
      var provs = new List<IProvinceCollection>();
      foreach (var province in Provinces)
      {
         provs.Add(province);
      }
      return provs;
   }

   public IProvinceCollection? ScopeOut()
   {
      return null;
   }
}