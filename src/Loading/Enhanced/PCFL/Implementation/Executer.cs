using Editor;
using Editor.Helper;
using Editor.Loading.Enhanced.PCFL.Implementation;
using Editor.Loading.Enhanced.PCFL.Implementation.CountryScope;
using Editor.Loading.Enhanced.PCFL.Implementation.ProvinceScope;
using Editor.Saving;

public static class MagicMister
{
   public static PCFL_Scope ProvinceScope = new(new()
   {
      [BaseManpowerTrigger.TRIGGER_NAME] = BaseManpowerTrigger.CreateTrigger,
   });

   public static PCFL_Scope CountryScope = new(new()
   {
      [AllProvince_Scope.TRIGGER_NAME] = AllProvince_Scope.CreateTrigger,
   });

   public static void ExecuteFile(string filePath)
   {
      var po = PathObj.FromPath(filePath, false);
      var content = IO.ReadAllInUTF8(filePath);

      var tokens = GeneralFileParser.ParseSomeFile(content, CountryScope, po);

      ITarget target = Globals.Countries["TUR"];

      foreach (var token in tokens)
      {
         token.Activate(target);
      }


   }
}