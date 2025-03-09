
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using Editor.DataClasses.GameDataClasses;
using Editor.DataClasses.Saveables;
using Editor.ErrorHandling;
using Editor.Saving;

namespace Editor.Loading.Enhanced.PCFL.Implementation.ProvinceScope;

public class AddBaseManpowerEffect() : SimpleEffect<int>(1)
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

public class AddBaseTaxEffect() : SimpleEffect<int>(1)
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

public class AddBaseProductionEffect() : SimpleEffect<int>(1)
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

public class BaseManpowerEffect() : SimpleEffect<int>(1)
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

public class BaseTaxEffect() : SimpleEffect<int>(1)
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

public class BaseProductionEffect() : SimpleEffect<int>(1)
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

public class OwnerEffect() : SimpleEffect<Country>(Country.Empty)
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
      Debug.Assert(Globals.Countries.TryGetValue(_value.Val.Tag, out _), "The country must always exist to use it in execution");
      ((Province)target).Owner = _value.Val;
   }

   public override string GetTokenName() => EFFECT_NAME;
   public override string GetTokenDescription() => EFFECT_DESCRIPTION;
   public override string GetTokenExample() => EFFECT_EXAMPLE;
}

public class ControllerEffect() : SimpleEffect<Country>(Country.Empty)
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
      Debug.Assert(Globals.Countries.TryGetValue(_value.Val.Tag, out _), "The country must always exist to use it in execution");
      ((Province)target).Controller = _value.Val;
   }

   public override string GetTokenName() => EFFECT_NAME;
   public override string GetTokenDescription() => EFFECT_DESCRIPTION;
   public override string GetTokenExample() => EFFECT_EXAMPLE;
}

public class AddCoreEffect() : SimpleEffect<Country>(Country.Empty)
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
      Debug.Assert(Globals.Countries.TryGetValue(_value.Val.Tag, out _), "The country must always exist to use it in execution");
      ((Province)target).Cores.Add(_value.Val.Tag);
   }

   public override string GetTokenName() => EFFECT_NAME;
   public override string GetTokenDescription() => EFFECT_DESCRIPTION;
   public override string GetTokenExample() => EFFECT_EXAMPLE;
}

public class RemoveCoreEffect() : SimpleEffect<Country>(Country.Empty)
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
      Debug.Assert(Globals.Countries.TryGetValue(_value.Val.Tag, out _), "The country must always exist to use it in execution");
      ((Province)target).Cores.Remove(_value.Val.Tag);
   }

   public override string GetTokenName() => EFFECT_NAME;
   public override string GetTokenDescription() => EFFECT_DESCRIPTION;
   public override string GetTokenExample() => EFFECT_EXAMPLE;
}

public class UnrestEffect() : SimpleEffect<int>(1)
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

public class RevoltRiskEffect() : SimpleEffect<int>(1)
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
                  if (!GeneralFileParser.ParseSingleTriggerVal(ref type, kvp, po))
                     return false;
                  break;
               }
               case "size":
               {
                  if (!GeneralFileParser.ParseSingleTriggerVal(ref size, kvp, po))
                     return false;
                  break;
               }
               case "leader":
               {
                  if (!GeneralFileParser.ParseSingleTriggerVal(ref leader, kvp, po))
                     return false;
                  break;
               }
               case "name":
               {
                  if (!GeneralFileParser.ParseSingleTriggerVal(ref name, kvp, po))
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

public class ReligionEffect() : SimpleEffect<Religion>(Religion.Empty)
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

   public override void Activate(ITarget target)
   {
      Debug.Assert(target is Province, $"'{EFFECT_NAME}' effect is only valid on provinces");
      Debug.Assert(Religion.TryParse(_value.Val.Name, out _).Ignore(), "The religion must always exist to use it in execution");
      ((Province)target).Religion = _value.Val;
   }

   public override string GetTokenName() => EFFECT_NAME;
   public override string GetTokenDescription() => EFFECT_DESCRIPTION;
   public override string GetTokenExample() => EFFECT_EXAMPLE;
}

public class ReformationCenterEffect() : SimpleEffect<Religion>(Religion.Empty)
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
   
   public override void Activate(ITarget target)
   {
      Debug.Assert(target is Province, $"'{EFFECT_NAME}' effect is only valid on provinces");
      Debug.Assert(Religion.TryParse(_value.Val.Name, out _).Ignore(), "The religion must always exist to use it in execution");
      ((Province)target).ReformationCenter = _value.Val;
   }

   public override string GetTokenName() => EFFECT_NAME;
   public override string GetTokenDescription() => EFFECT_DESCRIPTION;
   public override string GetTokenExample() => EFFECT_EXAMPLE;
}

