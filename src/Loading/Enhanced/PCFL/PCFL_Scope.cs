using Editor.DataClasses.Saveables;
using Editor.ErrorHandling;
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
      Tradenode,
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

   public static class ScriptExecutionTest
   {
      public static PCFL_Scope ProvinceScope = new (new()
      {
         [BaseManpowerTrigger.TRIGGER_NAME] = BaseManpowerTrigger.CreateTrigger
      });
   }


}