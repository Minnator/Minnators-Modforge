using Editor.DataClasses.GameDataClasses;

namespace Editor.Helper
{
   public static class SearchHelper
   {
      public static List<int> SearchForProvinces(string input)
      {
         input = input.ToLower();
         List<int> result = [];

         // search for all province names which contain the input
         if (input.Length > 3)
            foreach (var province in Globals.Provinces.Values)
            {
               if (province.GetLocalisation().Contains(input, StringComparison.CurrentCultureIgnoreCase))
                  result.Add(province.Id);
            }
         if (int.TryParse(input, out var id))
            if (Globals.Provinces.ContainsKey(id))
               result.Add(id);

         return result;
      }

      public static List<Tag> SearchForCountries(string input)
      {
         input = input.ToLower();
         List<Tag> result = [];

         // search for all country tags which contain the input

         if (input.Length > 3)
            foreach (var country in Globals.Countries.Values)
            {
               if (country.GetLocalisation().Contains(input, StringComparison.OrdinalIgnoreCase))
                  result.Add(country.Tag);
            }
         if (Globals.Countries.TryGetValue(input, out var country1))
            result.Add(country1.Tag);

         return result;
      }

   }
}