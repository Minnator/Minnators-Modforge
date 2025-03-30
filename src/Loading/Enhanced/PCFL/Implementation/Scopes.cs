﻿using Editor.DataClasses.Saveables;
using Editor.ErrorHandling;
using Editor.Loading.Enhanced.PCFL.Implementation.CountryScope;
using Editor.Loading.Enhanced.PCFL.Implementation.ProvinceScope;
using static Editor.Loading.Enhanced.PCFL.Implementation.PCFL_Scope;
using Region = Editor.DataClasses.Saveables.Region;

namespace Editor.Loading.Enhanced.PCFL.Implementation;

public static class Scopes
{

   public static bool AddToAllScopes(string name, PCFL_TokenParseDelegate func)
   {
      if (Province.Effects.ContainsKey(name) || Province.Effects.ContainsKey(name))
      {
         _ = new ErrorObject(ErrorType.DuplicateObjectDefinition, $"An effect with the name '{name}' has already been added to parsing!");
         return false;
      }

      Province.Effects.Add(name, func);
      Country.Effects.Add(name, func);
      return true;
   }

   public static bool AddToAllScopes(string name, PCFL_TriggerParseDelegate func)
   {
      if (Province.Triggers.ContainsKey(name) || Province.Triggers.ContainsKey(name))
      {
         _ = new ErrorObject(ErrorType.DuplicateObjectDefinition, $"A trigger with the name '{name}' has already been added to parsing!");
         return false;
      }
      
      Province.Triggers.Add(name, func);
      Country.Triggers.Add(name, func);
      return true;
   }

   public static bool IsEffectKeyUnused(string key)
   {
      return !Province.Effects.ContainsKey(key)
             && !Country.Effects.ContainsKey(key);
   }

   public static bool IsTriggerKeyUnused(string key)
   {
      return !Province.Triggers.ContainsKey(key)
             && !Country.Triggers.ContainsKey(key);
   }

   public static PCFL_Scope Province = new(typeof(Province))
   {
      Triggers = {
         [BaseManpowerTrigger.TRIGGER_NAME] = BaseManpowerTrigger.CreateTrigger,
         [BaseTaxTrigger.TRIGGER_NAME] = BaseTaxTrigger.CreateTrigger,
      },

      // Separate effects from Scopes so that owner = {} is not mistaken for owner = <tag>

      Effects = {
         // ------------------- Scopes --------------------
         [OwnerProvince_Scope.EFFECT_NAME] = OwnerProvince_Scope.CreateToken,
         // ------------------- Effects -------------------
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

   public static PCFL_Scope Country = new(typeof(Country))
   {
      Triggers = {
         [AllProvince_Scope.TRIGGER_NAME] = AllProvince_Scope.CreateTrigger,
      },
      Effects = {
         [EveryOwnedProvince_Scope.EFFECT_NAME] = EveryOwnedProvince_Scope.CreateToken,
         [EveryCoreProvince_Scope.EFFECT_NAME] = EveryCoreProvince_Scope.CreateToken,
      }
   };
}