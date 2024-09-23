using Editor.ParadoxLanguage.Scope;

namespace Editor.ParadoxLanguage.Trigger
{
   public class AndTrigger(IEnumerable<Trigger> triggers) : Trigger
   {
      private readonly List<Trigger> _triggers = [..triggers];

      public override bool Evaluate(IScope scope)
      {
         foreach (var trigger in _triggers)
            if (!trigger.Evaluate(scope))
               return false; // If any trigger is false, return false
         return true; // All triggers are true
      }
   }

   public class OrTrigger(IEnumerable<Trigger> triggers) : Trigger
   {
      private readonly List<Trigger> _triggers = [..triggers];

      public override bool Evaluate(IScope scope)
      {
         foreach (var trigger in _triggers)
            if (trigger.Evaluate(scope))
               return true; // If any trigger is true, return true
         return false; // All triggers are false
      }
   }

   public class NotTrigger(Trigger trigger) : Trigger
   {
      private readonly Trigger _trigger = trigger;

      public override bool Evaluate(IScope scope)
      {
         return !_trigger.Evaluate(scope);
      }
   }

   public class IfTrigger(Trigger condition) : Trigger
   {
      public override bool Evaluate(IScope scope)
      {
         return condition.Evaluate(scope);
      }
   }


}