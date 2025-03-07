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

public class PCFL_Scope
{
   public delegate ITrigger? PCFL_TriggerParseDelegate(EnhancedBlock? block, LineKvp<string, string>? kvp, PCFL_Scope scope, PathObj po);
   public delegate IToken? PCFL_TokenParseDelegate(EnhancedBlock? block, LineKvp<string, string>? kvp, PCFL_Scope scope, PathObj po);
   public Dictionary<string, PCFL_TriggerParseDelegate> Triggers { get; init; } = [];
   public Dictionary<string, PCFL_TokenParseDelegate> Effects { get; init; } = [];

   public bool IsValidTrigger(string str) => Triggers.ContainsKey(str);
   public bool IsValidTrigger(string str, out PCFL_TriggerParseDelegate pcflParse) => Triggers.TryGetValue(str, out pcflParse);

   public bool IsValidEffect(string str) => Effects.ContainsKey(str);
   public bool IsValidEffect(string str, out PCFL_TokenParseDelegate pcflParse) => Effects.TryGetValue(str, out pcflParse);

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

public abstract class All_ScopeSwitch(ITrigger trigger) : ITrigger
{
   public readonly ITrigger Trigger = trigger;

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

   public bool Evaluate(ITarget target)
   {
      foreach (var innerTarget in GetTargets(target))
      {
         if (!Trigger.Evaluate(innerTarget))
            return false;
      }
      return true;
   }
}

public abstract class Every_ScopeSwitch(List<IToken> tokens) : IToken
{
   public readonly List<IToken> Tokens = tokens;

   public abstract List<ITarget> GetTargets(ITarget target);

   public void Activate(ITarget target)
   {
      foreach (var innerTarget in GetTargets(target))
         foreach (var token in Tokens)
            token.Activate(innerTarget);
   }
}

public abstract class Any_ScopeSwitch(ITrigger trigger) : ITrigger
{
   public readonly ITrigger Trigger = trigger;
   public abstract List<ITarget> GetTargets(ITarget target);

   public bool EvaluateIfMoreOrEqualThan(ITarget target, int n)
   {
      _ = new ErrorInformation("Any scope used in 'calc_true_if'", "don't.");
      return false;
   }

   public bool Evaluate(ITarget target)
   {
      foreach (var innerTarget in GetTargets(target))
      {
         if (Trigger.Evaluate(innerTarget))
            return true;
      }
      return false;
   }
}
