using Editor.ParadoxLanguage.Scope;

namespace Editor.ParadoxLanguage.Trigger
{
   public interface ITrigger
   {
      bool Evaluate(IScope scope);
   }

   public abstract class Trigger : ITrigger
   {
      public abstract bool Evaluate(IScope scope);
   }
}