
using System.Diagnostics;
using System.Text;
using Editor.DataClasses.GameDataClasses;
using Editor.DataClasses.Saveables;
using Editor.DataClasses.Settings;
using Editor.ErrorHandling;
using Editor.Helper;
using Editor.Parser;
using Editor.Saving;

namespace Editor.Loading.Enhanced.PCFL.Implementation.ProvinceScope;

public class AddBaseManpowerEffect : SimpleIntEffect
{
   public const string EFFECT_NAME = "add_base_manpower";
   private const string EFFECT_DESCRIPTION = "Adds base manpower to the current province scope.";
   private const string EFFECT_EXAMPLE = "add_base_manpower = 3";
   private const ScopeType EFFECT_SCOPE = ScopeType.Province;
   
   public static IToken? CreateEffect(EnhancedBlock? block, LineKvp<string, string>? kvp, PCFL_Scope scope, PathObj po)
   {
      Debug.Assert(kvp is not null, "At this point the kvp must not be null. This must be filtered earlier in the pipeline");

      AddBaseManpowerEffect token = new();
      return token.Parse(kvp.Value, po) ? token : null;
   }

   public override void Activate(ITarget target)
   {
      Debug.Assert(target is Province, $"'{EFFECT_NAME}' effect is only valid on provinces");
      ((Province)target).BaseManpower += _value.Val;
   }

   public override string GetTokenName() => EFFECT_NAME;
   public override string GetTokenDescription() => EFFECT_DESCRIPTION;
   public override string GetTokenExample() => EFFECT_EXAMPLE;
}

public class AddBaseTaxEffect : SimpleIntEffect
{
   public const string EFFECT_NAME = "add_base_tax";
   private const string EFFECT_DESCRIPTION = "Adds base tax to the current province scope.";
   private const string EFFECT_EXAMPLE = "add_base_tax = 3";
   private const ScopeType EFFECT_SCOPE = ScopeType.Province;

   public static IToken? CreateEffect(EnhancedBlock? block, LineKvp<string, string>? kvp, PCFL_Scope scope, PathObj po)
   {
      Debug.Assert(kvp is not null, "At this point the kvp must not be null. This must be filtered earlier in the pipeline");

      AddBaseTaxEffect token = new();
      return token.Parse(kvp.Value, po) ? token : null;
   }

   public override void Activate(ITarget target)
   {
      Debug.Assert(target is Province, $"'{EFFECT_NAME}' effect is only valid on provinces");
      ((Province)target).BaseTax += _value.Val;
   }

   public override string GetTokenName() => EFFECT_NAME;
   public override string GetTokenDescription() => EFFECT_DESCRIPTION;
   public override string GetTokenExample() => EFFECT_EXAMPLE;
}

public class AddBaseProductionEffect : SimpleIntEffect
{
   public const string EFFECT_NAME = "add_base_production";
   private const string EFFECT_DESCRIPTION = "Adds base production to the current province scope.";
   private const string EFFECT_EXAMPLE = "add_base_production = 3";
   private const ScopeType EFFECT_SCOPE = ScopeType.Province;

   public static IToken? CreateEffect(EnhancedBlock? block, LineKvp<string, string>? kvp, PCFL_Scope scope, PathObj po)
   {
      Debug.Assert(kvp is not null, "At this point the kvp must not be null. This must be filtered earlier in the pipeline");

      AddBaseProductionEffect token = new();
      return token.Parse(kvp.Value, po) ? token : null;
   }

   public override void Activate(ITarget target)
   {
      Debug.Assert(target is Province, $"'{EFFECT_NAME}' effect is only valid on provinces");
      ((Province)target).BaseProduction += _value.Val;
   }

   public override string GetTokenName() => EFFECT_NAME;
   public override string GetTokenDescription() => EFFECT_DESCRIPTION;
   public override string GetTokenExample() => EFFECT_EXAMPLE;
}

public class BaseManpowerEffect : SimpleIntEffect
{
   public const string EFFECT_NAME = "base_manpower";
   private const string EFFECT_DESCRIPTION = "Sets the base_manpower to the current province scope. ONLY in province histrory files.";
   private const string EFFECT_EXAMPLE = "base_manpower = 3";
   private const ScopeType EFFECT_SCOPE = ScopeType.Province;
   
   public static IToken? CreateEffect(EnhancedBlock? block, LineKvp<string, string>? kvp, PCFL_Scope scope, PathObj po)
   {
      Debug.Assert(kvp is not null, "At this point the kvp must not be null. This must be filtered earlier in the pipeline");

      BaseManpowerEffect token = new();
      return token.Parse(kvp.Value, po) ? token : null;
   }

   public override void Activate(ITarget target)
   {
      Debug.Assert(target is Province, $"'{EFFECT_NAME}' effect is only valid on provinces");
      ((Province)target).BaseManpower = _value.Val;
   }

   public override string GetTokenName() => EFFECT_NAME;
   public override string GetTokenDescription() => EFFECT_DESCRIPTION;
   public override string GetTokenExample() => EFFECT_EXAMPLE;
}

public class BaseTaxEffect : SimpleIntEffect
{
   public const string EFFECT_NAME = "base_tax";
   private const string EFFECT_DESCRIPTION = "Sets base tax to the current province scope. ONLY in province histrory files.";
   private const string EFFECT_EXAMPLE = "base_tax = 3";
   private const ScopeType EFFECT_SCOPE = ScopeType.Province;

