using System.Diagnostics;
using System.Text;
using Editor.DataClasses.Saveables;
using Editor.ErrorHandling;
using Editor.Helper;
using Editor.Saving;

namespace Editor.Loading.Enhanced.PCFL.Implementation;

public interface ITarget
{
   public static ITarget Empty { get; } = new EmptyTarget();
}

public class EmptyTarget : ITarget;

public abstract class Value(Type type)
{
   public Type Type = type;
   public abstract void CopyFrom(Value val);

   public abstract void CopyTo(Value val);

   public abstract void SetDefault();
}

public class RefTargetValue<T> : Value<T> where T : notnull
{
   public override T Val { 
      get 
      {
         Debug.Assert(scopeSwitch.CurrentTarget is T, "CurrentTarget must always be of matching type!");
         return (T)scopeSwitch.CurrentTarget;
      }
   }
   private ScopeSwitch scopeSwitch;

   public static bool TryParseRefTargetValue(ScopeSwitch scopeSwitch, ref Value<T> value)
   {
      if (scopeSwitch.scope.ScopeType != typeof(T))
      {

         return false;
         //TODO what should happen here parsing error and fallback?
      }
      value = new RefTargetValue<T>(scopeSwitch, value.DefaultValue);

      return true;
   }

   private RefTargetValue(ScopeSwitch scopeSwitch, T defaultValue) : base(defaultValue)
   {
      this.scopeSwitch = scopeSwitch;
   }
}

public class Value<T>(T defaultValue) : Value(typeof(T)) where T : notnull
{
   public T DefaultValue = defaultValue;
   public virtual T Val { get; set; } = defaultValue;

   public static Value<T> operator <<(Value<T> a, Value<T> b)
   {
      a.Val = b.Val;
      return b;
   }
   public static Value<T> operator >>(Value<T> b, Value<T> a)
   {
      a.Val = b.Val;
      return a;
   }

   public override void CopyFrom(Value val)
   {
      Val = val.Type == Type ? (val as Value<T>)!.Val : Val;
   }

   public override void CopyTo(Value val)
   {
      if (val.Type == Type)
         (val as Value<T>)!.Val = Val;
      else
         val.SetDefault();
   }

   public override void SetDefault() { Val = DefaultValue; }
}

public interface IPCFLObject 
{
   public bool ParseWithReplacement(ScriptedTriggerSource parent, EnhancedBlock block, PathObj po) => throw new NotImplementedException();
   public bool ParseWithReplacement(ScriptedTriggerSource parent, LineKvp<string, string> command, PathObj po) => throw new NotImplementedException();
   // Scope?
   public bool Parse(EnhancedBlock block, PathObj po, ParsingContext context) => throw new NotImplementedException();
   public bool Parse(LineKvp<string, string> command, PathObj po, ParsingContext context) => throw new NotImplementedException();
};



public class Empty_Scope : IToken
{
   public void Activate(ITarget target) { }
   public void GetTokenString(int tabs, ref StringBuilder sb){}
   public string GetTokenName() => string.Empty;
   public string GetTokenDescription() => string.Empty;
   public string GetTokenExample() => string.Empty;
}

public interface IToken: IPCFLObject
{
   public static IToken Empty { get; } = new Empty_Scope();
   public void Activate(ITarget target);
   public void GetTokenString(int tabs, ref StringBuilder sb);
   public string GetTokenName();
   public string GetTokenDescription();
   public string GetTokenExample();
}

public interface ITrigger: IPCFLObject
{
   public static ITrigger Empty { get; }= AlwaysTrigger.TrueTrigger;
   public bool Evaluate(ITarget target);
}

public class Effect
{
   public ITarget Root;
   public ITarget From;
   public List<IToken> tokens { get; private set; }

   public Effect(ITarget root, ITarget from, List<IToken> tokens)
   {
      Root = root;
      From = from;
      this.tokens = tokens;
   }

   public static Effect Empty => new (Province.Empty, ITarget.Empty, []);

   private Effect(IEnumerable<IEnhancedElement> elements, PathObj po, PCFL_Scope scope, ITarget root, bool allowRoot = true) : this(elements, po, scope, root, ITarget.Empty, allowRoot) {}
   private Effect(IEnumerable<IEnhancedElement> elements, PathObj po, PCFL_Scope scope, ITarget root , ITarget from, bool allowRoot = true)
   {
      Root = allowRoot ? root : ITarget.Empty;
      From = from;
      tokens = GeneralFileParser.ParseElementsToTokens(elements, new (new SimpleFileScopeSwitch(scope, root), this), po);
   }

   public static Effect ConstructEffect(IEnumerable<IEnhancedElement> elements, PathObj po, PCFL_Scope scope, ITarget root, ITarget from, bool allowRoot = true) => new (elements, po, scope, root, from, allowRoot);
   public static Effect ConstructEffect(IEnumerable<IEnhancedElement> elements, PathObj po, PCFL_Scope scope, ITarget root, bool allowRoot = true) => new (elements, po, scope, root, allowRoot);
   public static Effect ConstructEffect(PathObj po, PCFL_Scope scope, ITarget root, bool allowRoot = true) => new(po.LoadBaseOrder(), po, scope, root, allowRoot);
   public void Activate()
   {
      foreach (var token in tokens) 
         token.Activate(Root);
   }
}

public struct ParsingContext(ScopeSwitch current, ScopeSwitch prev, ScopeSwitch prevPrev, Effect rootEffect)
{
   public ScopeSwitch This = current;
   public ScopeSwitch Prev = prev;
   public ScopeSwitch PrevPrev = prevPrev;
   public Effect Effect = rootEffect;

   public ParsingContext(ScopeSwitch current, Effect rootEffect) : this (current, ScopeSwitch.Empty, ScopeSwitch.Empty, rootEffect)
   {

   }

   public ParsingContext GetNext(ScopeSwitch next)
   {
      return new(next, This, Prev, Effect);
   }

   public static ParsingContext Empty => new (ScopeSwitch.Empty, Effect.Empty);
   public static ParsingContext ProvinceEmpty => new (new SimpleFileScopeSwitch(Scopes.Province, Province.Empty), Effect.Empty);
} 

public class AlwaysTrigger : ITrigger
{

   private bool _value;

   private AlwaysTrigger(bool value)
   {
      _value = value;
   }

   public static readonly AlwaysTrigger TrueTrigger = new (true);
   public static readonly AlwaysTrigger FalseTrigger = new (false);
   
   public bool Evaluate(ITarget target)
   {
      return _value;
   }
}