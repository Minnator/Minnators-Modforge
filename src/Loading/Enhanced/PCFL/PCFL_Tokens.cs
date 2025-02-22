using Editor.Saving;

namespace Editor.Loading.Enhanced.PCFL
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

   public class Effect
   {
      public List<PCFL_Token> Tokens = [];
   }

   public class ScopeSwitch : Effect // every_ and random_
   {

   }



   public abstract class Trigger<T> where T : ITarget // can only contain PCFL_Tokens which are scope switches, flow controls or triggers
   {
      public abstract bool Evaluate(T target);
   }

   public class TriggerScopeSwitch<T> : Trigger<T> where T : ITarget// all_ and any_
   {
      public override bool Evaluate(T target)
      {
         throw new NotImplementedException();
      }
   }

   public class BaseTrigger<T>(object[] values, Func<T, BaseTrigger<T>, bool> function) : Trigger<T> where T : ITarget
   {
      public override bool Evaluate(T target)
      {
         return function(target, this);
      }
   }


   // Parse -> Evalute Scripted: Define Blueprint -> Parse Blueprint to Trigger -> Evaluate
   // Scripted Triggers will be populated with values during Evaluation from a dictionary
   public class ScriptedTrigger
   {

   }

   public class PCFL_ComplexTrigger<T>(string[] options) where T : ITarget
   {

      public Func<T, BaseTrigger<T>, bool> TestFunc { get; init; }
      // checks for validity of the trigger and returns the values
      public Func<List<KeyValuePair<string, string>>, object[]> GetFunc { get; init; }

      public BaseTrigger<T> ConstructTrigger(List<KeyValuePair<string, string>> kvps)
      {
         return new(GetFunc(kvps), TestFunc);
      }


      public static void Test()
      {
         Func<T, BaseTrigger<T>, bool> function = (b, t) => { return true; };
         var estate_influence = new PCFL_ComplexTrigger<T>(["estate", "influence"])
         {
            TestFunc = function,
            GetFunc = (b) => { return [];}
         };




      }
   }

   public class TargetDummy : ITarget
   {

   }

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

   public class BooleanOperation<T>(Operation op) : Trigger<T> where T : ITarget // AND, OR, NOT
   {

      
      List<Trigger<T>> Triggers = [];

      public override bool Evaluate(T target)
      {
         return op switch
         {
            Operation.AND => Triggers.All(t => t.Evaluate(target)),
            Operation.OR => Triggers.Any(t => t.Evaluate(target)),
            Operation.NOT => !Triggers.All(t => t.Evaluate(target)),
            _ => throw new EvilActions("WTF is this operation? We don't do quantum stuff yet!")
         };
      }
   }
}