   public static IToken? CreateEffect(EnhancedBlock? block, LineKvp<string, string>? kvp, PCFL_Scope scope, PathObj po)
   {
      Debug.Assert(kvp is not null, "At this point the kvp must not be null. This must be filtered earlier in the pipeline");

      BaseTaxEffect token = new();
      return token.Parse(kvp.Value, po) ? token : null;
   }

   public override void Activate(ITarget target)
   {
      Debug.Assert(target is Province, $"'{EFFECT_NAME}' effect is only valid on provinces");
      ((Province)target).BaseTax = _value.Val;
   }

   public override string GetTokenName() => EFFECT_NAME;
   public override string GetTokenDescription() => EFFECT_DESCRIPTION;
   public override string GetTokenExample() => EFFECT_EXAMPLE;
}

public class BaseProductionEffect : SimpleIntEffect
{
   public const string EFFECT_NAME = "base_production";
   private const string EFFECT_DESCRIPTION = "Sets base production to the current province scope. ONLY in province histrory files.";
   private const string EFFECT_EXAMPLE = "base_production = 3";
   private const ScopeType EFFECT_SCOPE = ScopeType.Province;

   public static IToken? CreateEffect(EnhancedBlock? block, LineKvp<string, string>? kvp, PCFL_Scope scope, PathObj po)
   {
      Debug.Assert(kvp is not null, "At this point the kvp must not be null. This must be filtered earlier in the pipeline");

      BaseProductionEffect token = new();
      return token.Parse(kvp.Value, po) ? token : null;
   }

   public override void Activate(ITarget target)
   {
      Debug.Assert(target is Province, $"'{EFFECT_NAME}' effect is only valid on provinces");
      ((Province)target).BaseProduction = _value.Val;
   }

   public override string GetTokenName() => EFFECT_NAME;
   public override string GetTokenDescription() => EFFECT_DESCRIPTION;
   public override string GetTokenExample() => EFFECT_EXAMPLE;
}

public class OwnerEffect : SimpleTagEffect
{
   public const string EFFECT_NAME = "owner";
   private const string EFFECT_DESCRIPTION = "Sets the owner of the province. ONLY valid in province history files";
   private const string EFFECT_EXAMPLE = $"{EFFECT_NAME} = TAG";
   private const ScopeType EFFECT_SCOPE = ScopeType.Province;
   
   public static IToken? CreateEffect(EnhancedBlock? block, LineKvp<string, string>? kvp, PCFL_Scope scope, PathObj po)
   {
      Debug.Assert(kvp is not null, "At this point the kvp must not be null. This must be filtered earlier in the pipeline");

      OwnerEffect token = new();
      return token.Parse(kvp.Value, po) ? token : null;
   }

   public override void Activate(ITarget target)
   {
      Debug.Assert(target is Province, $"'{EFFECT_NAME}' effect is only valid on provinces");
      if (Tag.TryParse(_value.Val, out var tag) && Globals.Countries.TryGetValue(tag, out var country))
         ((Province)target).Owner = country;
      else
         _ = new ErrorObject(type: ErrorType.PCFL_TokenValidationError, $"value for {EFFECT_NAME} is not of type Tag!");
   }

   public override string GetTokenName() => EFFECT_NAME;
   public override string GetTokenDescription() => EFFECT_DESCRIPTION;
   public override string GetTokenExample() => EFFECT_EXAMPLE;
}

public class ControllerEffect : SimpleTagEffect
{
   public const string EFFECT_NAME = "controller";
   private const string EFFECT_DESCRIPTION = $"Sets the {EFFECT_NAME} of the province. ONLY valid in province history files";
   private const string EFFECT_EXAMPLE = $"{EFFECT_NAME} = TAG";
   private const ScopeType EFFECT_SCOPE = ScopeType.Province;
   
   public static IToken? CreateEffect(EnhancedBlock? block, LineKvp<string, string>? kvp, PCFL_Scope scope, PathObj po)
   {
      Debug.Assert(kvp is not null, "At this point the kvp must not be null. This must be filtered earlier in the pipeline");

      ControllerEffect token = new();
      return token.Parse(kvp.Value, po) ? token : null;
   }

   public override void Activate(ITarget target)
   {
      Debug.Assert(target is Province, $"'{EFFECT_NAME}' effect is only valid on provinces");
      if (Tag.TryParse(_value.Val, out var tag) && Globals.Countries.TryGetValue(tag, out var country))
         ((Province)target).Controller = country;
      else
         _ = new ErrorObject(type: ErrorType.PCFL_TokenValidationError, $"value for {EFFECT_NAME} is not of type Tag!");
   }

   public override string GetTokenName() => EFFECT_NAME;
   public override string GetTokenDescription() => EFFECT_DESCRIPTION;
   public override string GetTokenExample() => EFFECT_EXAMPLE;
}

public class AddCoreEffect : SimpleTagEffect
{
   public const string EFFECT_NAME = "add_core";
   private const string EFFECT_DESCRIPTION = $"<scope>The country that gains the core.";
   private const string EFFECT_EXAMPLE = $"{EFFECT_NAME} = TAG";
   private const ScopeType EFFECT_SCOPE = ScopeType.Province;
   
   public static IToken? CreateEffect(EnhancedBlock? block, LineKvp<string, string>? kvp, PCFL_Scope scope, PathObj po)
   {
      Debug.Assert(kvp is not null, "At this point the kvp must not be null. This must be filtered earlier in the pipeline");

      AddCoreEffect token = new();
      return token.Parse(kvp.Value, po) ? token : null;
   }

