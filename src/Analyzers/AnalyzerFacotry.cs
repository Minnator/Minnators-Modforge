namespace Editor.Analyzers
{
   internal static class AnalyzerFacotry
   {
      private static readonly Dictionary<string, IAnalyzer> Analyzers = new()
      {
         { "culture", new CultureAnalyzer() },
      };


      public static IAnalyzer GetAnalyzer(string type)
      {
         if (Analyzers.TryGetValue(type, out var analyzer))
            return analyzer;

         return null!;
      }

      public static bool HasAnalyzer(string type)
      {
        return Analyzers.ContainsKey(type);
      }
   }
}
