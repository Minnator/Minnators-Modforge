using Editor.Interfaces;

namespace Editor.DataClasses.GameDataClasses
{
   public class ProvinceGroup : IProvinceCollection
   {
      public string Name { get; }
      public Province[] Provinces { get; set; }
      public int[] GetProvinceIds()
      {
         List<int> ids = [];
         foreach (var province in Provinces)
            ids.Add(province.Id);
         return ids.ToArray();
      }

      public ICollection<Province> GetProvinces()
      {
         return Provinces;
      }

      public IProvinceCollection? ScopeOut()
      {
         return this;
      }

      public List<IProvinceCollection>? ScopeIn()
      {
         return [];
      }

      public Color Color { get; set; }
   }
}