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

      public static ITrigger CreateTrigger(EnhancedBlock? block, LineKvp<string, string>? kvp, PCFL_Scope scope, PathObj po)
      {
         Debug.Assert(block is not null, "At this point the block must not be null. This must be filtered earlier in the pipeline");

         if (block.ParseTriggerBlockToAnd(ProvinceScopes.Scope, po, out var trigger))
            return new AllProvince_Scope(trigger);
         return ITrigger.Empty;
      }
   }

   #endregion

   #region EffectScopes
   public class EveryOwnedProvince_Scope(List<IToken> token) : Every_ScopeSwitch(token)
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

      public static IToken CreateToken(EnhancedBlock? block, LineKvp<string, string>? kvp, PCFL_Scope scope, PathObj po)
      {
         Debug.Assert(block is not null, "At this point the block must not be null. This must be filtered earlier in the pipeline");

         List<IToken> tokens = [];
         if (block.ParseTokenBlock(ProvinceScopes.Scope, po, tokens))
            return new EveryOwnedProvince_Scope(tokens);
         return IToken.Empty;
      }

   }

   #endregion
}