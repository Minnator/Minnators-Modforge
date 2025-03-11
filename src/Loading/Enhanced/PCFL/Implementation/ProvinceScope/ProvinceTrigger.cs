using Editor.DataClasses.Saveables;
using Editor.Saving;
using System.Diagnostics;

namespace Editor.Loading.Enhanced.PCFL.Implementation.ProvinceScope;

public class BaseManpowerTrigger : ITrigger
{
   public const string TRIGGER_NAME = "base_manpower";
   private const string TRIGGER_DESCRIPTION = "Returns true if the base manpower of the province is at least X.";
   private const string TRIGGER_EXAMPLE = "base_manpower = 3";
   private const ScopeType TRIGGER_SCOPE = ScopeType.Province;

   private Value<int> _baseManpower = new(0); // Default value and type of int


   public static ITrigger? CreateTrigger(EnhancedBlock? block, LineKvp<string, string>? kvp, ParsingContext context, PathObj po)
   {
      Debug.Assert(kvp is not null, "At this point the kvp must not be null. This must be filtered earlier in the pipeline");

      BaseManpowerTrigger trigger = new();
      return trigger.Parse(kvp.Value, po, context) ? trigger : null;
   }

   public bool Parse(LineKvp<string, string> command, PathObj po, ParsingContext context)
   {
      return GeneralFileParser.ParseSingleTriggerValue(ref _baseManpower, command, po);
   }

   public bool ParseWithReplacement(ScriptedTriggerSource parent, LineKvp<string, string> command, PathObj po)
   {
      return GeneralFileParser.ParseSingleTriggerReplaceValue(parent, ref _baseManpower, command, po, TRIGGER_NAME);
   }

   public bool Evaluate(ITarget target)
   {
      Debug.Assert(target is Province, $"'{TRIGGER_NAME}' trigger is only valid on provinces");
      return ((Province)target).BaseManpower >= _baseManpower.Val;
   }
}

public class BaseTaxTrigger : ITrigger
{
   public const string TRIGGER_NAME = "base_tax";
   private const string TRIGGER_DESCRIPTION = "Returns true if the base tax of the province is at least X.";
   private const string TRIGGER_EXAMPLE = "base_tax = 3";
   private const ScopeType TRIGGER_SCOPE = ScopeType.Province;

   private Value<int> _baseTax = new(0); // Default value and type of int


   public static ITrigger? CreateTrigger(EnhancedBlock? block, LineKvp<string, string>? kvp, ParsingContext context, PathObj po)
   {
      Debug.Assert(kvp is not null, "At this point the kvp must not be null. This must be filtered earlier in the pipeline");

      BaseManpowerTrigger trigger = new();
      return trigger.Parse(kvp.Value, po, context) ? trigger : null;
   }

   public bool Parse(LineKvp<string, string> command, PathObj po)
   {
      return GeneralFileParser.ParseSingleTriggerValue(ref _baseTax, command, po);
   }

   public bool ParseWithReplacement(ScriptedTriggerSource parent, LineKvp<string, string> command, PathObj po)
   {
      return GeneralFileParser.ParseSingleTriggerReplaceValue(parent, ref _baseTax, command, po, TRIGGER_NAME);
   }

   public bool Evaluate(ITarget target)
   {
      Debug.Assert(target is Province, $"'{TRIGGER_NAME}' trigger is only valid on provinces");
      return ((Province)target).BaseTax >= _baseTax.Val;
   }
}
