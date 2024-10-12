using System.Collections.Generic;
using System.Drawing;
using Editor.Events;
using Editor.Interfaces;

namespace Editor.DataClasses.GameDataClasses;
#nullable enable
public class SuperRegion(string name) : IProvinceCollection
{
   public string Name { get; } = name;
   private List<string> _regions { get; set; } = [];
   public Color Color { get; set; }

   public List<string> Regions => _regions;

   public SuperRegion(string name, List<string> regions) : this(name)
   {
      foreach (var region in regions)
         AddRmvRegion(region, true);
   }

   public void AddRegion(string region)
   {
      AddRmvRegion(region, true);
   }

   public void RemoveRegion(string region)
   {
      AddRmvRegion(region, false);
   }

   private void AddRmvRegion(string regionName, bool add)
   {
      if (add)
      {
         if (!Regions.Contains(regionName))
            _regions.Add(regionName);
      }
      else
         _regions.Remove(regionName);

      if (Globals.State == State.Running)
         if (Globals.Regions.TryGetValue(regionName, out var region))
            foreach (var id in region.GetProvinces())
               ProvinceEventHandler.RaiseSuperRegionRegionChanged(id, region, nameof(Regions));
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
         if (Globals.Regions.TryGetValue(region, out Region? value))
            provinces.AddRange(value.GetProvinceIds());
      return [.. provinces];
   }

   public ICollection<Province> GetProvinces()
   {
      var provinces = new List<Province>();
      foreach (var region in Regions)
         if (Globals.Regions.TryGetValue(region, out Region? value))
            provinces.AddRange(value.GetProvinces());
      return provinces;
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
         regions.Add(Globals.Regions[region]);
      }
      return regions;
   }
}