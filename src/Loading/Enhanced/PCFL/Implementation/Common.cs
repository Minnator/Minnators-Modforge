using System.Text;
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


public class Value<T>(T defaultValue) : Value(typeof(T)) where T : notnull
{
   public T Val = defaultValue;

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
      Val = val.Type == Type ? (val as Value<T>)!.Val : defaultValue;
   }

   public override void CopyTo(Value val)
   {
      if (val.Type == Type)
         (val as Value<T>)!.Val = Val;
      else
         val.SetDefault();
   }

   public override void SetDefault() { Val = defaultValue; }
}

public interface IPCFLObject 
{
   public bool ParseWithReplacement(ScriptedTriggerSource parent, EnhancedBlock block, PathObj po) => throw new NotImplementedException();
   public bool ParseWithReplacement(ScriptedTriggerSource parent, LineKvp<string, string> command, PathObj po) => throw new NotImplementedException();
   // Scope?
   public bool Parse(EnhancedBlock block, PathObj po) => throw new NotImplementedException();
   public bool Parse(LineKvp<string, string> command, PathObj po) => throw new NotImplementedException();
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

   private Effect(IEnumerable<IEnhancedElement> elements, PathObj po, PCFL_Scope scope, ITarget root) : this(elements, po, scope, root, ITarget.Empty) {}
   private Effect(IEnumerable<IEnhancedElement> elements, PathObj po, PCFL_Scope scope, ITarget root , ITarget from)
   {
      Root = root;
      From = from;
      tokens = GeneralFileParser.ParseElementsToTokens(elements, new (scope, this), po);
   }

   public static Effect ConstructEffect(IEnumerable<IEnhancedElement> elements, PathObj po, PCFL_Scope scope, ITarget root, ITarget from) => new (elements, po, scope, root, from);
   public static Effect ConstructEffect(IEnumerable<IEnhancedElement> elements, PathObj po, PCFL_Scope scope, ITarget root) => new (elements, po, scope, root);
   public static Effect ConstructEffect(PathObj po, PCFL_Scope scope, ITarget root) => new(po.LoadBaseOrder(), po, scope, root);
   public void Activate()
   {
      foreach (var token in tokens) 
         token.Activate(Root);
   }

   
}

public struct ParsingContext(PCFL_Scope current, ScopeSwitch prev, ScopeSwitch prevPrev, Effect rootEffect)
{
   public PCFL_Scope scope = current;
   public ScopeSwitch Prev = prev;
   public ScopeSwitch PrevPrev = prevPrev;
   public Effect Effect = rootEffect;

   public ParsingContext(PCFL_Scope current, Effect rootEffect) : this (current, ScopeSwitch.Empty, ScopeSwitch.Empty, rootEffect)
   {

   }

   public ParsingContext GetNext(ScopeSwitch next)
   {
      return new(scope, next, Prev, Effect);
   }
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