using System.Diagnostics;
using System.Text;
using Editor.DataClasses.Saveables;
using Editor.ErrorHandling;
using Editor.Helper;
using Editor.Loading.Enhanced.PCFL.Implementation.CountryScope;
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

public class PCFL_Scope(Type scopeType)
{
   public readonly Type ScopeType = scopeType;
   public delegate ITrigger? PCFL_TriggerParseDelegate(EnhancedBlock? block, LineKvp<string, string>? kvp, ParsingContext context, PathObj po);
   public delegate IToken? PCFL_TokenParseDelegate(EnhancedBlock? block, LineKvp<string, string>? kvp, ParsingContext context, PathObj po);
   public Dictionary<string, PCFL_TriggerParseDelegate> Triggers { get; init; } = [];
   public Dictionary<string, PCFL_TokenParseDelegate> Effects { get; init; } = [];
   
   public readonly static PCFL_Scope Empty = new(typeof(Type)); 

   public bool IsValidTrigger(string str) => Triggers.ContainsKey(str);
   public bool IsValidTrigger(string str, out PCFL_TriggerParseDelegate pcflParse) => Triggers.TryGetValue(str, out pcflParse);

   public bool IsValidEffect(string str) => Effects.ContainsKey(str) || Scopes.ScriptedEffects.ContainsKey(str);
   public bool IsValidEffect(string str, out PCFL_TokenParseDelegate pcflParse)
   {
      if(Effects.TryGetValue(str, out pcflParse))
         return true;
      if (Scopes.ScriptedEffects.TryGetValue(str, out pcflParse))
         return true;
      return false;
   }

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

/*
 * ScopeSwitch 
 *
 */

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

public abstract class ScopeSwitch(PCFL_Scope scope) : IPCFLObject
{
   public abstract List<ITarget> GetTargets(ITarget target);
   public ITarget? CurrentTarget { get; protected set; }
   public readonly PCFL_Scope scope = scope;

   public static readonly ScopeSwitch Empty = new EmptyScopeSwitch();

   public abstract bool Parse(EnhancedBlock block, PathObj po, ParsingContext parseContext);
}

public class EmptyScopeSwitch() : ScopeSwitch(PCFL_Scope.Empty)
{
   public override List<ITarget> GetTargets(ITarget target)
   {
      throw new NotImplementedException();
   }

   public override bool Parse(EnhancedBlock block, PathObj po, ParsingContext parseContext)
   {
      throw new NotImplementedException();
   }
}


public class ProvinceCollectionScopeSwitch<T, Q> : TokenScopeSwitch where T : ProvinceCollection<Q> where Q : ProvinceComposite
{
   private readonly T _collection;
   public ProvinceCollectionScopeSwitch(PCFL_Scope scope, T collection) : base(scope)
   {
      Debug.Assert(typeof(Province) == scope.ScopeType, "Scope and target type must always match up!");
      _collection = collection;
   }

   public override List<ITarget> GetTargets(ITarget target) => _collection.GetProvinces().Cast<ITarget>().ToList();
   public override string GetTokenName() => _collection.Name;
   public override string GetTokenDescription() => $"Scopes to the province of \"{_collection.Name}\"";
   public override string GetTokenExample() => $"{_collection.Name} = {{ <effects> }}";
}
public class SimpleFileScopeSwitch : TokenScopeSwitch
{
   public SimpleFileScopeSwitch(PCFL_Scope scope, ITarget target) : base(scope)
   {
      Debug.Assert(target.GetType() == scope.ScopeType, "Scope and target type must always match up!");
      CurrentTarget = target;
   }

   public override List<ITarget> GetTargets(ITarget target)
   {
      throw new NotImplementedException();
   }

   public override string GetTokenName() => "SimpleFileScopeSwitch";
   public override string GetTokenDescription() => "Scopes to the current targets of an executable file.";
   public override string GetTokenExample() => "---";

   public override void Activate(ITarget target)
   {
      foreach (var token in Tokens)
         token.Activate(CurrentTarget!);
   }
}

public abstract class TokenScopeSwitch(PCFL_Scope scope) : ScopeSwitch(scope), IToken
{
   public readonly List<IToken> Tokens = [];

   public override bool Parse(EnhancedBlock block, PathObj po, ParsingContext parseContext)
   {
      return block.ParseTokenBlock(parseContext.GetNext(this), po, Tokens);
   }

   public virtual void Activate(ITarget target)
   {
      foreach (var t in GetTargets(target))
      {
         CurrentTarget = t;
         foreach (var token in Tokens)
            token.Activate(CurrentTarget);
      }
   }

   public void GetTokenString(int tabs, ref StringBuilder sb)
   {
      SavingUtil.FormatSimpleTokenBlock(Tokens, tabs, GetTokenName(), ref sb);
   }
   public abstract string GetTokenName();
   public abstract string GetTokenDescription();
   public abstract string GetTokenExample();
}

public abstract class Every_ScopeSwitch(List<IToken> tokens) : IToken
{
   public List<IToken> Tokens = tokens;
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
