using System.Diagnostics;
using System.Text;
using Editor.DataClasses.Saveables;
using Editor.Loading.Enhanced.PCFL.Implementation.ProvinceScope;
using Editor.Saving;
namespace Editor.Loading.Enhanced.PCFL.Implementation.CountryScope;

public static class CountryScope
{
   public static PCFL_Scope Scope = new()
   {
      Triggers = {
         [AllProvince_Scope.TRIGGER_NAME] = AllProvince_Scope.CreateTrigger,
      },
      Effects = {
         [EveryOwnedProvince_Scope.EFFECT_NAME] = EveryOwnedProvince_Scope.CreateToken,
      }
   };
}

