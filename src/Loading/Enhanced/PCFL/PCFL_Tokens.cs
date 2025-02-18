namespace Editor.src.Loading.Enhanced.PCFL
{
   public abstract class PCFL_Token
   {
      
   }

   /*
    *
    * Trigger objects will be saved seperatly in a TokenCollection with a list of tokens(either scope switch or effect) and a optional LimitObject with all the triggers
    *
   */

   public class PCFL_ScopeSwitch_Token(PCFL_TargetProvider provider, List<PCFL_Token> internalTokens) : PCFL_Token
   {
   }

   /*
    * limits are not positional, and all must be true for several limits in one effect/scope/trigger
    */
}