   public override void Activate(ITarget target)
   {
      Debug.Assert(target is Province, $"'{EFFECT_NAME}' effect is only valid on provinces");
      if (Tag.TryParse(_value.Val, out var tag))
         ((Province)target).Cores.Add(tag);
      else
         _ = new ErrorObject(type: ErrorType.PCFL_TokenValidationError, $"value for {EFFECT_NAME} is not of type Tag!");
   }

   public override string GetTokenName() => EFFECT_NAME;
   public override string GetTokenDescription() => EFFECT_DESCRIPTION;
   public override string GetTokenExample() => EFFECT_EXAMPLE;
}

public class RemoveCoreEffect : SimpleTagEffect
{
   public const string EFFECT_NAME = "remove_core";
   private const string EFFECT_DESCRIPTION = $"<scope>The country that loses their core.";
   private const string EFFECT_EXAMPLE = $"{EFFECT_NAME} = TAG";
   private const ScopeType EFFECT_SCOPE = ScopeType.Province;
   
   public static IToken? CreateEffect(EnhancedBlock? block, LineKvp<string, string>? kvp, PCFL_Scope scope, PathObj po)
   {
      Debug.Assert(kvp is not null, "At this point the kvp must not be null. This must be filtered earlier in the pipeline");

      RemoveCoreEffect token = new();
      return token.Parse(kvp.Value, po) ? token : null;
   }

   public override void Activate(ITarget target)
   {
      Debug.Assert(target is Province, $"'{EFFECT_NAME}' effect is only valid on provinces");
      if (Tag.TryParse(_value.Val, out var tag))
         ((Province)target).Cores.Remove(tag);
      else
         _ = new ErrorObject(type: ErrorType.PCFL_TokenValidationError, $"value for {EFFECT_NAME} is not of type Tag!");
   }

   public override string GetTokenName() => EFFECT_NAME;
   public override string GetTokenDescription() => EFFECT_DESCRIPTION;
   public override string GetTokenExample() => EFFECT_EXAMPLE;
}

public class UnrestEffect : SimpleIntEffect
{
   public const string EFFECT_NAME = "unrest";
   private const string EFFECT_DESCRIPTION = $"Sets the {EFFECT_NAME} of the province. ONLY valid in province history files";
   private const string EFFECT_EXAMPLE = $"{EFFECT_NAME} = 2";
   private const ScopeType EFFECT_SCOPE = ScopeType.Province;

   public static IToken? CreateEffect(EnhancedBlock? block, LineKvp<string, string>? kvp, PCFL_Scope scope, PathObj po)
   {
      Debug.Assert(kvp is not null, "At this point the kvp must not be null. This must be filtered earlier in the pipeline");

      UnrestEffect token = new();
      return token.Parse(kvp.Value, po) ? token : null;
   }

   public override void Activate(ITarget target)
   {
      Debug.Assert(target is Province, $"'{EFFECT_NAME}' effect is only valid on provinces");
      ((Province)target).RevoltRisk = _value.Val;
   }

   public override string GetTokenName() => EFFECT_NAME;
   public override string GetTokenDescription() => EFFECT_DESCRIPTION;
   public override string GetTokenExample() => EFFECT_EXAMPLE;
}

public class RevoltRiskEffect : SimpleIntEffect
{
   public const string EFFECT_NAME = "revolt_risk";
   private const string EFFECT_DESCRIPTION = $"Sets the {EFFECT_NAME} of the province. ONLY valid in province history files. DEPRECATED";
   private const string EFFECT_EXAMPLE = $"{EFFECT_NAME} = 1";
   private const ScopeType EFFECT_SCOPE = ScopeType.Province;

   public static IToken? CreateEffect(EnhancedBlock? block, LineKvp<string, string>? kvp, PCFL_Scope scope, PathObj po)
   {
      Debug.Assert(kvp is not null, "At this point the kvp must not be null. This must be filtered earlier in the pipeline");

      _ = new LoadingError(po, $"\"{EFFECT_NAME}\" is deprecated. Please use \"unrest\" instead.", line:kvp.Value.Line, type:ErrorType.PCFL_Deprecated, level: LogType.Warning);

      UnrestEffect token = new();
      return token.Parse(kvp.Value, po) ? token : null;
   }

   public override void Activate(ITarget target)
   {
      Debug.Assert(target is Province, $"'{EFFECT_NAME}' effect is only valid on provinces");
      ((Province)target).RevoltRisk = _value.Val;
   }

   public override string GetTokenName() => EFFECT_NAME;
   public override string GetTokenDescription() => EFFECT_DESCRIPTION;
   public override string GetTokenExample() => EFFECT_EXAMPLE;
}

public class RevoltEffect : IToken
{
   public const string EFFECT_NAME = "revolt";
   private const string EFFECT_DESCRIPTION = $"Adds a revolt";
   private const string EFFECT_EXAMPLE = $"revolt = {{\n\ttype = <rebel type>\n\tsize = <int>\n\tleader = <string>\n}}";
   private const ScopeType EFFECT_SCOPE = ScopeType.Province;

   internal Value<string> type = new(string.Empty);
   internal Value<float> size = new(-1);
   internal Value<string> leader = new(string.Empty);
   internal Value<string> name = new(string.Empty);

   public static IToken? CreateEffect(EnhancedBlock? block, LineKvp<string, string>? kvp, PCFL_Scope scope, PathObj po)
   {
      Debug.Assert(block is not null, "At this point the block must not be null. This must be filtered earlier in the pipeline");

      RevoltEffect token = new();
      return token.Parse(block, po) ? token : null;
   }