public class CultureEffect() : SimpleEffect<Culture>(Culture.Empty)
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

   public override void Activate(ITarget target)
   {
      Debug.Assert(target is Province, $"'{EFFECT_NAME}' effect is only valid on provinces");
      Debug.Assert(Culture.TryParse(_value.Val.Name, out _).Ignore(), "The culture must always exist to use it in execution");  
      
      ((Province)target).Culture = _value.Val;
   }

   public override string GetTokenName() => EFFECT_NAME;
   public override string GetTokenDescription() => EFFECT_DESCRIPTION;
   public override string GetTokenExample() => EFFECT_EXAMPLE;
}

public class DiscoveredByEffect() : SimpleEffect<string>(string.Empty)
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
      Debug.Assert(Globals.Countries.Contains(_value.Val) || Globals.TechnologyGroups.Contains(_value.Val), "The country or tech group must always exist to use it in execution");
      ((Province)target).DiscoveredBy.Add(_value.Val);
   }

   public override string GetTokenName() => EFFECT_NAME;
   public override string GetTokenDescription() => EFFECT_DESCRIPTION;
   public override string GetTokenExample() => EFFECT_EXAMPLE;
}

public class IsCityEffect() : SimpleEffect<bool>(false)
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

public class HREEffect() : SimpleEffect<bool>(false)
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

public class SeatInParliamentEffect() : SimpleEffect<bool>(false)
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

public class CapitalEffect() : SimpleEffect<string>(string.Empty)
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

public class TradeGoodsEffect() : SimpleEffect<TradeGood>(TradeGood.Empty)
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
   
   public override void Activate(ITarget target)
   {
      Debug.Assert(target is Province, $"'{EFFECT_NAME}' effect is only valid on provinces");
      Debug.Assert(TradeGood.TryParse(_value.Val.Name, out _).Ignore(), "The trade good must always exist to use it in execution");
      ((Province)target).TradeGood = _value.Val;
   }

   public override string GetTokenName() => EFFECT_NAME;
   public override string GetTokenDescription() => EFFECT_DESCRIPTION;
   public override string GetTokenExample() => EFFECT_EXAMPLE;
}

public class TribalOwnerEffect() : SimpleEffect<Country>(Country.Empty)
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
      Debug.Assert(Globals.Countries.TryGetValue(_value.Val.Tag, out _), "The country must always exist to use it in execution");
      ((Province)target).TribalOwner = _value.Val;
   }

   public override string GetTokenName() => EFFECT_NAME;
   public override string GetTokenDescription() => EFFECT_DESCRIPTION;
   public override string GetTokenExample() => EFFECT_EXAMPLE;
}

public class AddLocalAutonomyEffect() : SimpleEffect<float>(0f)
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

public class NativeSizeEffect() : SimpleEffect<int>(0)
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

public class AddClaimEffect() : SimpleEffect<Country>(Country.Empty)
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
      Debug.Assert(Country.TryParse(_value.Val.Tag, out _).Ignore(), "The country must always exist to use it in execution");
      ((Province)target).Claims.Add(_value.Val.Tag);
   }

   public override string GetTokenName() => EFFECT_NAME;
   public override string GetTokenDescription() => EFFECT_DESCRIPTION;
   public override string GetTokenExample() => EFFECT_EXAMPLE;
}

public class RemoveClaimEffect() : SimpleEffect<Country>(Country.Empty)
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
      Debug.Assert(Country.TryParse(_value.Val.Tag, out _).Ignore(), "The country must always exist to use it in execution");
      ((Province)target).Claims.Remove(_value.Val.Tag);
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
                     if (!GeneralFileParser.ParseSingleTriggerValue(ref duration, kvp, po))
                        return false;
                     break;
                  }
               case "hidden":
                  {
                     if (!GeneralFileParser.ParseSingleTriggerValue(ref hidden, kvp, po))
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

public class NativeFerocityEffect() : SimpleEffect<int>(0)
{
   public const string EFFECT_NAME = "native_ferocity";
   private const string EFFECT_DESCRIPTION = $"Adds to the Native Ferocity within an uncolonized province. ";
   private const string EFFECT_EXAMPLE = $"native_ferocity = 5";
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

      ((Province)target).NativeFerocity = _value.Val;
   }

   public override string GetTokenName() => EFFECT_NAME;
   public override string GetTokenDescription() => EFFECT_DESCRIPTION;
   public override string GetTokenExample() => EFFECT_EXAMPLE;
}
public class NativeHostilnessEffect() : SimpleEffect<int>(0)
{
   public const string EFFECT_NAME = "native_hostileness ";
   private const string EFFECT_DESCRIPTION = $"Adds to the Native Hostileness within an uncolonized province. ";
   private const string EFFECT_EXAMPLE = $"native_hostileness = 5";
   private const ScopeType EFFECT_SCOPE = ScopeType.Province;

