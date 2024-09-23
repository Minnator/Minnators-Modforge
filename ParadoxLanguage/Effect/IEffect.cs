using Editor.ParadoxLanguage.Scope;

namespace Editor.ParadoxLanguage.Effect
{
   public interface IEffect
   {
      void Apply(IScope scope);
   }

   public abstract class Effect : IEffect
   {
      public bool HasExecuted { get; set; } = false; // Flag to track complex executions
      public abstract void Apply(IScope scope);
   }
}