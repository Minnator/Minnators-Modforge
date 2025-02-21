namespace Editor.Loading.Enhanced.PCFL
{
   /*
   public interface PCFL_TargetProvider
   {
      List<ITarget> GetAllTargets(out PCFL_Scope newScope, ITarget currentTarget);
   }


   public class DynamicPropertyScopeProvider : PCFL_TargetProvider
   {
      PCFL_Scope _scope;
      PropertyInfo _info;

      public DynamicPropertyScopeProvider(PCFL_Scope scope, PropertyInfo info)
      {
         _scope = scope;
         _info = info;
      }

      public List<ITarget> GetAllTargets(out PCFL_Scope newScope, ITarget currentTarget)
      {
         newScope = _scope;
         var target = (ITarget)_info.GetValue(currentTarget)!;
         return [target];
      }
   }

   public class StaticPropertyScopeProvider(PCFL_Scope scope, List<ITarget> targets)
   {
      public List<ITarget> GetAllTargets(out PCFL_Scope newScope, ITarget _)
      {
         newScope = scope;
         return targets;
      }
   }

   */
   public interface ITarget 
   {

   }
}