   public bool Parse(EnhancedBlock block, PathObj po)
   {
      foreach (var element in block.GetContentElements(true, po))
      {
         foreach (var kvp in element.GetLineKvpEnumerator(po))
         {
            switch (kvp.Key)
            {
               case "type":
               {
                  if (!GeneralFileParser.ParseSingleTriggerValue(ref type, kvp, po, EFFECT_NAME))
                     return false;
                  break;
               }
               case "size":
               {
                  if (!GeneralFileParser.ParseSingleTriggerValue(ref size, kvp, po, EFFECT_NAME))
                     return false;
                  break;
               }
               case "leader":
               {
                  if (!GeneralFileParser.ParseSingleTriggerValue(ref leader, kvp, po, EFFECT_NAME))
                     return false;
                  break;
               }
               case "name":
               {
                  if (!GeneralFileParser.ParseSingleTriggerValue(ref name, kvp, po, EFFECT_NAME))
                     return false;
                  break;
               }
               default:
                  _ = new LoadingError(po, $"Invalid key in {EFFECT_NAME}: {kvp.Key}", line: kvp.Line, type: ErrorType.PCFL_TokenValidationError);
                  return false;
            }
         }
      }
      return false;
   }

   public bool ParseReplace()
   {
      return false;
   }

   public void Activate(ITarget target)
   {
      Debug.Assert(target is Province, $"'{GetTokenName()}' effect is only valid on provinces");

      ((Province)target).HasRevolt = true;
   }

   public void GetTokenString(int tabs, ref StringBuilder sb)
   {
      SavingUtil.OpenBlock(ref tabs, EFFECT_NAME, ref sb);
      SavingUtil.AddString(tabs, type.Val, "type", ref sb);
      SavingUtil.AddFloatIfNotValue(tabs, size.Val, "size", -1, ref sb);
      SavingUtil.AddQuotedString(tabs, leader.Val, "leader", ref sb);
      SavingUtil.AddQuotedString(tabs, name.Val, "name", ref sb);
      SavingUtil.CloseBlock(ref tabs, ref sb);
   }

   public string GetTokenName() => EFFECT_NAME;
   public string GetTokenDescription() => EFFECT_DESCRIPTION;
   public string GetTokenExample() => EFFECT_EXAMPLE;
}

public class ReligionEffect : SimpleStringEffect
{
   public const string EFFECT_NAME = "religion";
   private const string EFFECT_DESCRIPTION = $"Sets the {EFFECT_NAME} of the province. ONLY valid in province history files.";
   private const string EFFECT_EXAMPLE = $"{EFFECT_NAME} = <religion>";
   private const ScopeType EFFECT_SCOPE = ScopeType.Province;

   public static IToken? CreateEffect(EnhancedBlock? block, LineKvp<string, string>? kvp, PCFL_Scope scope, PathObj po)
   {
      Debug.Assert(kvp is not null, "At this point the kvp must not be null. This must be filtered earlier in the pipeline");

      ReligionEffect token = new();
      return token.Parse(kvp.Value, po) ? token : null;
   }

   public override bool Parse(LineKvp<string, string> command, PathObj po)
   {
      if (Religion.TryParse(command.Value, out _).Then(o => o.ConvertToLoadingError(po, "", line: command.Line)))
      {
         _value.Val = command.Value;
         return true;
      }
      return false;
   }

   public override void Activate(ITarget target)
   {
      Debug.Assert(target is Province, $"'{EFFECT_NAME}' effect is only valid on provinces");
      if (Religion.TryParse(_value.Val, out var religion).Log())
         ((Province)target).Religion = religion;
   }

   public override string GetTokenName() => EFFECT_NAME;
   public override string GetTokenDescription() => EFFECT_DESCRIPTION;
   public override string GetTokenExample() => EFFECT_EXAMPLE;
}

public class ReformationCenterEffect : SimpleStringEffect
{
   public const string EFFECT_NAME = "reformation_center";
   private const string EFFECT_DESCRIPTION = $"Sets the {EFFECT_NAME} of the province. ONLY valid in province history files.";
   private const string EFFECT_EXAMPLE = $"{EFFECT_NAME} = <religion>";
   private const ScopeType EFFECT_SCOPE = ScopeType.Province;

   public static IToken? CreateEffect(EnhancedBlock? block, LineKvp<string, string>? kvp, PCFL_Scope scope, PathObj po)
   {
      Debug.Assert(kvp is not null, "At this point the kvp must not be null. This must be filtered earlier in the pipeline");
      ReligionEffect token = new();
      return token.Parse(kvp.Value, po) ? token : null;
   }

   public override bool Parse(LineKvp<string, string> command, PathObj po)
   {
      // TODO check if religion allows the reformation center

      if (Religion.TryParse(command.Value, out _).Then(o => o.ConvertToLoadingError(po, "", line: command.Line)))
      {
         _value.Val = command.Value;
         return true;
      }
      return false;
   }

   public override void Activate(ITarget target)
   {
      Debug.Assert(target is Province, $"'{EFFECT_NAME}' effect is only valid on provinces");
      if (Religion.TryParse(_value.Val, out var religion).Log())
         ((Province)target).ReformationCenter = religion;
   }

   public override string GetTokenName() => EFFECT_NAME;
   public override string GetTokenDescription() => EFFECT_DESCRIPTION;
   public override string GetTokenExample() => EFFECT_EXAMPLE;
}

public class CultureEffect : SimpleStringEffect
{
   public const string EFFECT_NAME = "culture";
   private const string EFFECT_DESCRIPTION = $"Sets the {EFFECT_NAME} of the province. ONLY valid in province history files.";
   private const string EFFECT_EXAMPLE = $"{EFFECT_NAME} = <culture>";
   private const ScopeType EFFECT_SCOPE = ScopeType.Province;

