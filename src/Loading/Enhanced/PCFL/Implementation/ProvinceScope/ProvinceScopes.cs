
using Editor.Loading.Enhanced.PCFL.Implementation.ProvinceScope;
using static Editor.Loading.Enhanced.PCFL.Implementation.PCFL_Scope;

namespace Editor.Loading.Enhanced.PCFL.Implementation.ProvinceScope;

public static class ProvinceScopes
{
   public static PCFL_Scope Scope = new()
   {
      Triggers = {
         [BaseManpowerTrigger.TRIGGER_NAME] = BaseManpowerTrigger.CreateTrigger,
         [BaseTaxTrigger.TRIGGER_NAME] = BaseTaxTrigger.CreateTrigger,
      },


      Effects = {
         [BaseManpowerEffect.EffectName] = BaseManpowerEffect.CreateEffect,
         [OwnerEffect.EffectName] = OwnerEffect.CreateEffect,
         [ControllerEffect.EffectName] = ControllerEffect.CreateEffect,
         [AddCoreEffect.EffectName] = AddCoreEffect.CreateEffect,
         [RemoveCoreEffect.EffectName] = RemoveCoreEffect.CreateEffect,
         [BaseProductionEffect.EffectName] = BaseProductionEffect.CreateEffect,
         [BaseTaxEffect.EffectName] = BaseTaxEffect.CreateEffect,
         [UnrestEffect.EffectName] = UnrestEffect.CreateEffect,
         [RevoltRiskEffect.EffectName] = RevoltRiskEffect.CreateEffect,
         [RevoltEffect.EffectName] = RevoltEffect.CreateEffect,
         [AddBaseManpowerEffect.EffectName] = AddBaseManpowerEffect.CreateEffect,
         [AddBaseProductionEffect.EffectName] = AddBaseProductionEffect.CreateEffect,
         [AddBaseTaxEffect.EffectName] = AddBaseTaxEffect.CreateEffect,
         [ReligionEffect.EffectName] = ReligionEffect.CreateEffect,
         [ReformationCenterEffect.EffectName] = ReformationCenterEffect.CreateEffect,
         [CultureEffect.EffectName] = CultureEffect.CreateEffect,
         [DiscoveredByEffect.EffectName] = DiscoveredByEffect.CreateEffect,
         [IsCityEffect.EffectName] = IsCityEffect.CreateEffect,
         [HreEffect.EffectName] = HreEffect.CreateEffect,
         [CapitalEffect.EffectName] = CapitalEffect.CreateEffect,
         [TradeGoodsEffect.EffectName] = TradeGoodsEffect.CreateEffect,
         [SeatInParliamentEffect.EffectName] = SeatInParliamentEffect.CreateEffect,
         [TribalOwnerEffect.EffectName] = TribalOwnerEffect.CreateEffect,
         [AddLocalAutonomyEffect.EffectName] = AddLocalAutonomyEffect.CreateEffect,
         [NativeSizeEffect.EffectName] = NativeSizeEffect.CreateEffect,
         [AddClaimEffect.EffectName] = AddClaimEffect.CreateEffect,
         [RemoveClaimEffect.EffectName] = RemoveClaimEffect.CreateEffect,
         [AddPermanentProvinceModifierEffect.EffectName] = AddPermanentProvinceModifierEffect.CreateEffect,
         [AddTradeCompanyInvestmentModifierEffect.EffectName] = AddTradeCompanyInvestmentModifierEffect.CreateEffect,
         [NativeHostilnessEffect.EffectName] = NativeHostilnessEffect.CreateEffect,
         [NativeFerocityEffect.EffectName] = NativeFerocityEffect.CreateEffect,
         [RemoveProvinceTriggeredModifierEffect.EffectName] = RemoveProvinceTriggeredModifierEffect.CreateEffect,
         [AddProvinceTriggeredModifierEffect.EffectName] = AddProvinceTriggeredModifierEffect.CreateEffect,
         [AddToTradeCompanyEffect.EffectName] = AddToTradeCompanyEffect.CreateEffect,
         [RemoveProvinceModifierEffect.EffectName] = RemoveProvinceModifierEffect.CreateEffect,
         [ExtraCostEffect.EffectName] = ExtraCostEffect.CreateEffect,
         [CenterOfTradeEffect.EffectName] = CenterOfTradeEffect.CreateEffect,
         [SetGlobalFlagEffect.EffectName] = SetGlobalFlagEffect.CreateEffect,
         [RenameCapitalEffect.EffectName] = RenameCapitalEffect.CreateEffect,
         [ChangeProvinceNameEffect.EffectName] = ChangeProvinceNameEffect.CreateEffect,
         [AddProsperityEffect.EffectName] = AddProsperityEffect.CreateEffect,
         [AddDevastationEffect.EffectName] = AddDevastationEffect.CreateEffect,
         [SetLocalAutonomyEffect.EffectName] = SetLocalAutonomyEffect.CreateEffect,
         [ChangeTradeGoodsEffect.EffectName] = ChangeTradeGoodsEffect.CreateEffect,
         [AddScaledLocalMilPowerEffect.EffectName] = AddScaledLocalMilPowerEffect.CreateEffect,
         [AddScaledLocalDipPowerEffect.EffectName] = AddScaledLocalDipPowerEffect.CreateEffect,
         [AddScaledLocalAdmPowerEffect.EffectName] = AddScaledLocalAdmPowerEffect.CreateEffect,
      }
   };


}