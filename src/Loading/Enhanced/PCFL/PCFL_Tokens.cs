namespace Editor.src.Loading.Enhanced.PCFL
{
   public abstract class PCFL_Token
   {
      
   }

   /*
    *
    * Trigger objects will be saved separately in a TokenCollection with a list of tokens(either scope switch or effect) and an optional LimitObject with all the triggers
    *
   */

   /*
    * Effect contains any actions, triggers, scope switches
    *
    * Trigger  can contain triggers, scope switches and boolean operators
    * Scopes can contain scopes
    * Actions is a key value pair
    * Limit contains a trigger list (AND)
    * AND / OR / NOT (NAND) contains triggers and is one itself
    * These need hardcoding (flow control)
    * If
    * Else
    * Else_if
    * While
    * Trigger_switch
    * Random
    * Random_list
    */


   // Token Sequence(Effect): List<Tokens> token: Actions/ FlowControls/ Scopeswitches
   // Action does not contain any further effects
   // Flow Control: Contains one or multiple sub Effects (Token Sequences)
   // Scope Switch: Contains exactly one sub Effect (Token Sequences)

   // Trigger(AND): Boolean, ScopeSwitch, BaseTrigger 

   /*limit = {
    *    every_owned_provice={
    *       base_tax = 0
    *    }
    * }
    */

   public class PCFL_Effect : PCFL_Token
   {

   }

   public interface IEffectTrigger
   {
      public List<PCFL_Trigger> Trigger { get; set; }
      public List<PCFL_Effect> Actions { get; set; }
   }


   public class PCFL_TokenSequence()
   {
      public List<PCFL_Token> Tokens = [];
      public List<PCFL_TriggerToken> Triggers = []; 
   }


   // Have to be handled separately
   public class PCFL_TriggerToken() : PCFL_Token
   {
      //public List<PCFL_Trigger> 

      public static PCFL_TriggerToken Empty { get; } = new ();
   }

   public class PCFL_EffectToken() : PCFL_Token
   {
      public static PCFL_EffectToken Empty { get; } = new();
   }
   
   public class PCFL_ScopeSwitchToken : PCFL_Token
   {
      //public PCFL_TargetProvider Provider { get; }
      public PCFL_TokenSequence InternalTokens { get; }

      
      public static PCFL_ScopeSwitchToken Empty { get; } = new();
   }

   /*
    * limits are not positional, and all must be true for several limits in one effect/scope/trigger
    */
}