   public static IToken? CreateEffect(EnhancedBlock? block, LineKvp<string, string>? kvp, PCFL_Scope scope, PathObj po)
   {
      Debug.Assert(kvp is not null, "At this point the kvp must not be null. This must be filtered earlier in the pipeline");

      CultureEffect token = new();
      return token.Parse(kvp.Value, po) ? token : null;
   }

   public override bool Parse(LineKvp<string, string> command, PathObj po)
   {
      if (Culture.TryParse(command.Value, out _).Then(o => o.ConvertToLoadingError(po, "", line: command.Line)))
      {
         _value.Val = command.Value;
         return true;
      }
      return false;
   }

   public override void Activate(ITarget target)
   {
      Debug.Assert(target is Province, $"'{EFFECT_NAME}' effect is only valid on provinces");
      if (Culture.TryParse(_value.Val, out var culture).Log())
         ((Province)target).Culture = culture;
   }

   public override string GetTokenName() => EFFECT_NAME;
   public override string GetTokenDescription() => EFFECT_DESCRIPTION;
   public override string GetTokenExample() => EFFECT_EXAMPLE;
}

public class DiscoveredByEffect : SimpleStringEffect
{
   public const string EFFECT_NAME = "discovered_by";
   private const string EFFECT_DESCRIPTION = $"Grants the specified technology or tag group vision of this province. ONLY valid in province history.";
   private const string EFFECT_EXAMPLE = $"{EFFECT_NAME} = <tag | technology_group>";
   private const ScopeType EFFECT_SCOPE = ScopeType.Province;

   public static IToken? CreateEffect(EnhancedBlock? block, LineKvp<string, string>? kvp, PCFL_Scope scope, PathObj po)
   {
      Debug.Assert(kvp is not null, "At this point the kvp must not be null. This must be filtered earlier in the pipeline");

      DiscoveredByEffect token = new();
      return token.Parse(kvp.Value, po) ? token : null;
   }

   public override bool Parse(LineKvp<string, string> command, PathObj po)
   {
      if (Tag.TryParse(command.Value, out var tag))
      {
         _value.Val = tag;
         return true;
      }
      if (Globals.TechnologyGroups.Contains(command.Value))
      {
         _value.Val = command.Value;
         return true;
      }
      _ = new LoadingError(po, $"Invalid value for {EFFECT_NAME}: {command.Value}", line: command.Line, type: ErrorType.PCFL_TokenValidationError);
      return false;
   }

   public override void Activate(ITarget target)
   {
      Debug.Assert(target is Province, $"'{EFFECT_NAME}' effect is only valid on provinces");
      if (Tag.TryParse(_value.Val, out _) || Globals.TechnologyGroups.Contains(_value.Val))
         ((Province)target).DiscoveredBy.Add(_value.Val);
      else
         _ = new ErrorObject(type: ErrorType.PCFL_TokenValidationError, $"value {_value.Val} for {EFFECT_NAME} is not of type Tag or technology_group!");
   }

   public override string GetTokenName() => EFFECT_NAME;
   public override string GetTokenDescription() => EFFECT_DESCRIPTION;
   public override string GetTokenExample() => EFFECT_EXAMPLE;
}

public class IsCityEffect : SimpleBoolEffect
{
   public const string EFFECT_NAME = "is_city";
   private const string EFFECT_DESCRIPTION = $"Whether this province is a proper city, i.e. not a colony. ONLY valid in province history files.";
   private const string EFFECT_EXAMPLE = $"{EFFECT_NAME} = <bool>";
   private const ScopeType EFFECT_SCOPE = ScopeType.Province;

   public static IToken? CreateEffect(EnhancedBlock? block, LineKvp<string, string>? kvp, PCFL_Scope scope, PathObj po)
   {
      Debug.Assert(kvp is not null, "At this point the kvp must not be null. This must be filtered earlier in the pipeline");

      IsCityEffect token = new();
      return token.Parse(kvp.Value, po) ? token : null;
   }
   
   public override void Activate(ITarget target)
   {
      Debug.Assert(target is Province, $"'{EFFECT_NAME}' effect is only valid on provinces");
      ((Province)target).IsCity = _value.Val;
   }

   public override string GetTokenName() => EFFECT_NAME;
   public override string GetTokenDescription() => EFFECT_DESCRIPTION;
   public override string GetTokenExample() => EFFECT_EXAMPLE;
}

public class HREEffect : SimpleBoolEffect
{
   public const string EFFECT_NAME = "hre";
   private const string EFFECT_DESCRIPTION = $"Whether this province is in the HRE or not. ONLY valid in province history files.";
   private const string EFFECT_EXAMPLE = $"{EFFECT_NAME} = <bool>";
   private const ScopeType EFFECT_SCOPE = ScopeType.Province;

   public static IToken? CreateEffect(EnhancedBlock? block, LineKvp<string, string>? kvp, PCFL_Scope scope, PathObj po)
   {
      Debug.Assert(kvp is not null, "At this point the kvp must not be null. This must be filtered earlier in the pipeline");

      HREEffect token = new();
      return token.Parse(kvp.Value, po) ? token : null;
   }
   
   public override void Activate(ITarget target)
   {
      Debug.Assert(target is Province, $"'{EFFECT_NAME}' effect is only valid on provinces");
      ((Province)target).IsHre = _value.Val;
   }

   public override string GetTokenName() => EFFECT_NAME;
   public override string GetTokenDescription() => EFFECT_DESCRIPTION;
   public override string GetTokenExample() => EFFECT_EXAMPLE;
}

