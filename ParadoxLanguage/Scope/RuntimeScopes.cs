using Editor.DataClasses.GameDataClasses;

namespace Editor.ParadoxLanguage.Scope
{
   public class AreaScope(Area area, List<Trigger.Trigger> triggers) : IScope
   {
      public bool EvaluateTrigger(Trigger.Trigger trigger)
      {
         return true; // Placeholder
      }

      public void ApplyEffect(Effect.Effect effect)
      {
         foreach (var id in area.GetProvinces()) 
            id.ApplyEffect(effect);
      }

      public IScope Rescope(ScopeEffector effector, string scopeType)
      {
         throw new NotImplementedException();
      }
   }

}