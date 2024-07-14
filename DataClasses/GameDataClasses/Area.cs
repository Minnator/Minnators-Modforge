using Editor.Interfaces;

namespace Editor.DataClasses.GameDataClasses;
#nullable enable
public class Area(string name, int[] provinces, Color color) : IProvinceCollection
{
   public string Name { get; } = name;
   // Contains the provinces in the area will be editable as the array is only a few elements long
   public int[] Provinces { get; set; } = provinces;
   public string Edict { get; set; } = string.Empty;
   public float Prosperity { get; set; } = 0;
   public bool IsStated { get; set; } = false;
   public string Region { get; set; } = string.Empty;
   public Color Color { get; set; } = color;

   public override bool Equals(object? obj)
   {
      if (obj is Area other)
         return Name == other.Name;
      return false;
   }

   public override int GetHashCode()
   {
      return Name.GetHashCode();
   }

   public int[] GetProvinceIds()
   {
      return Provinces;
   }

   public IProvinceCollection ScopeOut()
   {
      return Globals.Regions[Region];
   }

   public List<IProvinceCollection> ScopeIn()
   {
      var provs = new List<IProvinceCollection>();
      foreach (var province in Provinces)
         provs.Add(Globals.Provinces[province]);
      return provs;
   }
}