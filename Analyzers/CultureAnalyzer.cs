using Editor.DataClasses.GameDataClasses;

namespace Editor.Analyzers
{
   public class CultureAnalyzer : IAnalyzer
   {
      public void AnalyzeAllCountries(out AnalyzerFeeback feedback)
      {
         throw new NotImplementedException();
      }

      public void AnalyzeAllProvinces(out AnalyzerFeeback feedback)
      {
         throw new NotImplementedException();
      }

      public void AnalyzeCountry(Province id, out AnalyzerFeeback feedback)
      {
         throw new NotImplementedException();
      }

      public void AnalyzeProvince(Province id, out AnalyzerFeeback feedback)
      {         
         AnalyzeCulture(id, out feedback);
      }

      public string GetAnalyzerName()
      {
         return "Culture Analyzer";
      }

      private void AnalyzeCulture(Province id, out AnalyzerFeeback feeback)
      {
         feeback = new BaseAnalyzerFeedback();
         var culture = Globals.Cultures[id.Culture];
         var provincesWithCulture = GetProvincesWithCulture(culture.Name);
         feeback.KeyValuePairs.Add($"[{provincesWithCulture.Count}] Provinces with Culture", provincesWithCulture);
         feeback.KeyValuePairs.Add("maleNames", [GetNameStats(culture.MaleNames, "maleNames")]);
      }

      private string GetNameStats(string[] names, string namesType)
      {
         var duplicateCounter = 0;
         HashSet<string> uniqueNames = [];

         foreach (var name in names)
         {
            if (!uniqueNames.Add(name))
            {
               duplicateCounter++;
            }
         }
         return $"[{uniqueNames.Count}] {namesType} with [{duplicateCounter}] duplicates";
      }

      private List<string> GetProvincesWithCulture(string culture){
         List<string> provinces = [];

         foreach (var province in Globals.Provinces)
         {
            if (province.Culture.Equals(culture))
            {
               provinces.Add(province.Id.ToString());
            }
         }

         return provinces;
      }
   }
}
