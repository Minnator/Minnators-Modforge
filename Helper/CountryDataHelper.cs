namespace Editor.Helper
{
   public static class CountryDataHelper
   {

      public static List<int> GetAllCapitals()
      {
         List<int> capitals = [];

         foreach (var country in Globals.Countries.Values)
         {
            if (country.Exists && Globals.Provinces.TryGetValue(country.Capital, out var province))
               capitals.Add(province.Id);
         }

         return capitals;
      }

   }
}