public class SeatInParliamentEffect : SimpleBoolEffect
{
   public const string EFFECT_NAME = "seat_in_parliament";
   private const string EFFECT_DESCRIPTION = $"Gives or removes the province's seat in the parliament. ONLY valid in province history files.";
   private const string EFFECT_EXAMPLE = $"{EFFECT_NAME} = <bool>";
   private const ScopeType EFFECT_SCOPE = ScopeType.Province;

   public static IToken? CreateEffect(EnhancedBlock? block, LineKvp<string, string>? kvp, PCFL_Scope scope, PathObj po)
   {
      Debug.Assert(kvp is not null, "At this point the kvp must not be null. This must be filtered earlier in the pipeline");

      SeatInParliamentEffect token = new();
      return token.Parse(kvp.Value, po) ? token : null;
   }
   
   public override void Activate(ITarget target)
   {
      Debug.Assert(target is Province, $"'{EFFECT_NAME}' effect is only valid on provinces");
      ((Province)target).IsSeatInParliament = _value.Val;
   }

   public override string GetTokenName() => EFFECT_NAME;
   public override string GetTokenDescription() => EFFECT_DESCRIPTION;
   public override string GetTokenExample() => EFFECT_EXAMPLE;
}

public class CapitalEffect : SimpleStringEffect
{
   public const string EFFECT_NAME = "capital";
   private const string EFFECT_DESCRIPTION = $"The capital name to use for the province, otherwise the province name is used as the capital name. ONLY valid in province history files.";
   private const string EFFECT_EXAMPLE = $"{EFFECT_NAME} = <string>";
   private const ScopeType EFFECT_SCOPE = ScopeType.Province;

   public static IToken? CreateEffect(EnhancedBlock? block, LineKvp<string, string>? kvp, PCFL_Scope scope, PathObj po)
   {
      Debug.Assert(kvp is not null, "At this point the kvp must not be null. This must be filtered earlier in the pipeline");

      CapitalEffect token = new();
      return token.Parse(kvp.Value, po) ? token : null;
   }
   
   public override void Activate(ITarget target)
   {
      Debug.Assert(target is Province, $"'{EFFECT_NAME}' effect is only valid on provinces");
      ((Province)target).Capital = _value.Val;
   }

   public override string GetTokenName() => EFFECT_NAME;
   public override string GetTokenDescription() => EFFECT_DESCRIPTION;
   public override string GetTokenExample() => EFFECT_EXAMPLE;
}

public class TradeGoodsEffect : SimpleStringEffect
{
   public const string EFFECT_NAME = "trade_goods";
   private const string EFFECT_DESCRIPTION = $"The tradegood assigned to this province, from 00_tradegoods.txt. ONLY valid in province history files.";
   private const string EFFECT_EXAMPLE = $"{EFFECT_NAME} = <trade_good>";
   private const ScopeType EFFECT_SCOPE = ScopeType.Province;

   public static IToken? CreateEffect(EnhancedBlock? block, LineKvp<string, string>? kvp, PCFL_Scope scope, PathObj po)
   {
      Debug.Assert(kvp is not null, "At this point the kvp must not be null. This must be filtered earlier in the pipeline");

      TradeGoodsEffect token = new();
      return token.Parse(kvp.Value, po) ? token : null;
   }

   public override bool Parse(LineKvp<string, string> command, PathObj po)
   {
      if (TradeGood.TryParse(command.Value, out _).Then(o => o.ConvertToLoadingError(po, " ", line: command.Line)))
      {
         _value.Val = command.Value;
         return true;
      }
      return false;
   }
   
   public override void Activate(ITarget target)
   {
      Debug.Assert(target is Province, $"'{EFFECT_NAME}' effect is only valid on provinces");
      if (TradeGood.TryParse(_value.Val, out var tradeGood).Log())
         ((Province)target).TradeGood = tradeGood;
   }

   public override string GetTokenName() => EFFECT_NAME;
   public override string GetTokenDescription() => EFFECT_DESCRIPTION;
   public override string GetTokenExample() => EFFECT_EXAMPLE;
}

public class TribalOwnerEffect : SimpleTagEffect
{
   public const string EFFECT_NAME = "tribal_owner";
   private const string EFFECT_DESCRIPTION = $"Sets the tribal owner of a province.";
   private const string EFFECT_EXAMPLE = $"{EFFECT_NAME} = <tag>";
   private const ScopeType EFFECT_SCOPE = ScopeType.Province;

   public static IToken? CreateEffect(EnhancedBlock? block, LineKvp<string, string>? kvp, PCFL_Scope scope, PathObj po)
   {
      Debug.Assert(kvp is not null, "At this point the kvp must not be null. This must be filtered earlier in the pipeline");

      TribalOwnerEffect token = new();
      return token.Parse(kvp.Value, po) ? token : null;
   }

   public override void Activate(ITarget target)
   {
      Debug.Assert(target is Province, $"'{EFFECT_NAME}' effect is only valid on provinces");
      if (Tag.TryParse(_value.Val, out var tag))
         ((Province)target).TribalOwner = tag;
      else
         _ = new ErrorObject(type: ErrorType.PCFL_TokenValidationError, $"value for {EFFECT_NAME} is not of type Tag!");
   }

   public override string GetTokenName() => EFFECT_NAME;
   public override string GetTokenDescription() => EFFECT_DESCRIPTION;
   public override string GetTokenExample() => EFFECT_EXAMPLE;
}

