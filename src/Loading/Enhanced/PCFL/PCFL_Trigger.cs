using System.Diagnostics;
using Editor.DataClasses.Saveables;
using Editor.ErrorHandling;
using Editor.Loading.Enhanced.PCFL.Scribbel;
using Editor.Saving;

namespace Editor.Loading.Enhanced.PCFL
{
   public class BaseManpowerTrigger : Trigger
   {
      public const string TRIGGER_NAME = "base_manpower";
      private const string TRIGGER_DESCRIPTION = "Returns true if the base manpower of the province is at least X.";
      private const string TRIGGER_EXAMPLE = "base_manpower = 3";
      private const ScopeType TRIGGER_SCOPE = ScopeType.Province;
      
      private Value<int> _baseManpower = new(0); // Default value and type of int


      public static Trigger? CreateTrigger(EnhancedBlock? block, LineKvp<string, string>? kvp, PCFL_Scope scope, PathObj po)
      {
         Debug.Assert(kvp is not null, "At this point the kvp must not be null. This must be filtered earlier in the pipeline");

         BaseManpowerTrigger trigger = new();
         return trigger.Parse(kvp.Value, po) ? trigger : null;
      }

      public override bool Parse(LineKvp<string, string> command, PathObj po)
      {
         return PCFL_TriggerParser.ParseSingleTriggerValue(ref _baseManpower, command, po, TRIGGER_NAME);
      }

      public override bool ParseWithReplacement(ScriptedTriggerSource parent, LineKvp<string, string> command, PathObj po)
      {
         return PCFL_TriggerParser.ParseSingleTriggerReplaceValue(parent, ref _baseManpower, command, po, TRIGGER_NAME);
      }

      public override bool Evaluate(ITarget target)
      {
         Debug.Assert(target is Province, $"'{TRIGGER_NAME}' trigger is only valid on provinces");
         return ((Province)target).BaseManpower >= _baseManpower.Val;
      }
   }

   public class BaseTaxTrigger : Trigger
   {
      public const string TRIGGER_NAME = "base_tax";
      private const string TRIGGER_DESCRIPTION = "Returns true if the base tax of the province is at least X.";
      private const string TRIGGER_EXAMPLE = "base_tax = 3";
      private const ScopeType TRIGGER_SCOPE = ScopeType.Province;
      
      private Value<int> _baseTax = new(0); // Default value and type of int


      public static Trigger? CreateTrigger(EnhancedBlock? block, LineKvp<string, string>? kvp, PCFL_Scope scope, PathObj po)
      {
         Debug.Assert(kvp is not null, "At this point the kvp must not be null. This must be filtered earlier in the pipeline");

         BaseManpowerTrigger trigger = new();
         return trigger.Parse(kvp.Value, po) ? trigger : null;
      }

      public override bool Parse(LineKvp<string, string> command, PathObj po)
      {
         return PCFL_TriggerParser.ParseSingleTriggerValue(ref _baseTax, command, po, TRIGGER_NAME);
      }

      public override bool ParseWithReplacement(ScriptedTriggerSource parent, LineKvp<string, string> command, PathObj po)
      {
         return PCFL_TriggerParser.ParseSingleTriggerReplaceValue(parent, ref _baseTax, command, po, TRIGGER_NAME);
      }

      public override bool Evaluate(ITarget target)
      {
         Debug.Assert(target is Province, $"'{TRIGGER_NAME}' trigger is only valid on provinces");
         return ((Province)target).BaseTax >= _baseTax.Val;
      }
   }
}