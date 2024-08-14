using System.Diagnostics;
using Editor.DataClasses.GameDataClasses;

namespace Editor.Helper
{
   public static class CountryDataHelper
   {

      public static List<int> GetAllVisibleCapitals()
      {
         var capitals = new List<int>();
         foreach (var country in Globals.Countries.Values)
         {
            if (country.Capital == 2297)
            {
               foreach (var id in Globals.LandProvinces)
               {
                  if (Globals.Provinces[id].Owner == country.Tag)
                     Debug.WriteLine($"{country.Tag} owns {id}");
               }
            }

            if (country.Exists)
               capitals.Add(country.Capital);
         }
         return capitals;
      }

      public static void CrossCheckCapitals()
      {
         var capitals = GetAllVisibleCapitals();
         foreach (var cap in capitals)
         {
            Globals.Capitals.Remove(cap);
         }

         Debug.WriteLine($"{Globals.Capitals.Count} were wrong");
      }

      public static void CorrectCapitals()
      {
         Globals.Capitals = [..GetAllVisibleCapitals()];

      }
   }
}