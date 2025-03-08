
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
         [OwnerEffect.EFFECT_NAME] = OwnerEffect.CreateEffect,
         [ControllerEffect.EFFECT_NAME] = ControllerEffect.CreateEffect,
         [AddCoreEffect.EFFECT_NAME] = AddCoreEffect.CreateEffect,
         [RemoveCoreEffect.EFFECT_NAME] = RemoveCoreEffect.CreateEffect,
         [BaseProductionEffect.EFFECT_NAME] = BaseProductionEffect.CreateEffect,
         [BaseTaxEffect.EFFECT_NAME] = BaseTaxEffect.CreateEffect,
         [UnrestEffect.EFFECT_NAME] = UnrestEffect.CreateEffect,
         [RevoltEffect.EFFECT_NAME] = RevoltEffect.CreateEffect,
         [AddBaseManpowerEffect.EFFECT_NAME] = AddBaseManpowerEffect.CreateEffect,
         [AddBaseProductionEffect.EFFECT_NAME] = AddBaseProductionEffect.CreateEffect,
         [AddBaseTaxEffect.EFFECT_NAME] = AddBaseTaxEffect.CreateEffect,
         [ReligionEffect.EFFECT_NAME] = ReligionEffect.CreateEffect,
         [CultureEffect.EFFECT_NAME] = CultureEffect.CreateEffect,

      }
   };
}