namespace Editor.ParadoxLanguage.Scope
{
   public enum ScopeEffector
   {
      Country,
      Province,
      Unit,
   }


   public interface IScope
   {
      bool EvaluateTrigger(Trigger.Trigger trigger);
      void ApplyEffect(Effect.Effect effect);
      IScope Rescope(ScopeEffector effector, string scopeType);
   }

   public abstract class Scope(IScope root) : IScope
   {
      public virtual IScope Root { get; init; } = root;
      public virtual IScope? From { get; init; }
      public abstract bool EvaluateTrigger(Trigger.Trigger trigger);
      public abstract void ApplyEffect(Effect.Effect effect);

      // Parses the scopeType and returns a new scope of that type if the context allows it
      public virtual IScope Rescope(ScopeEffector effector, string scopeType)
      {
         throw new 
            
            
            
            
            
            
            
            ();
         return this;
      }
   }
}