public class AddLocalAutonomyEffect : SimpleFloatEffect
{
   public const string EFFECT_NAME = "add_local_autonomy";
   private const string EFFECT_DESCRIPTION = $"Adds local autonomy to the current province scope.";
   private const string EFFECT_EXAMPLE = $"{EFFECT_NAME} = 0.1";
   private const ScopeType EFFECT_SCOPE = ScopeType.Province;

   public static IToken? CreateEffect(EnhancedBlock? block, LineKvp<string, string>? kvp, PCFL_Scope scope, PathObj po)
   {
      Debug.Assert(kvp is not null, "At this point the kvp must not be null. This must be filtered earlier in the pipeline");

      AddLocalAutonomyEffect token = new();
      return token.Parse(kvp.Value, po) ? token : null;
   }

   public override void Activate(ITarget target)
   {
      Debug.Assert(target is Province, $"'{EFFECT_NAME}' effect is only valid on provinces");
      ((Province)target).LocalAutonomy += _value.Val;
   }

   public override string GetTokenName() => EFFECT_NAME;
   public override string GetTokenDescription() => EFFECT_DESCRIPTION;
   public override string GetTokenExample() => EFFECT_EXAMPLE;
}

public class NativeSizeEffect : SimpleIntEffect
{
   public const string EFFECT_NAME = "native_size";
   private const string EFFECT_DESCRIPTION = $"Sets native_size of the current province scope. ONLY useable in province history!";
   private const string EFFECT_EXAMPLE = $"{EFFECT_NAME} = 50";
   private const ScopeType EFFECT_SCOPE = ScopeType.Province;

   public static IToken? CreateEffect(EnhancedBlock? block, LineKvp<string, string>? kvp, PCFL_Scope scope, PathObj po)
   {
      Debug.Assert(kvp is not null, "At this point the kvp must not be null. This must be filtered earlier in the pipeline");

      NativeSizeEffect token = new();
      return token.Parse(kvp.Value, po) ? token : null;
   }

   public override void Activate(ITarget target)
   {
      Debug.Assert(target is Province, $"'{EFFECT_NAME}' effect is only valid on provinces");
      ((Province)target).NativeSize = _value.Val;
   }

   public override string GetTokenName() => EFFECT_NAME;
   public override string GetTokenDescription() => EFFECT_DESCRIPTION;
   public override string GetTokenExample() => EFFECT_EXAMPLE;
}

public class AddClaimEffect : SimpleTagEffect
{
   public const string EFFECT_NAME = "add_claim";
   private const string EFFECT_DESCRIPTION = $"The defined scope gains a claim on the current province scope. ";
   private const string EFFECT_EXAMPLE = $"capital_scope = {{\n\tadd_claim = FRA\n}}";
   private const ScopeType EFFECT_SCOPE = ScopeType.Province;

   public static IToken? CreateEffect(EnhancedBlock? block, LineKvp<string, string>? kvp, PCFL_Scope scope, PathObj po)
   {
      Debug.Assert(kvp is not null, "At this point the kvp must not be null. This must be filtered earlier in the pipeline");

      AddClaimEffect token = new();
      return token.Parse(kvp.Value, po) ? token : null;
   }

   public override void Activate(ITarget target)
   {
      Debug.Assert(target is Province, $"'{EFFECT_NAME}' effect is only valid on provinces");
      if (Tag.TryParse(_value.Val, out var tag))
         ((Province)target).Claims.Add(tag);
      else
         _ = new ErrorObject(type: ErrorType.PCFL_TokenValidationError, $"value for {EFFECT_NAME} is not of type Tag!");
   }

   public override string GetTokenName() => EFFECT_NAME;
   public override string GetTokenDescription() => EFFECT_DESCRIPTION;
   public override string GetTokenExample() => EFFECT_EXAMPLE;
}

public class RemoveClaimEffect : SimpleTagEffect
{
   public const string EFFECT_NAME = "remove_claim";
   private const string EFFECT_DESCRIPTION = $"The defined scope gains a claim on the current province scope. ";
   private const string EFFECT_EXAMPLE = $"capital_scope = {{\n\tremove_claim = FRA\n}}";
   private const ScopeType EFFECT_SCOPE = ScopeType.Province;

   public static IToken? CreateEffect(EnhancedBlock? block, LineKvp<string, string>? kvp, PCFL_Scope scope, PathObj po)
   {
      Debug.Assert(kvp is not null, "At this point the kvp must not be null. This must be filtered earlier in the pipeline");

      RemoveClaimEffect token = new();
      return token.Parse(kvp.Value, po) ? token : null;
   }

   public override void Activate(ITarget target)
   {
      Debug.Assert(target is Province, $"'{EFFECT_NAME}' effect is only valid on provinces");
      if (Tag.TryParse(_value.Val, out var tag))
         ((Province)target).Claims.Remove(tag);
      else
         _ = new ErrorObject(type: ErrorType.PCFL_TokenValidationError, $"value for {EFFECT_NAME} is not of type Tag!");
   }

   public override string GetTokenName() => EFFECT_NAME;
   public override string GetTokenDescription() => EFFECT_DESCRIPTION;
   public override string GetTokenExample() => EFFECT_EXAMPLE;
}

public class AddPermanentProvinceModifierEffect : IToken
{
   public const string EFFECT_NAME = "add_permanent_province_modifier";
   private const string EFFECT_DESCRIPTION = $"Adds an event modifier to the current province scope as a province modifier. ";
   private const string EFFECT_EXAMPLE = $"add_permanent_province_modifier = {{\n\tname = annoyed_people\n\tduration = -1\n\tdesc = annoyed_people_tooltip\n\thidden = yes\n}}";
   private const ScopeType EFFECT_SCOPE = ScopeType.Province;

