using System.Text;

namespace Editor.Analyzers
{
   internal interface IAnalyzer
   {
      public void AnalyzeProvince(int id, out AnalyzerFeeback feedback);
      public void AnalyzeCountry(int id, out AnalyzerFeeback feedback);
      public void AnalyzeAllProvinces(out AnalyzerFeeback feedback);
      public void AnalyzeAllCountries(out AnalyzerFeeback feedback);
      public string GetAnalyzerName();
   }

   public abstract class AnalyzerFeeback
   {
      public virtual string Feedback
      {
         get
         {
            var sb = new StringBuilder();
            var maxLength = 100;
            foreach (var (key, value) in KeyValuePairs)
            {
               var val = $"| -> {key}: [{string.Join(", ", value)}]\n";
               maxLength = maxLength > val.Length ? maxLength : val.Length; 
               sb.Append(val);
            }
            sb.Insert(0, new string('-', maxLength) + "\n");
            sb.Append(new string('-', maxLength));

            return sb.ToString();
         }
      }
      public virtual string AnalyzedAttribute => string.Empty;
      // Contains the key value pairs for the analyzer e.g.:
      //                                                    "Provinces with Culture" -> ["12", "432", ...]
      //                                                    "Countries with it as Primary" -> ["TUR", "<TAG>", ...]
      public virtual Dictionary<string, List<string>> KeyValuePairs { get; set; } = [];

   }

   public class BaseAnalyzerFeedback : AnalyzerFeeback
   {
   }
}
