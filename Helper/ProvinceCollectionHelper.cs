using Editor.DataClasses.GameDataClasses;

namespace Editor.Helper
{
   public static class ProvinceCollectionHelper
   {

      public static List<int> GetProvincesWithAttribute(string attribute, object value, bool onlyLandProvinces = true)
      {
         if (onlyLandProvinces)
            return GetLandProvincesWithAttribute(attribute, value);
         var provinces = GetLandProvincesWithAttribute(attribute, value);
         provinces.AddRange(GetSeaProvincesWithAttribute(attribute, value));
         return provinces;
      }

      private static List<int> GetLandProvincesWithAttribute(string attribute, object value)
      {
         List<int> provinces = [];
         foreach (var id in Globals.LandProvinceIds)
         {
            if (Globals.Provinces[id].GetAttribute(attribute)!.Equals(value))
               provinces.Add(id);
         }
         return provinces;
      }

      private static List<int> GetSeaProvincesWithAttribute(string attribute, object value)
      {
         List<int> provinces = [];
         foreach (var id in Globals.SeaProvinces)
         {
            var attr = Globals.Provinces[id].GetAttribute(attribute);
            if (attr != null && attr.Equals(value))
               provinces.Add(id);
         }
         return provinces;
      }

   }
}