
using System.Diagnostics;
using Editor.DataClasses.Saveables;
using Editor.Saving;

namespace Editor.Loading.Enhanced.PCFL.Implementation.ProvinceScope;

public class BaseManpowerEffect : IToken
{
   public const string EFFECT_NAME = "add_base_manpower";
   private const string EFFECT_DESCRIPTION = "Adds base manpower to the current province scope.";
   private const string EFFECT_EXAMPLE = "add_base_manpower = 3";
   private const ScopeType EFFECT_SCOPE = ScopeType.Province;

   private Value<int> _baseManpower = new(0); // Default value and type of int


   public static IToken? CreateEffect(EnhancedBlock? block, LineKvp<string, string>? kvp, PCFL_Scope scope, PathObj po)
   {
      Debug.Assert(kvp is not null, "At this point the kvp must not be null. This must be filtered earlier in the pipeline");

      BaseManpowerEffect token = new();
      return token.Parse(kvp.Value, po) ? token : null;
   }

   public bool Parse(LineKvp<string, string> command, PathObj po)
   {
      return GeneralFileParser.ParseSingleTriggerValue(ref _baseManpower, command, po, EFFECT_NAME);
   }

   public bool ParseWithReplacement(ScriptedTriggerSource parent, LineKvp<string, string> command, PathObj po)
   {
      return GeneralFileParser.ParseSingleTriggerReplaceValue(parent, ref _baseManpower, command, po, EFFECT_NAME);
   }

   public void Activate(ITarget target)
   {
      Debug.Assert(target is Province, $"'{EFFECT_NAME}' effect is only valid on provinces");
      ((Province)target).BaseManpower += _baseManpower.Val;
   }
}