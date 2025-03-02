using System.Diagnostics;
using Editor.DataClasses.Saveables;
using Editor.ErrorHandling;
using Editor.Helper;
using Editor.Loading.Enhanced.PCFL.Scribbel;
using Editor.Saving;
using static Editor.Loading.Enhanced.PCFL.PCFL_Scope;

namespace Editor.Loading.Enhanced.PCFL
{
   // Possible scopes
   // Country
   // Province
   // Tradenode
   // Ruler / heir / consort
   // Advisor
   // Unit
   // Fleet
   // Missions

   public enum ScopeType
   {
      Country,
      Province,
      TradeNode,
      Ruler,
      Heir,
      Consort,
      Advisor,
      Unit,
      Fleet,
      Missions
   }

   public class PCFL_Scope(Dictionary<string, TriggerDelegate> triggers)
   {
      public delegate Trigger? TriggerDelegate(EnhancedBlock? block, LineKvp<string, string>? kvp, PCFL_Scope scope, PathObj po);
      private readonly Dictionary<string, TriggerDelegate> Triggers = triggers;

      public bool IsValidTrigger(string str) => Triggers.ContainsKey(str);
      public bool IsValidTrigger(string str, out TriggerDelegate trigger) => Triggers.TryGetValue(str, out trigger);

      /*
       * Routing
       * Trigger 
       * Effect
       */
      /*
      private Dictionary<string, ITargetProvider> ScopeRouting;
      private Dictionary<string, Func<Trigger>> Triggers;
      private Dictionary<string, PCFL_EffectBase> Effects;
      */

      
   }

   public abstract class All_ScopeSwitch(Trigger trigger) : Trigger
   {
      public readonly Trigger Trigger = trigger;

      public abstract List<ITarget> GetTargets(ITarget target);

      public bool EvaluateIfMoreOrEqualThan(ITarget target, int n)
      {
         var counter = 0;
         foreach (var innerTarget in GetTargets(target))
            if (Trigger.Evaluate(innerTarget))
               if (++counter >= n)
                  return true;
         return false;
      }

      public override bool Evaluate(ITarget target)
      {
         foreach (var innerTarget in GetTargets(target))
         {
            if (!Trigger.Evaluate(innerTarget))
               return false;
         }
         return true;
      }
   }

   public abstract class Any_ScopeSwitch(Trigger trigger) : Trigger
   {
      public readonly Trigger Trigger = trigger;
      public abstract List<ITarget> GetTargets(ITarget target);

      public bool EvaluateIfMoreOrEqualThan(ITarget target, int n)
      {
         _ = new ErrorInformation("Any scope used in 'calc_true_if'", "don't.");
         return false;
      }

      public override bool Evaluate(ITarget target)
      {
         foreach (var innerTarget in GetTargets(target))
         {
            if (Trigger.Evaluate(innerTarget))
               return true;
         }
         return false;
      }
   }

   public class AllProvince_Scope(Trigger trigger) : All_ScopeSwitch(trigger)
   {
      public const string TRIGGER_NAME = "all_province";
      public override List<ITarget> GetTargets(ITarget target) //TODO resolve via ref or enumerable to reduce ram in case of deep nesting and sorting needs fixing IMPORTANT
      {
         var list = Globals.Provinces.Cast<ITarget>().ToList();
         list.Sort();
         return list;
      }

      public static Trigger CreateTrigger(EnhancedBlock? block, LineKvp<string, string>? kvp, PCFL_Scope scope, PathObj po)
      {
         Debug.Assert(block is not null, "At this point the block must not be null. This must be filtered earlier in the pipeline");
         
         if (block.ParseTriggerBlockToAnd(MagicMister.ProvinceScope, po, out var trigger))
            return new AllProvince_Scope(trigger);
         return Empty;
      }
   }


   public static class MagicMister
   {
      public static PCFL_Scope ProvinceScope = new (new()
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

         var tokens = PCFL_TriggerParser.ParseSomeFile(content, CountryScope, po);

         ITarget target = Globals.Countries["TUR"];

         foreach (var token in tokens) {
            token.Activate(target);
         }


      }
   }


}