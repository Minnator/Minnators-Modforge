using System.Collections;
using System.Reflection;
using Editor.DataClasses.GameDataClasses;
using Editor.Saving;
using Newtonsoft.Json;

namespace Editor.Helper
{
   public static class EditingHelper
   {
      private static readonly object DefaultProvince = Activator.CreateInstance(typeof(Province))!;
      private static readonly PropertyInfo[] ProvinceProperties = DefaultProvince.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

      /// <summary>
      /// Compares the values of the given province with the default values of a province and returns a list of key-value pairs of the non-default values.
      /// </summary>
      /// <param name="prov"></param>
      /// <returns></returns>
      public static List<KeyValuePair<string, object>> GetNonDefaultValues(this Province prov)
      {
         var nonDefaultValues = new List<KeyValuePair<string, object>>();

         foreach (var property in ProvinceProperties)
         {
            var value = property.GetValue(prov); 

            if (value is IList list && property.GetValue(DefaultProvince) is IList defaultList)
            {
               if (!AreListsEqual(list, defaultList)) 
                  nonDefaultValues.Add(new(property.Name, list));
            }
            else if (value != null && !value.Equals(property.GetValue(DefaultProvince)))
            {
               if (value.GetType() != typeof(ProvinceData)) // Ignore ProvinceData as all data is stored in it.
                  nonDefaultValues.Add(new(property.Name, value));
            }
         }

         return nonDefaultValues;
      }

      public static List<Province> GetModifiedProvinces()
      {
         List<Province> modifiedProvinces = [];
         foreach (var province in Globals.Provinces)
         {
            if (province.EditingStatus == ObjEditingStatus.Modified)
               modifiedProvinces.Add(province);
         }
         return modifiedProvinces;
      }

      public static List<Province> GetProvincesWithStatus(ObjEditingStatus status)
      {
         List<Province> provinces = [];
         foreach (var province in Globals.Provinces)
            if (province.EditingStatus == status) 
               provinces.Add(province);
         return provinces;
      }

      public static List<Province> GetProvincesWithoutStatus(ObjEditingStatus status)
      {
         List<Province> provinces = [];
         foreach (var province in Globals.Provinces)
            if (province.EditingStatus != status) 
               provinces.Add(province);
         return provinces;
      }

      public static void PrintModifiedProvinceValues(this Province province, out string str)
      {
         var nonDefaultValues = province.GetNonDefaultValues();
         str = "";
         foreach (var (key, value) in nonDefaultValues)
            str += $"{key}: {value}\n";
      }

      private static bool AreListsEqual(IList? list1, IList? list2)
      {
         if (list1 == null || list2 == null)
            return Equals(list1, list2);

         if (list1.Count != list2.Count)
            return false;

         for (var i = 0; i < list1.Count; i++)
         {
            var item1 = list1[i];
            var item2 = list2[i];

            if (!Equals(item1, item2))
               return false;
         }

         return true;
      }

      public static bool DeepCopy(object obj, out object? copy)
      {
         if (obj == null!)
         {
            copy = null;
            return false;
         }

         var json = JsonConvert.SerializeObject(obj);
         copy = JsonConvert.DeserializeObject(json, obj.GetType());
         return copy != null;
      }
   }

}