   internal Value<string> name= new(string.Empty);
   internal Value<string> desc = new(string.Empty);
   internal Value<int> duration = new(-1);
   internal Value<bool> hidden = new(false);

   public static IToken? CreateEffect(EnhancedBlock? block, LineKvp<string, string>? kvp, PCFL_Scope scope, PathObj po)
   {
      Debug.Assert(block is not null, "At this point the block must not be null. This must be filtered earlier in the pipeline");

      AddPermanentProvinceModifierEffect token = new();
      return token.Parse(block, po) ? token : null;
   }

   public bool Parse(EnhancedBlock block, PathObj po)
   {
      foreach (var element in block.GetContentElements(true, po))
      {
         foreach (var kvp in element.GetLineKvpEnumerator(po))
         {
            switch (kvp.Key)
            {
               case "name":
                  {
                     if (!GeneralFileParser.ParseSingleTriggerValue(ref name, kvp, po, EFFECT_NAME))
                        return false;
                     break;
                  }
               case "duration":
                  {
                     if (!GeneralFileParser.ParseSingleTriggerValue(ref duration, kvp, po, EFFECT_NAME))
                        return false;
                     break;
                  }
               case "hidden":
                  {
                     if (!GeneralFileParser.ParseSingleTriggerValue(ref hidden, kvp, po, EFFECT_NAME))
                        return false;
                     break;
                  }
               case "desc":
                  {
                     if (!GeneralFileParser.ParseSingleTriggerValue(ref name, kvp, po, EFFECT_NAME))
                        return false;
                     break;
                  }
               default:
                  _ = new LoadingError(po, $"Invalid key in {EFFECT_NAME}: {kvp.Key}", line: kvp.Line, type: ErrorType.PCFL_TokenValidationError);
                  return false;
            }
         }
      }
      return false;
   }

   public bool ParseReplace()
   {
      return false;
   }

   public void Activate(ITarget target)
   {
      Debug.Assert(target is Province, $"'{GetTokenName()}' effect is only valid on provinces");

      ((Province)target).PermanentProvinceModifiers.Add(new (name.Val, duration.Val)); // TODO proper modifier implementation
   }

   public void GetTokenString(int tabs, ref StringBuilder sb)
   {
      SavingUtil.OpenBlock(ref tabs, EFFECT_NAME, ref sb);
      SavingUtil.AddString(tabs, name.Val, "name", ref sb);
      SavingUtil.AddInt(tabs, duration.Val, "duration", ref sb);
      SavingUtil.AddBoolIfYes(tabs, hidden.Val, "hidden", ref sb);
      SavingUtil.AddString(tabs, desc.Val, "desc", ref sb);
      SavingUtil.CloseBlock(ref tabs, ref sb);
   }

   public string GetTokenName() => EFFECT_NAME;
   public string GetTokenDescription() => EFFECT_DESCRIPTION;
   public string GetTokenExample() => EFFECT_EXAMPLE;
}

public class AddTradeCompanyInvestmentModifierEffect : IToken
{
   public const string EFFECT_NAME = "add_trade_company_investment";
   private const string EFFECT_DESCRIPTION = $"Adds a trade company investment in the provinces area.";
   private const string EFFECT_EXAMPLE = $"add_trade_company_investment = {{\n\t\tinvestment = local_quarter\n\t\tinvestor = NED\n\t}}";
   private const ScopeType EFFECT_SCOPE = ScopeType.Province;

   internal Value<string> investment = new(string.Empty);
   internal Value<string> investor = new(string.Empty);

   public static IToken? CreateEffect(EnhancedBlock? block, LineKvp<string, string>? kvp, PCFL_Scope scope, PathObj po)
   {
      Debug.Assert(block is not null, "At this point the block must not be null. This must be filtered earlier in the pipeline");

      AddTradeCompanyInvestmentModifierEffect token = new();
      return token.Parse(block, po) ? token : null;
   }

   public bool Parse(EnhancedBlock block, PathObj po)
   {
      foreach (var element in block.GetContentElements(true, po))
      {
         foreach (var kvp in element.GetLineKvpEnumerator(po))
         {
            switch (kvp.Key)
            {
               case "investment":
                  if (!GeneralFileParser.ParseSingleTriggerValue(ref investment, kvp, po, EFFECT_NAME))
                     return false;
                  break;
               case "investor":
                  if (!GeneralFileParser.ParseSingleTriggerValue(ref investor, kvp, po, EFFECT_NAME))
                     return false;
                  break;
               default:
                  _ = new LoadingError(po, $"Invalid key in {EFFECT_NAME}: {kvp.Key}", line: kvp.Line, type: ErrorType.PCFL_TokenValidationError);
                  return false;
            }
         }
      }
      return false;
   }

   public bool ParseReplace()
   {
      return false;
   }

   public void Activate(ITarget target)
   {
      Debug.Assert(target is Province, $"'{GetTokenName()}' effect is only valid on provinces");

      ((Province)target).TradeCompanyInvestments.Add(investment.Val); // TODO proper modifier implementation
   }

   public void GetTokenString(int tabs, ref StringBuilder sb)
   {
      SavingUtil.OpenBlock(ref tabs, EFFECT_NAME, ref sb);
      SavingUtil.AddString(tabs, investment.Val, "name", ref sb);
      SavingUtil.AddString(tabs, investment.Val, "desc", ref sb);
      SavingUtil.CloseBlock(ref tabs, ref sb);
   }

   public string GetTokenName() => EFFECT_NAME;
   public string GetTokenDescription() => EFFECT_DESCRIPTION;
   public string GetTokenExample() => EFFECT_EXAMPLE;
}