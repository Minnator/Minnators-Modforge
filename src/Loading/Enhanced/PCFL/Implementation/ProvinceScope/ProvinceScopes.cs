
using Editor.Loading.Enhanced.PCFL.Implementation.ProvinceScope;
using static Editor.Loading.Enhanced.PCFL.Implementation.PCFL_Scope;

namespace Editor.Loading.Enhanced.PCFL.Implementation.ProvinceScope;

public static class ProvinceScopes
{
   public static PCFL_Scope Scope = new()
   {
      Triggers = {
         [BaseManpowerTrigger.TRIGGER_NAME] = BaseManpowerTrigger.CreateTrigger,
      },


      Effects = {
         [BaseManpowerEffect.EFFECT_NAME] = BaseManpowerEffect.CreateEffect,
      }
   };
}