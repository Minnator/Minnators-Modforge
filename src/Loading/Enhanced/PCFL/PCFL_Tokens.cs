using Editor.DataClasses.GameDataClasses;
using Editor.Saving;

namespace Editor.Loading.Enhanced.PCFL
{

   // Only used for ScopeSwitches, Actions and FlowControls
   public abstract class PCFL_Token
   {
      public abstract void Activate(ITarget target);
   }

   /*
    * One Superclass to handle Scope switches, Actions, FlowControls
    * -> All of them have an Activate
    *
    * Triggers have scope switches
    * -> All of them have an Evaluate
    *
    */

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


   // Token Sequence(Effect): List<Tokens> token: Actions / FlowControls / Scopeswitches
   // Action does not contain any further effects
   // Flow Control: Contains one or multiple sub Effects (Token Sequences)
   // Scope Switch: Contains exactly one sub Effect (Token Sequences)

   // Trigger(AND): Boolean, ScopeSwitch, BaseTrigger 

   /*limit = {
    *    every_owned_provice={
    *       base_tax = 0
    *    }
    * }
    * limits are not positional, and all must be true for several limits in one effect/scope/trigger
    */

   /*
      estate_influence = {
          estate = estate_church
          influence = 80
      }
    */
   
   // BaseTrigger ParadoxTrigger (hardcoded)
   // ScriptedTrigger user defined benutzen Trigger

   // PCFL Trigger uses Func to generate bools. 
   // Func(target, value), name
   // array of option strings
   // array of object values
   // options=["estate","influence", "optional"]
   // values=[estate_church(obj), 80, Value.Empty]

   public enum Operation
   {
      AND,
      OR,
      NOT
   }

}