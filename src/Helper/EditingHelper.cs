using System.Collections;
using System.Reflection;
using Editor.DataClasses.GameDataClasses;
using Editor.DataClasses.Saveables;
using Newtonsoft.Json;

namespace Editor.Helper
{
   public static class EditingHelper
   {
      private static readonly object DefaultProvince = Activator.CreateInstance(typeof(Province))!;
      private static readonly PropertyInfo[] ProvinceProperties = DefaultProvince.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);


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