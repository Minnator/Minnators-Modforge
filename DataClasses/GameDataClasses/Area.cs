using Editor.Interfaces;

namespace Editor.DataClasses.GameDataClasses;
#nullable enable
public class Area(string name, Province[] provinces, Color color) : IProvinceCollection
{
   public string Name { get; } = name;
   // Contains the provinces in the area will be editable as the array is only a few elements long
   public Province[] Provinces { get; set; } = provinces;
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
      var ids = new int[Provinces.Length];
      for (var i = 0; i < Provinces.Length; i++)
         ids[i] = Provinces[i].Id;
      return ids;
   }

   public ICollection<Province> GetProvinces()
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
         provs.Add(province);
      return provs;
   }
}