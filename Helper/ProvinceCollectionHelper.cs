using System.Collections;
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

      public static List<int> GetProvincesWithAttribute(ProvinceAttribute attribute, object value, bool onlyLandProvinces = true)
      {
         return GetProvincesWithAttribute(attribute.ToString(), value, onlyLandProvinces);
      }

      private static List<int> GetLandProvincesWithAttribute(string attribute, object value)
      {
         List<int> provinces = [];
         foreach (var id in Globals.LandProvinceIds)
            if (HasAttribute(id, attribute, value)) 
               provinces.Add(id);
         return provinces;
      }

      private static bool HasAttribute(int id, string attribute, object value)
      {
         var attr = Globals.Provinces[id].GetAttribute(attribute)!;
         var val = value.ToString();
         if (attr is IList list)
         {
            foreach (var item in list)
               if (item is not null && item.ToString()!.Equals(val))
                  return true;
         }
         else if (attr.ToString()!.Equals(val))
         {
            return true;
         }
         return false;
      }

      private static List<int> GetSeaProvincesWithAttribute(string attribute, object value)
      {
         List<int> provinces = [];
         foreach (var id in Globals.SeaProvinces)
            if (HasAttribute(id, attribute, value))
               provinces.Add(id);
         return provinces;
      }

   }
}