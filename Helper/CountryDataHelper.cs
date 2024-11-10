using System.Diagnostics;
using Editor.DataClasses.GameDataClasses;

namespace Editor.Helper
{
   public static class CountryDataHelper
   {

      public static List<Province> GetAllVisibleCapitals()
      {
         var capitals = new List<Province>();
         foreach (var country in Globals.Countries.Values)
         {
            if (country.Exists)
               capitals.Add(country.Capital);
         }
         return capitals;
      }

   }
}