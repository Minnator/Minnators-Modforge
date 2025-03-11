using Editor.DataClasses.Saveables;
using Editor.Loading.Enhanced.PCFL.Implementation.ProvinceScope;
using Editor.Saving;
using System.Diagnostics;

namespace Editor.Loading.Enhanced.PCFL.Implementation.CountryScope
{
   #region TriggerScopes

   public class AllProvince_Scope(ITrigger trigger) : All_ScopeSwitch(trigger)
   {
      public const string TRIGGER_NAME = "all_province";
      public override List<ITarget> GetTargets(ITarget target) //TODO resolve via ref or enumerable to reduce ram in case of deep nesting and sorting needs fixing IMPORTANT
      {
         var list = Globals.Provinces.Cast<ITarget>().ToList();
         list.Sort();
         return list;
      }

      public static ITrigger CreateTrigger(EnhancedBlock? block, LineKvp<string, string>? kvp, ParsingContext context, PathObj po)
      {
         Debug.Assert(block is not null, "At this point the block must not be null. This must be filtered earlier in the pipeline");

         if (block.ParseTriggerBlockToAnd(context, po, out var trigger))
            return new AllProvince_Scope(trigger);
         return ITrigger.Empty;
      }
   }

   #endregion

   #region EffectScopes
   public class EveryOwnedProvince_Scope() : TokenScopeSwitch(Scopes.Province)
   {
      public const string EFFECT_NAME = "every_owned_province";

      public override List<ITarget> GetTargets(ITarget target)
      {
         Debug.Assert(target is Country, $"'{EFFECT_NAME}' effect is only valid on countries");

         var list = ((Country)target).SubCollection.Cast<ITarget>().ToList();
         list.Sort();
         return list;
      }

      public override string GetTokenName() => EFFECT_NAME;
      public override string GetTokenDescription()
      {
         throw new NotImplementedException();
      }

      public override string GetTokenExample()
      {
         throw new NotImplementedException();
      }

      public static IToken CreateToken(EnhancedBlock? block, LineKvp<string, string>? kvp, ParsingContext context, PathObj po)
      {
         Debug.Assert(block is not null, "At this point the block must not be null. This must be filtered earlier in the pipeline");

         var switchScope = new EveryOwnedProvince_Scope();
         if (switchScope.Parse(block, po, context))
            return switchScope;
         return IToken.Empty;
      }
   }

   public class EveryCoreProvince_Scope() : TokenScopeSwitch(Scopes.Province)
   {
      public static readonly string EFFECT_NAME = "every_core_province";
      public static readonly string EFFECT_DESC = "Scopes to all provinces that this country has a core on, whether owned or not. ";
      public static readonly string EFFECT_EXAMPLE = "every_core_province = { <Effect> }";

      public override List<ITarget> GetTargets(ITarget target)
      {
         Debug.Assert(target is Country, $"'{EFFECT_NAME}' effect is only valid on countries");

         var country = (Country)target;
         var list = Globals.Provinces.Where(x => x.Cores.Contains(country.Tag)).Cast<ITarget>().ToList();
         list.Sort();
         return list;
      }

      public override string GetTokenName() => EFFECT_NAME;
      public override string GetTokenDescription() => EFFECT_DESC;

      public override string GetTokenExample() => EFFECT_EXAMPLE;

      public static IToken CreateToken(EnhancedBlock? block, LineKvp<string, string>? kvp, ParsingContext context, PathObj po)
      {
         Debug.Assert(block is not null, "At this point the block must not be null. This must be filtered earlier in the pipeline");

         var switchScope = new EveryCoreProvince_Scope();
         if (switchScope.Parse(block, po, context))
            return switchScope;
         return IToken.Empty;
      }
   }

   #endregion
}