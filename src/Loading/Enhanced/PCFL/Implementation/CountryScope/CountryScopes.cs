using System.Diagnostics;
using Editor.Saving;

namespace Editor.Loading.Enhanced.PCFL.Implementation.CountryScope;

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

      if (block.ParseTriggerBlockToAnd(MagicMister.ProvinceScope, po, out var trigger))
         return new AllProvince_Scope(trigger);
      return ITrigger.Empty;
   }
}
