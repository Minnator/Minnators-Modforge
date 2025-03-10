using System.Text;
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

   public void GetTokenString(int tabs, ref StringBuilder sb)
   {
      SavingUtil.FormatSimpleTokenBlock(Tokens, tabs, GetTokenName(), ref sb);
   }
   public abstract string GetTokenName();
   public string GetTokenDescription() => "Scopes to all provinces the current scope owns.";
   public string GetTokenExample() => "every_owned_province = { <Effect> }";
}

public abstract class Random_ScopeSwitch(List<IToken> tokens)
{
   public readonly List<IToken> Tokens = tokens;
   public abstract List<ITarget> GetTargets(ITarget target);

   public void Activate(ITarget target)
   {
      var targets = GetTargets(target);
      if (targets.Count == 0)
         return;
      var randomTarget = targets[Globals.Random.Next(targets.Count)];
      foreach (var token in Tokens)
         token.Activate(randomTarget);
   }

   public void GetTokenString(int tabs, ref StringBuilder sb)
   {
      SavingUtil.FormatSimpleTokenBlock(Tokens, tabs, GetTokenName(), ref sb);
   }

   public abstract string GetTokenName();
   public string GetTokenDescription() => "Scopes to a random province the current scope owns.";
   public string GetTokenExample() => "random_owned_province = { <Effect> }";
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