   public static IToken? CreateEffect(EnhancedBlock? block, LineKvp<string, string>? kvp, PCFL_Scope scope, PathObj po)
   {
      Debug.Assert(kvp is not null, "At this point the kvp must not be null. This must be filtered earlier in the pipeline");

      NativeHostilnessEffect token = new();
      return token.Parse(kvp.Value, po) ? token : null;
   }

   public override void Activate(ITarget target)
   {
      Debug.Assert(target is Province, $"'{EFFECT_NAME}' effect is only valid on provinces");
      ((Province)target).NativeHostileness = _value.Val;
   }

   public override string GetTokenName() => EFFECT_NAME;
   public override string GetTokenDescription() => EFFECT_DESCRIPTION;
   public override string GetTokenExample() => EFFECT_EXAMPLE;
}
public class AddProvinceTriggeredModifierEffect() : SimpleEffect<string>(string.Empty)
{
   public const string EFFECT_NAME = "add_province_triggered_modifier ";
   private const string EFFECT_DESCRIPTION = $"Adds a province triggered modifier to the current province scope. ";
   private const string EFFECT_EXAMPLE = $"add_province_triggered_modifier = test_modifier";
   private const ScopeType EFFECT_SCOPE = ScopeType.Province;

   public static IToken? CreateEffect(EnhancedBlock? block, LineKvp<string, string>? kvp, PCFL_Scope scope, PathObj po)
   {
      Debug.Assert(kvp is not null, "At this point the kvp must not be null. This must be filtered earlier in the pipeline");

      AddProvinceTriggeredModifierEffect token = new();
      return token.Parse(kvp.Value, po) ? token : null;
   }

   public override void Activate(ITarget target)
   {
      Debug.Assert(target is Province, $"'{EFFECT_NAME}' effect is only valid on provinces");
      ((Province)target).ProvinceTriggeredModifiers.Add(_value.Val);
   }

   public override string GetTokenName() => EFFECT_NAME;
   public override string GetTokenDescription() => EFFECT_DESCRIPTION;
   public override string GetTokenExample() => EFFECT_EXAMPLE;
}
public class RemoveProvinceTriggeredModifierEffect() : SimpleEffect<string>(string.Empty)
{
   public const string EFFECT_NAME = "remove_province_triggered_modifier ";
   private const string EFFECT_DESCRIPTION = $"Removes a province triggered modifier to the current province scope. ";
   private const string EFFECT_EXAMPLE = $"remove_province_triggered_modifier = test_modifier";
   private const ScopeType EFFECT_SCOPE = ScopeType.Province;

   public static IToken? CreateEffect(EnhancedBlock? block, LineKvp<string, string>? kvp, PCFL_Scope scope, PathObj po)
   {
      Debug.Assert(kvp is not null, "At this point the kvp must not be null. This must be filtered earlier in the pipeline");

      RemoveProvinceTriggeredModifierEffect token = new();
      return token.Parse(kvp.Value, po) ? token : null;
   }

   public override void Activate(ITarget target)
   {
      Debug.Assert(target is Province, $"'{EFFECT_NAME}' effect is only valid on provinces");
      ((Province)target).ProvinceTriggeredModifiers.Remove(_value.Val);
   }

   public override string GetTokenName() => EFFECT_NAME;
   public override string GetTokenDescription() => EFFECT_DESCRIPTION;
   public override string GetTokenExample() => EFFECT_EXAMPLE;
}

public class AddToTradeCompanyEffect() : SimpleEffect<Country>(Country.Empty)
{
   public const string EFFECT_NAME = "add_to_trade_company";
   private const string EFFECT_DESCRIPTION = $"The defined scope gains a claim on the current province scope. ";
   private const string EFFECT_EXAMPLE = $"add_to_trade_company = FRA";
   private const ScopeType EFFECT_SCOPE = ScopeType.Province;

   public static IToken? CreateEffect(EnhancedBlock? block, LineKvp<string, string>? kvp, PCFL_Scope scope, PathObj po)
   {
      Debug.Assert(kvp is not null, "At this point the kvp must not be null. This must be filtered earlier in the pipeline");

      AddToTradeCompanyEffect token = new();
      return token.Parse(kvp.Value, po) ? token : null;
   }

   public override void Activate(ITarget target)
   {
      Debug.Assert(target is Province, $"'{EFFECT_NAME}' effect is only valid on provinces");
      Debug.Assert(Country.TryParse(_value.Val.Tag, out _).Ignore(), "The country must always exist to use it in execution");
      // TODO
   }

   public override string GetTokenName() => EFFECT_NAME;
   public override string GetTokenDescription() => EFFECT_DESCRIPTION;
   public override string GetTokenExample() => EFFECT_EXAMPLE;
}