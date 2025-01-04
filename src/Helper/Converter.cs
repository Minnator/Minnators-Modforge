using Editor.DataClasses.GameDataClasses;
using Editor.Saving;

namespace Editor.Helper
{

   public static class Converter
   {
      public static bool Convert<T>(string value, out T output) 
      {
         output = default!;
         var conversionType = typeof(T);

         try
         {
            if (typeof(IConvertible).IsAssignableFrom(conversionType))
            {
               output = (T)System.Convert.ChangeType(value, conversionType);
               return true;
            }

            if (ReferenceEquals(conversionType, typeof(Province)))
            {
               if (int.TryParse(value, out var id))
                  if (Globals.ProvinceIdToProvince.TryGetValue(id, out var province))
                  {
                     output = (T)(object)province;
                     return true;
                  }
               return false;
            }


            throw new EvilActions($"No conversion exists for {typeof(T)} for value {value}!");
         }
         catch (InvalidCastException)
         {
            return false;
         }
         catch (FormatException)
         {
            return false;
         }
         catch (OverflowException)
         {
            return false;
         }
      }
   }
}