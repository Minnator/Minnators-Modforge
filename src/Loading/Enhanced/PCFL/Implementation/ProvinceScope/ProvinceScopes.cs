﻿
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
         [RevoltRiskEffect.EFFECT_NAME] = RevoltRiskEffect.CreateEffect,
         [RevoltEffect.EFFECT_NAME] = RevoltEffect.CreateEffect,
         [AddBaseManpowerEffect.EFFECT_NAME] = AddBaseManpowerEffect.CreateEffect,
         [AddBaseProductionEffect.EFFECT_NAME] = AddBaseProductionEffect.CreateEffect,
         [AddBaseTaxEffect.EFFECT_NAME] = AddBaseTaxEffect.CreateEffect,
         [ReligionEffect.EFFECT_NAME] = ReligionEffect.CreateEffect,
         [ReformationCenterEffect.EFFECT_NAME] = ReformationCenterEffect.CreateEffect,
         [CultureEffect.EFFECT_NAME] = CultureEffect.CreateEffect,
         [DiscoveredByEffect.EFFECT_NAME] = DiscoveredByEffect.CreateEffect,
         [IsCityEffect.EFFECT_NAME] = IsCityEffect.CreateEffect,
         [HREEffect.EFFECT_NAME] = HREEffect.CreateEffect,
         [CapitalEffect.EFFECT_NAME] = CapitalEffect.CreateEffect,
         [TradeGoodsEffect.EFFECT_NAME] = TradeGoodsEffect.CreateEffect,
         [SeatInParliamentEffect.EFFECT_NAME] = SeatInParliamentEffect.CreateEffect,
         [TribalOwnerEffect.EFFECT_NAME] = TribalOwnerEffect.CreateEffect,
         [AddLocalAutonomyEffect.EFFECT_NAME] = AddLocalAutonomyEffect.CreateEffect,
         [NativeSizeEffect.EFFECT_NAME] = NativeSizeEffect.CreateEffect,
         [AddClaimEffect.EFFECT_NAME] = AddClaimEffect.CreateEffect,
         [RemoveClaimEffect.EFFECT_NAME] = RemoveClaimEffect.CreateEffect,
         [AddPermanentProvinceModifierEffect.EFFECT_NAME] = AddPermanentProvinceModifierEffect.CreateEffect,
         [AddTradeCompanyInvestmentModifierEffect.EFFECT_NAME] = AddTradeCompanyInvestmentModifierEffect.CreateEffect,
         [NativeHostilnessEffect.EFFECT_NAME] = NativeHostilnessEffect.CreateEffect,
         [NativeFerocityEffect.EFFECT_NAME] = NativeFerocityEffect.CreateEffect,
         [RemoveProvinceTriggeredModifierEffect.EFFECT_NAME] = RemoveProvinceTriggeredModifierEffect.CreateEffect,
         [AddProvinceTriggeredModifierEffect.EFFECT_NAME] = AddProvinceTriggeredModifierEffect.CreateEffect,
         [AddToTradeCompanyEffect.EFFECT_NAME] = AddToTradeCompanyEffect.CreateEffect,
      }
   };


}