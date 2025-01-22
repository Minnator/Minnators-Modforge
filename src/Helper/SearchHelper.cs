using Editor.DataClasses.GameDataClasses;

namespace Editor.Helper
{
   public static class SearchHelper
   {
      public static List<Province> SearchForProvinces(string input)
      {
         input = input.ToLower();
         List<Province> result = [];

         // search for all province names which contain the input
         if (input.Length > 3)
            foreach (var province in Globals.Provinces)
            {
               if (province.TitleLocalisation.Contains(input, StringComparison.CurrentCultureIgnoreCase))
                  result.Add(province);
            }
         if (int.TryParse(input, out var id))
            if (Globals.ProvinceIdToProvince.TryGetValue(id, out var prov))
               result.Add(prov);

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
               if (country.TitleLocalisation.Contains(input, StringComparison.OrdinalIgnoreCase))
                  result.Add(country.Tag);
            }
         
         if (input.Length == 3)
            if (Globals.Countries.TryGetValue(input.ToUpper(), out var country))
               result.Add(country.Tag);

         return result;
      }

   }
}