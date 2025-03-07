using System.Diagnostics;
using Editor;
using Editor.Helper;
using Editor.Loading.Enhanced.PCFL.Implementation;
using Editor.Loading.Enhanced.PCFL.Implementation.CountryScope;
using Editor.Loading.Enhanced.PCFL.Implementation.ProvinceScope;
using Editor.Saving;

public static class Executor
{
   public static void ExecuteFile(string filePath, bool country)
   {
      var po = PathObj.FromPath(filePath, false);
      var content = IO.ReadAllInUTF8(filePath);
      List<IToken> tokens;
      ITarget target;

      if (country)
      {
         var sw = Stopwatch.StartNew();
         tokens = GeneralFileParser.ParseSomeFile(content, CountryScope.Scope, po);
         target = Globals.Countries["TUR"];
         sw.Stop();
         Debug.WriteLine($"Parsed {tokens.Count} tokens in {sw.ElapsedMilliseconds}ms");
      }
      else
      {
         tokens = GeneralFileParser.ParseSomeFile(content, ProvinceScopes.Scope, po);
         target = Globals.Provinces.First();
      }

      foreach (var token in tokens)
      {
         token.Activate(target);
      }


   }
}