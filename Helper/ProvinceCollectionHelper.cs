using System.Collections;
using Editor.DataClasses.GameDataClasses;
using static Editor.Helper.ProvinceEnumHelper;

namespace Editor.Helper
{
   public static class ProvinceCollectionHelper
   {
      public static List<string> GetRegionNamesFromProvinces(List<Province> provinces)
      {
         List<string> uniqueRegionNames = [];
         var areaNames = GetAreaNamesFromProvinces(provinces);

         for (var i = 0; i < areaNames.Count; i++)
            if (!uniqueRegionNames.Contains(Globals.Areas[areaNames[i]].Region))
               uniqueRegionNames.Add(Globals.Areas[areaNames[i]].Region);

         return uniqueRegionNames;
      }

      public static List<string> GetAreaNamesFromProvinces(List<Province> provinces)
      {

         List<string> uniqueAreaNames = [];
         for (var i = 0; i < Selection.Count; i++)
            if (!uniqueAreaNames.Contains(Selection.GetSelectedProvinces[i].Area))
               uniqueAreaNames.Add(Selection.GetSelectedProvinces[i].Area);
         return uniqueAreaNames;
      }

      public static List<Province> GetProvincesWithAttribute(string attribute, object value, bool onlyLandProvinces = true)
      {
         if (onlyLandProvinces)
            return GetLandProvincesWithAttribute(attribute, value);
         var provinces = GetLandProvincesWithAttribute(attribute, value);
         provinces.AddRange(GetSeaProvincesWithAttribute(attribute, value));
         return provinces;
      }

      public static List<Province> GetProvincesWithAttribute(ProvAttrGet attribute, object value, bool onlyLandProvinces = true)
      {
         return GetProvincesWithAttribute(attribute.ToString(), value, onlyLandProvinces);
      }

      private static List<Province> GetLandProvincesWithAttribute(string attribute, object value)
      {
         List<Province> provinces = [];
         foreach (var id in Globals.LandProvinces)
            if (HasAttribute(id, attribute, value)) 
               provinces.Add(id);
         return provinces;
      }

      private static bool HasAttribute(Province province, string attribute, object value)
      {
         var attr = province.GetAttribute(attribute)!;
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

      private static List<Province> GetSeaProvincesWithAttribute(string attribute, object value)
      {
         List<Province> provinces = [];
         foreach (var id in Globals.SeaProvinces)
            if (HasAttribute(id, attribute, value))
               provinces.Add(id);
         return provinces;
      }

   }
}