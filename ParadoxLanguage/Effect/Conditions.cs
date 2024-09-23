using Editor.ParadoxLanguage.Scope;
using Editor.ParadoxLanguage.Trigger;

namespace Editor.ParadoxLanguage.Effect
{
   public class IfEffect(ITrigger trigger, Effect effect) : Effect
   {
      public override void Apply(IScope scope)
      {
         if (trigger.Evaluate(scope))
         {
            effect.Apply(scope);
            HasExecuted = true; // Mark as executed
         }
      }
   }

   public class ElseIfEffect : Effect
   {
      private readonly Trigger.Trigger _trigger;
      private readonly Effect _effect;
      private readonly Effect _previousEffect;

      public ElseIfEffect(Trigger.Trigger trigger, Effect effect, Effect previousEffect)
      {
         _trigger = trigger;
         _effect = effect;
         _previousEffect = previousEffect;
      }

      public override void Apply(IScope scope)
      {
         if (!_previousEffect.HasExecuted && _trigger.Evaluate(scope))
         {
            _effect.Apply(scope);
            HasExecuted = true; // Mark as executed
         }
      }
   }

   public class ElseEffect(Effect[] previousEffects, Effect effect) : Effect
   {
      public override void Apply(IScope scope)
      {
         if (previousEffects.All(e => !e.HasExecuted)) // Check if all previous effects were not executed
         {
            effect.Apply(scope);
            HasExecuted = true; // Mark as executed
         }
      }
   }



}