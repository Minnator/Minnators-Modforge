using Editor.ErrorHandling;
using Editor.Helper;
using Editor.Loading.Enhanced.PCFL.Implementation.ProvinceScope;
using Editor.Saving;

namespace Editor.Loading.Enhanced.PCFL.Implementation;

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

public class PCFL_Scope(Dictionary<string, PCFL_Scope.TriggerDelegate> triggers)
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
