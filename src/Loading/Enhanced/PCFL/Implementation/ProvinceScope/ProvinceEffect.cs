
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using Editor.DataClasses.GameDataClasses;
using Editor.DataClasses.Saveables;
using Editor.ErrorHandling;
using Editor.Helper;
using Editor.Saving;

namespace Editor.Loading.Enhanced.PCFL.Implementation.ProvinceScope;

public class AddBaseManpowerEffect() : SimpleEffect<int>(1)
{
   public static readonly string EffectName = "add_base_manpower";
   public static readonly string EffectDescription = "Adds base manpower to the current province scope.";
   public static readonly string EffectExample = "add_base_manpower = 3";
   
   
   public static IToken? CreateEffect(EnhancedBlock? block, LineKvp<string, string>? kvp, ParsingContext context, PathObj po)
   {
      Debug.Assert(kvp is not null, "At this point the kvp must not be null. This must be filtered earlier in the pipeline");

      AddBaseManpowerEffect token = new();
      return token.Parse(kvp.Value, po, context) ? token : null;
   }

   public override void Activate(ITarget target)
   {
      Debug.Assert(target is Province, $"'{EffectName}' effect is only valid on provinces");
      ((Province)target).BaseManpower += _value.Val;
   }

   public override string GetTokenName() => EffectName;
   public override string GetTokenDescription() => EffectDescription;
   public override string GetTokenExample() => EffectExample;
}

public class AddBaseTaxEffect() : SimpleEffect<int>(1)
{
   public static readonly string EffectName = "add_base_tax";
   public static readonly string EffectDescription = "Adds base tax to the current province scope.";
   public static readonly string EffectExample = "add_base_tax = 3";
   

   public static IToken? CreateEffect(EnhancedBlock? block, LineKvp<string, string>? kvp, ParsingContext context, PathObj po)
   {
      Debug.Assert(kvp is not null, "At this point the kvp must not be null. This must be filtered earlier in the pipeline");

      AddBaseTaxEffect token = new();
      return token.Parse(kvp.Value, po, context) ? token : null;
   }

   public override void Activate(ITarget target)
   {
      Debug.Assert(target is Province, $"'{EffectName}' effect is only valid on provinces");
      ((Province)target).BaseTax += _value.Val;
   }

   public override string GetTokenName() => EffectName;
   public override string GetTokenDescription() => EffectDescription;
   public override string GetTokenExample() => EffectExample;
}

public class AddBaseProductionEffect() : SimpleEffect<int>(1)
{
   public static readonly string EffectName = "add_base_production";
   public static readonly string EffectDescription = "Adds base production to the current province scope.";
   public static readonly string EffectExample = "add_base_production = 3";
   

   public static IToken? CreateEffect(EnhancedBlock? block, LineKvp<string, string>? kvp, ParsingContext context, PathObj po)
   {
      Debug.Assert(kvp is not null, "At this point the kvp must not be null. This must be filtered earlier in the pipeline");

      AddBaseProductionEffect token = new();
      return token.Parse(kvp.Value, po, context) ? token : null;
   }

   public override void Activate(ITarget target)
   {
      Debug.Assert(target is Province, $"'{EffectName}' effect is only valid on provinces");
      ((Province)target).BaseProduction += _value.Val;
   }

   public override string GetTokenName() => EffectName;
   public override string GetTokenDescription() => EffectDescription;
   public override string GetTokenExample() => EffectExample;
}

public class BaseManpowerEffect() : SimpleEffect<int>(1)
{
   public static readonly string EffectName = "base_manpower";
   public static readonly string EffectDescription = "Sets the base_manpower to the current province scope. ONLY in province histrory files.";
   public static readonly string EffectExample = "base_manpower = 3";
   
   public static IToken? CreateEffect(EnhancedBlock? block, LineKvp<string, string>? kvp, ParsingContext context, PathObj po)
   {
      Debug.Assert(kvp is not null, "At this point the kvp must not be null. This must be filtered earlier in the pipeline");

      BaseManpowerEffect token = new();
      return token.Parse(kvp.Value, po, context) ? token : null;
   }

   public override void Activate(ITarget target)
   {
      Debug.Assert(target is Province, $"'{EffectName}' effect is only valid on provinces");
      ((Province)target).BaseManpower = _value.Val;
   }

   public override string GetTokenName() => EffectName;
   public override string GetTokenDescription() => EffectDescription;
   public override string GetTokenExample() => EffectExample;
}

public class BaseTaxEffect() : SimpleEffect<int>(1)
{
   public static readonly string EffectName = "base_tax";
   public static readonly string EffectDescription = "Sets base tax to the current province scope. ONLY in province histrory files.";
   public static readonly string EffectExample = "base_tax = 3";
   

   public static IToken? CreateEffect(EnhancedBlock? block, LineKvp<string, string>? kvp, ParsingContext context, PathObj po)
   {
      Debug.Assert(kvp is not null, "At this point the kvp must not be null. This must be filtered earlier in the pipeline");

      BaseTaxEffect token = new();
      return token.Parse(kvp.Value, po, context) ? token : null;
   }

   public override void Activate(ITarget target)
   {
      Debug.Assert(target is Province, $"'{EffectName}' effect is only valid on provinces");
      ((Province)target).BaseTax = _value.Val;
   }

   public override string GetTokenName() => EffectName;
   public override string GetTokenDescription() => EffectDescription;
   public override string GetTokenExample() => EffectExample;
}

public class BaseProductionEffect() : SimpleEffect<int>(1)
{
   public static readonly string EffectName = "base_production";
   public static readonly string EffectDescription = "Sets base production to the current province scope. ONLY in province histrory files.";
   public static readonly string EffectExample = "base_production = 3";
   

   public static IToken? CreateEffect(EnhancedBlock? block, LineKvp<string, string>? kvp, ParsingContext context, PathObj po)
   {
      Debug.Assert(kvp is not null, "At this point the kvp must not be null. This must be filtered earlier in the pipeline");

      BaseProductionEffect token = new();
      return token.Parse(kvp.Value, po, context) ? token : null;
   }

   public override void Activate(ITarget target)
   {
      Debug.Assert(target is Province, $"'{EffectName}' effect is only valid on provinces");
      ((Province)target).BaseProduction = _value.Val;
   }

   public override string GetTokenName() => EffectName;
   public override string GetTokenDescription() => EffectDescription;
   public override string GetTokenExample() => EffectExample;
}

public class OwnerEffect() : SimpleEffect<Country>(Country.Empty)
{
   public static readonly string EffectName = "owner";
   public static readonly string EffectDescription = "Sets the owner of the province. ONLY valid in province history files";
   public static readonly string EffectExample = $"{EffectName} = TAG";
   
   
   public static IToken? CreateEffect(EnhancedBlock? block, LineKvp<string, string>? kvp, ParsingContext context, PathObj po)
   {
      Debug.Assert(kvp is not null, "At this point the kvp must not be null. This must be filtered earlier in the pipeline");
      
      OwnerEffect token = new();
      return token.Parse(kvp.Value, po, context) ? token : null;
   }

   public override void Activate(ITarget target)
   {
      Debug.Assert(target is Province, $"'{EffectName}' effect is only valid on provinces");
      Debug.Assert(Globals.Countries.TryGetValue(_value.Val.Tag, out _), "The country must always exist to use it in execution");
      ((Province)target).Owner = _value.Val;
   }

   public override string GetTokenName() => EffectName;
   public override string GetTokenDescription() => EffectDescription;
   public override string GetTokenExample() => EffectExample;
}

public class ControllerEffect() : SimpleEffect<Country>(Country.Empty)
{
   public static readonly string EffectName = "controller";
   public static readonly string EffectDescription = $"Sets the {EffectName} of the province. ONLY valid in province history files";
   public static readonly string EffectExample = $"{EffectName} = TAG";
   
   
   public static IToken? CreateEffect(EnhancedBlock? block, LineKvp<string, string>? kvp, ParsingContext context, PathObj po)
   {
      Debug.Assert(kvp is not null, "At this point the kvp must not be null. This must be filtered earlier in the pipeline");

      ControllerEffect token = new();
      return token.Parse(kvp.Value, po, context) ? token : null;
   }

   public override void Activate(ITarget target)
   {
      Debug.Assert(target is Province, $"'{EffectName}' effect is only valid on provinces");
      Debug.Assert(Globals.Countries.TryGetValue(_value.Val.Tag, out _), "The country must always exist to use it in execution");
      ((Province)target).Controller = _value.Val;
   }

   public override string GetTokenName() => EffectName;
   public override string GetTokenDescription() => EffectDescription;
   public override string GetTokenExample() => EffectExample;
}

public class AddCoreEffect() : SimpleEffect<Country>(Country.Empty)
{
   public static readonly string EffectName = "add_core";
   public static readonly string EffectDescription = $"<scope>The country that gains the core.";
   public static readonly string EffectExample = $"{EffectName} = TAG";
   
   
   public static IToken? CreateEffect(EnhancedBlock? block, LineKvp<string, string>? kvp, ParsingContext context, PathObj po)
   {
      Debug.Assert(kvp is not null, "At this point the kvp must not be null. This must be filtered earlier in the pipeline");

      AddCoreEffect token = new();
      return token.Parse(kvp.Value, po, context) ? token : null;
   }

   public override void Activate(ITarget target)
   {
      Debug.Assert(target is Province, $"'{EffectName}' effect is only valid on provinces");
      Debug.Assert(Globals.Countries.TryGetValue(_value.Val.Tag, out _), "The country must always exist to use it in execution");
      ((Province)target).Cores = [.. ((Province)target).Cores, _value.Val.Tag];
   }

   public override string GetTokenName() => EffectName;
   public override string GetTokenDescription() => EffectDescription;
   public override string GetTokenExample() => EffectExample;
}

public class RemoveCoreEffect() : SimpleEffect<Country>(Country.Empty)
{
   public static readonly string EffectName = "remove_core";
   public static readonly string EffectDescription = $"<scope>The country that loses their core.";
   public static readonly string EffectExample = $"{EffectName} = TAG";
   
   
   public static IToken? CreateEffect(EnhancedBlock? block, LineKvp<string, string>? kvp, ParsingContext context, PathObj po)
   {
      Debug.Assert(kvp is not null, "At this point the kvp must not be null. This must be filtered earlier in the pipeline");

      RemoveCoreEffect token = new();
      return token.Parse(kvp.Value, po, context) ? token : null;
   }

   public override void Activate(ITarget target)
   {
      Debug.Assert(target is Province, $"'{EffectName}' effect is only valid on provinces");
      Debug.Assert(Globals.Countries.TryGetValue(_value.Val.Tag, out _), "The country must always exist to use it in execution");
      List<Tag> coresTemp = new(((Province)target).Cores);
      coresTemp.Remove(_value.Val.Tag);
      ((Province)target).Cores = coresTemp;
   }

   public override string GetTokenName() => EffectName;
   public override string GetTokenDescription() => EffectDescription;
   public override string GetTokenExample() => EffectExample;
}

public class UnrestEffect() : SimpleEffect<int>(1)
{
   public static readonly string EffectName = "unrest";
   public static readonly string EffectDescription = $"Sets the {EffectName} of the province. ONLY valid in province history files";
   public static readonly string EffectExample = $"{EffectName} = 2";
   

   public static IToken? CreateEffect(EnhancedBlock? block, LineKvp<string, string>? kvp, ParsingContext context, PathObj po)
   {
      Debug.Assert(kvp is not null, "At this point the kvp must not be null. This must be filtered earlier in the pipeline");

      UnrestEffect token = new();
      return token.Parse(kvp.Value, po, context) ? token : null;
   }

   public override void Activate(ITarget target)
   {
      Debug.Assert(target is Province, $"'{EffectName}' effect is only valid on provinces");
      ((Province)target).RevoltRisk = _value.Val;
   }

   public override string GetTokenName() => EffectName;
   public override string GetTokenDescription() => EffectDescription;
   public override string GetTokenExample() => EffectExample;
}

public class RevoltRiskEffect() : SimpleEffect<int>(1)
{
   public static readonly string EffectName = "revolt_risk";
   public static readonly string EffectDescription = $"Sets the {EffectName} of the province. ONLY valid in province history files. DEPRECATED";
   public static readonly string EffectExample = $"{EffectName} = 1";
   

   public static IToken? CreateEffect(EnhancedBlock? block, LineKvp<string, string>? kvp, ParsingContext context, PathObj po)
   {
      Debug.Assert(kvp is not null, "At this point the kvp must not be null. This must be filtered earlier in the pipeline");

      _ = new LoadingError(po, $"\"{EffectName}\" is deprecated. Please use \"unrest\" instead.", line:kvp.Value.Line, type:ErrorType.PCFL_Deprecated, level: LogType.Warning);

      UnrestEffect token = new();
      return token.Parse(kvp.Value, po, context) ? token : null;
   }

   public override void Activate(ITarget target)
   {
      Debug.Assert(target is Province, $"'{EffectName}' effect is only valid on provinces");
      ((Province)target).RevoltRisk = _value.Val;
   }

   public override string GetTokenName() => EffectName;
   public override string GetTokenDescription() => EffectDescription;
   public override string GetTokenExample() => EffectExample;
}

public class RevoltEffect : IToken
{
   public static readonly string EffectName = "revolt";
   public static readonly string EffectDescription = $"Adds a revolt";
   public static readonly string EffectExample = $"revolt = {{\n\ttype = <rebel type>\n\tsize = <int>\n\tleader = <string>\n}}";
   

   internal Value<string> Type = new(string.Empty);
   internal Value<float> Size = new(-1);
   internal Value<string> Leader = new(string.Empty);
   internal Value<string> Name = new(string.Empty);

   public static IToken? CreateEffect(EnhancedBlock? block, LineKvp<string, string>? kvp, ParsingContext context, PathObj po)
   {
      Debug.Assert(block is not null, "At this point the block must not be null. This must be filtered earlier in the pipeline");

      RevoltEffect token = new();
      return token.Parse(block, po, context) ? token : null;
   }

   public bool Parse(EnhancedBlock block, PathObj po, ParsingContext context)
   {
      foreach (var element in block.GetContentElements(true, po))
      {
         foreach (var kvp in element.GetLineKvpEnumerator(po))
         {
            switch (kvp.Key)
            {
               case "type":
               {
                  if (!GeneralFileParser.ParseSingleTriggerVal(ref Type, kvp, po, context))
                     return false;
                  break;
               }
               case "size":
               {
                  if (!GeneralFileParser.ParseSingleTriggerVal(ref Size, kvp, po, context))
                     return false;
                  break;
               }
               case "leader":
               {
                  if (!GeneralFileParser.ParseSingleTriggerVal(ref Leader, kvp, po, context))
                     return false;
                  break;
               }
               case "name":
               {
                  if (!GeneralFileParser.ParseSingleTriggerVal(ref Name, kvp, po, context))
                     return false;
                  break;
               }
               default:
                  _ = new LoadingError(po, $"Invalid key in {EffectName}: {kvp.Key}", line: kvp.Line, type: ErrorType.PCFL_TokenValidationError);
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
      SavingUtil.OpenBlock(ref tabs, EffectName, ref sb);
      SavingUtil.AddString(tabs, Type.Val, "type", ref sb);
      SavingUtil.AddFloatIfNotValue(tabs, Size.Val, "size", -1, ref sb);
      SavingUtil.AddQuotedString(tabs, Leader.Val, "leader", ref sb);
      SavingUtil.AddQuotedString(tabs, Name.Val, "name", ref sb);
      SavingUtil.CloseBlock(ref tabs, ref sb);
   }

   public string GetTokenName() => EffectName;
   public string GetTokenDescription() => EffectDescription;
   public string GetTokenExample() => EffectExample;

   public override bool Equals(object? obj)
   {
      if (obj is null)
         return false;
      if (ReferenceEquals(this, obj))
         return true;

      if (obj is not RevoltEffect other)
         return false;

      return Type.Equals(other.Type) && Size.Equals(other.Size) && Leader.Equals(other.Leader) && Name.Equals(other.Name);
   }

   public override int GetHashCode()
   {
      return Type.GetHashCode() ^ Size.GetHashCode() ^ Leader.GetHashCode() ^ Name.GetHashCode();
   }
}

public class ReligionEffect() : SimpleEffect<Religion>(Religion.Empty)
{
   public static readonly string EffectName = "religion";
   public static readonly string EffectDescription = $"Sets the {EffectName} of the province. ONLY valid in province history files.";
   public static readonly string EffectExample = $"{EffectName} = <religion>";
   

   public static IToken? CreateEffect(EnhancedBlock? block, LineKvp<string, string>? kvp, ParsingContext context, PathObj po)
   {
      Debug.Assert(kvp is not null, "At this point the kvp must not be null. This must be filtered earlier in the pipeline");

      ReligionEffect token = new();
      return token.Parse(kvp.Value, po, context) ? token : null;
   }

   public override void Activate(ITarget target)
   {
      Debug.Assert(target is Province, $"'{EffectName}' effect is only valid on provinces");
      Debug.Assert(Religion.TryParse(_value.Val.Name, out _).Ignore(), "The religion must always exist to use it in execution");
      ((Province)target).Religion = _value.Val;
   }

   public override string GetTokenName() => EffectName;
   public override string GetTokenDescription() => EffectDescription;
   public override string GetTokenExample() => EffectExample;
}

public class ReformationCenterEffect() : SimpleEffect<Religion>(Religion.Empty)
{
   public static readonly string EffectName = "reformation_center";
   public static readonly string EffectDescription = $"Sets the {EffectName} of the province. ONLY valid in province history files.";
   public static readonly string EffectExample = $"{EffectName} = <religion>";
   

   public static IToken? CreateEffect(EnhancedBlock? block, LineKvp<string, string>? kvp, ParsingContext context, PathObj po)
   {
      Debug.Assert(kvp is not null, "At this point the kvp must not be null. This must be filtered earlier in the pipeline");
      ReligionEffect token = new();
      return token.Parse(kvp.Value, po, context) ? token : null;
   }
   
   public override void Activate(ITarget target)
   {
      Debug.Assert(target is Province, $"'{EffectName}' effect is only valid on provinces");
      Debug.Assert(Religion.TryParse(_value.Val.Name, out _).Ignore(), "The religion must always exist to use it in execution");
      ((Province)target).ReformationCenter = _value.Val;
   }

   public override string GetTokenName() => EffectName;
   public override string GetTokenDescription() => EffectDescription;
   public override string GetTokenExample() => EffectExample;
}

public class CultureEffect() : SimpleEffect<Culture>(Culture.Empty)
{
   public static readonly string EffectName = "culture";
   public static readonly string EffectDescription = $"Sets the {EffectName} of the province. ONLY valid in province history files.";
   public static readonly string EffectExample = $"{EffectName} = <culture>";
   

   public static IToken? CreateEffect(EnhancedBlock? block, LineKvp<string, string>? kvp, ParsingContext context, PathObj po)
   {
      Debug.Assert(kvp is not null, "At this point the kvp must not be null. This must be filtered earlier in the pipeline");

      CultureEffect token = new();
      return token.Parse(kvp.Value, po, context) ? token : null;
   }

   public override void Activate(ITarget target)
   {
      Debug.Assert(target is Province, $"'{EffectName}' effect is only valid on provinces");
      Debug.Assert(Culture.TryParse(_value.Val.Name, out _).Ignore(), "The culture must always exist to use it in execution");  
      
      ((Province)target).Culture = _value.Val;
   }

   public override string GetTokenName() => EffectName;
   public override string GetTokenDescription() => EffectDescription;
   public override string GetTokenExample() => EffectExample;
}

public class DiscoveredByEffect() : SimpleEffect<string>(string.Empty)
{
   public static readonly string EffectName = "discovered_by";
   public static readonly string EffectDescription = $"Grants the specified technology or tag group vision of this province. ONLY valid in province history.";
   public static readonly string EffectExample = $"{EffectName} = <tag | technology_group>";
   

   public static IToken? CreateEffect(EnhancedBlock? block, LineKvp<string, string>? kvp, ParsingContext context, PathObj po)
   {
      Debug.Assert(kvp is not null, "At this point the kvp must not be null. This must be filtered earlier in the pipeline");

      DiscoveredByEffect token = new();
      return token.Parse(kvp.Value, po, context) ? token : null;
   }

   public override bool Parse(LineKvp<string, string> command, PathObj po, ParsingContext context)
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
      _ = new LoadingError(po, $"Invalid value for {EffectName}: {command.Value}", line: command.Line, type: ErrorType.PCFL_TokenValidationError);
      return false;
   }

   public override void Activate(ITarget target)
   {
      Debug.Assert(target is Province, $"'{EffectName}' effect is only valid on provinces");
      Debug.Assert(Globals.Countries.Contains(_value.Val) || Globals.TechnologyGroups.Contains(_value.Val), "The country or tech group must always exist to use it in execution");
      ((Province)target).DiscoveredBy.Add(_value.Val);
   }

   public override string GetTokenName() => EffectName;
   public override string GetTokenDescription() => EffectDescription;
   public override string GetTokenExample() => EffectExample;
}

public class IsCityEffect() : SimpleEffect<bool>(false)
{
   public static readonly string EffectName = "is_city";
   public static readonly string EffectDescription = $"Whether this province is a proper city, i.e. not a colony. ONLY valid in province history files.";
   public static readonly string EffectExample = $"{EffectName} = <bool>";
   

   public static IToken? CreateEffect(EnhancedBlock? block, LineKvp<string, string>? kvp, ParsingContext context, PathObj po)
   {
      Debug.Assert(kvp is not null, "At this point the kvp must not be null. This must be filtered earlier in the pipeline");

      IsCityEffect token = new();
      return token.Parse(kvp.Value, po, context) ? token : null;
   }
   
   public override void Activate(ITarget target)
   {
      Debug.Assert(target is Province, $"'{EffectName}' effect is only valid on provinces");
      ((Province)target).IsCity = _value.Val;
   }

   public override string GetTokenName() => EffectName;
   public override string GetTokenDescription() => EffectDescription;
   public override string GetTokenExample() => EffectExample;
}

public class HreEffect() : SimpleEffect<bool>(false)
{
   public static readonly string EffectName = "hre";
   public static readonly string EffectDescription = $"Whether this province is in the HRE or not. ONLY valid in province history files.";
   public static readonly string EffectExample = $"{EffectName} = <bool>";
   

   public static IToken? CreateEffect(EnhancedBlock? block, LineKvp<string, string>? kvp, ParsingContext context, PathObj po)
   {
      Debug.Assert(kvp is not null, "At this point the kvp must not be null. This must be filtered earlier in the pipeline");

      HreEffect token = new();
      return token.Parse(kvp.Value, po, context) ? token : null;
   }
   
   public override void Activate(ITarget target)
   {
      Debug.Assert(target is Province, $"'{EffectName}' effect is only valid on provinces");
      ((Province)target).IsHre = _value.Val;
   }

   public override string GetTokenName() => EffectName;
   public override string GetTokenDescription() => EffectDescription;
   public override string GetTokenExample() => EffectExample;
}

public class SeatInParliamentEffect() : SimpleEffect<bool>(false)
{
   public static readonly string EffectName = "seat_in_parliament";
   public static readonly string EffectDescription = $"Gives or removes the province's seat in the parliament. ONLY valid in province history files.";
   public static readonly string EffectExample = $"{EffectName} = <bool>";
   

   public static IToken? CreateEffect(EnhancedBlock? block, LineKvp<string, string>? kvp, ParsingContext context, PathObj po)
   {
      Debug.Assert(kvp is not null, "At this point the kvp must not be null. This must be filtered earlier in the pipeline");

      SeatInParliamentEffect token = new();
      return token.Parse(kvp.Value, po, context) ? token : null;
   }
   
   public override void Activate(ITarget target)
   {
      Debug.Assert(target is Province, $"'{EffectName}' effect is only valid on provinces");
      ((Province)target).IsSeatInParliament = _value.Val;
   }

   public override string GetTokenName() => EffectName;
   public override string GetTokenDescription() => EffectDescription;
   public override string GetTokenExample() => EffectExample;
}

public class CapitalEffect() : SimpleEffect<string>(string.Empty)
{
   public static readonly string EffectName = "capital";
   public static readonly string EffectDescription = $"The capital name to use for the province, otherwise the province name is used as the capital name. ONLY valid in province history files.";
   public static readonly string EffectExample = $"{EffectName} = <string>";
   

   public static IToken? CreateEffect(EnhancedBlock? block, LineKvp<string, string>? kvp, ParsingContext context, PathObj po)
   {
      Debug.Assert(kvp is not null, "At this point the kvp must not be null. This must be filtered earlier in the pipeline");

      CapitalEffect token = new();
      return token.Parse(kvp.Value, po, context) ? token : null;
   }
   
   public override void Activate(ITarget target)
   {
      Debug.Assert(target is Province, $"'{EffectName}' effect is only valid on provinces");
      ((Province)target).Capital = _value.Val;
   }

   public override string GetTokenName() => EffectName;
   public override string GetTokenDescription() => EffectDescription;
   public override string GetTokenExample() => EffectExample;
}

public class TradeGoodsEffect() : SimpleEffect<TradeGood>(TradeGood.Empty)
{
   public static readonly string EffectName = "trade_goods";
   public static readonly string EffectDescription = $"The tradegood assigned to this province, from 00_tradegoods.txt. ONLY valid in province history files.";
   public static readonly string EffectExample = $"{EffectName} = <trade_good>";
   

   public static IToken? CreateEffect(EnhancedBlock? block, LineKvp<string, string>? kvp, ParsingContext context, PathObj po)
   {
      Debug.Assert(kvp is not null, "At this point the kvp must not be null. This must be filtered earlier in the pipeline");

      TradeGoodsEffect token = new();
      return token.Parse(kvp.Value, po, context) ? token : null;
   }
   
   public override void Activate(ITarget target)
   {
      Debug.Assert(target is Province, $"'{EffectName}' effect is only valid on provinces");
      Debug.Assert(TradeGood.TryParse(_value.Val.Name, out _).Ignore(), "The trade good must always exist to use it in execution");
      ((Province)target).TradeGood = _value.Val;
   }

   public override string GetTokenName() => EffectName;
   public override string GetTokenDescription() => EffectDescription;
   public override string GetTokenExample() => EffectExample;
}

public class TribalOwnerEffect() : SimpleEffect<Country>(Country.Empty)
{
   public static readonly string EffectName = "tribal_owner";
   public static readonly string EffectDescription = $"Sets the tribal owner of a province.";
   public static readonly string EffectExample = $"{EffectName} = <tag>";
   

   public static IToken? CreateEffect(EnhancedBlock? block, LineKvp<string, string>? kvp, ParsingContext context, PathObj po)
   {
      Debug.Assert(kvp is not null, "At this point the kvp must not be null. This must be filtered earlier in the pipeline");

      TribalOwnerEffect token = new();
      return token.Parse(kvp.Value, po, context) ? token : null;
   }

   public override void Activate(ITarget target)
   {
      Debug.Assert(target is Province, $"'{EffectName}' effect is only valid on provinces");
      Debug.Assert(Globals.Countries.TryGetValue(_value.Val.Tag, out _), "The country must always exist to use it in execution");
      ((Province)target).TribalOwner = _value.Val;
   }

   public override string GetTokenName() => EffectName;
   public override string GetTokenDescription() => EffectDescription;
   public override string GetTokenExample() => EffectExample;
}

public class AddLocalAutonomyEffect() : SimpleEffect<float>(0f)
{
   public static readonly string EffectName = "add_local_autonomy";
   public static readonly string EffectDescription = $"Adds local autonomy to the current province scope.";
   public static readonly string EffectExample = $"{EffectName} = 0.1";
   

   public static IToken? CreateEffect(EnhancedBlock? block, LineKvp<string, string>? kvp, ParsingContext context, PathObj po)
   {
      Debug.Assert(kvp is not null, "At this point the kvp must not be null. This must be filtered earlier in the pipeline");

      AddLocalAutonomyEffect token = new();
      return token.Parse(kvp.Value, po, context) ? token : null;
   }

   public override void Activate(ITarget target)
   {
      Debug.Assert(target is Province, $"'{EffectName}' effect is only valid on provinces");
      ((Province)target).LocalAutonomy += _value.Val;
   }

   public override string GetTokenName() => EffectName;
   public override string GetTokenDescription() => EffectDescription;
   public override string GetTokenExample() => EffectExample;
}

public class NativeSizeEffect() : SimpleEffect<int>(0)
{
   public static readonly string EffectName = "native_size";
   public static readonly string EffectDescription = $"Sets native_size of the current province scope. ONLY useable in province history!";
   public static readonly string EffectExample = $"{EffectName} = 50";
   

   public static IToken? CreateEffect(EnhancedBlock? block, LineKvp<string, string>? kvp, ParsingContext context, PathObj po)
   {
      Debug.Assert(kvp is not null, "At this point the kvp must not be null. This must be filtered earlier in the pipeline");

      NativeSizeEffect token = new();
      return token.Parse(kvp.Value, po, context) ? token : null;
   }

   public override void Activate(ITarget target)
   {
      Debug.Assert(target is Province, $"'{EffectName}' effect is only valid on provinces");
      ((Province)target).NativeSize = _value.Val;
   }

   public override string GetTokenName() => EffectName;
   public override string GetTokenDescription() => EffectDescription;
   public override string GetTokenExample() => EffectExample;
}

public class AddClaimEffect() : SimpleEffect<Country>(Country.Empty)
{
   public static readonly string EffectName = "add_claim";
   public static readonly string EffectDescription = $"The defined scope gains a claim on the current province scope. ";
   public static readonly string EffectExample = $"capital_scope = {{\n\tadd_claim = FRA\n}}";
   

   public static IToken? CreateEffect(EnhancedBlock? block, LineKvp<string, string>? kvp, ParsingContext context, PathObj po)
   {
      Debug.Assert(kvp is not null, "At this point the kvp must not be null. This must be filtered earlier in the pipeline");

      AddClaimEffect token = new();
      return token.Parse(kvp.Value, po, context) ? token : null;
   }

   public override void Activate(ITarget target)
   {
      Debug.Assert(target is Province, $"'{EffectName}' effect is only valid on provinces");
      Debug.Assert(Country.TryParse(_value.Val.Tag, out _).Ignore(), "The country must always exist to use it in execution");
      ((Province)target).Claims.Add(_value.Val.Tag);
   }

   public override string GetTokenName() => EffectName;
   public override string GetTokenDescription() => EffectDescription;
   public override string GetTokenExample() => EffectExample;
}

public class RemoveClaimEffect() : SimpleEffect<Country>(Country.Empty)
{
   public static readonly string EffectName = "remove_claim";
   public static readonly string EffectDescription = $"The defined scope gains a claim on the current province scope. ";
   public static readonly string EffectExample = $"capital_scope = {{\n\tremove_claim = FRA\n}}";
   

   public static IToken? CreateEffect(EnhancedBlock? block, LineKvp<string, string>? kvp, ParsingContext context, PathObj po)
   {
      Debug.Assert(kvp is not null, "At this point the kvp must not be null. This must be filtered earlier in the pipeline");

      RemoveClaimEffect token = new();
      return token.Parse(kvp.Value, po, context) ? token : null;
   }

   public override void Activate(ITarget target)
   {
      Debug.Assert(target is Province, $"'{EffectName}' effect is only valid on provinces");
      Debug.Assert(Country.TryParse(_value.Val.Tag, out _).Ignore(), "The country must always exist to use it in execution");
      ((Province)target).Claims.Remove(_value.Val.Tag);
   }

   public override string GetTokenName() => EffectName;
   public override string GetTokenDescription() => EffectDescription;
   public override string GetTokenExample() => EffectExample;
}

public class AddPermanentClaimEffect() : SimpleEffect<Country>(Country.Empty)
{
   public static readonly string EffectName = "add_permanent_claim";
   public static readonly string EffectDescription = $"Adds a permanent claim to the current province scope.";
   public static readonly string EffectExample = $"add_permanent_claim = {{\n\tname = <string>\n}}";
   

   public static IToken? CreateEffect(EnhancedBlock? block, LineKvp<string, string>? kvp, ParsingContext context, PathObj po)
   {
      Debug.Assert(kvp is not null, "At this point the kvp must not be null. This must be filtered earlier in the pipeline");

      AddPermanentClaimEffect token = new();
      return token.Parse(kvp.Value, po, context) ? token : null;
   }

   public override void Activate(ITarget target)
   {
      Debug.Assert(target is Province, $"'{EffectName}' effect is only valid on provinces");
      Debug.Assert(Country.TryParse(_value.Val.Tag, out _).Ignore(), "The country must always exist to use it in execution");
      ((Province)target).PermanentClaims.Add(_value.Val.Tag);
   }

   public override string GetTokenName() => EffectName;
   public override string GetTokenDescription() => EffectDescription;
   public override string GetTokenExample() => EffectExample;
}

public class RemovePermanentClaimEffect() : SimpleEffect<Country>(Country.Empty)
{
   public static readonly string EffectName = "remove_permanent_claim";
   public static readonly string EffectDescription = $"Removes a permanent claim from the current province scope.";
   public static readonly string EffectExample = $"remove_permanent_claim = {{\n\tname = <string>\n}}";
   

   public static IToken? CreateEffect(EnhancedBlock? block, LineKvp<string, string>? kvp, ParsingContext context, PathObj po)
   {
      Debug.Assert(kvp is not null, "At this point the kvp must not be null. This must be filtered earlier in the pipeline");

      RemovePermanentClaimEffect token = new();
      return token.Parse(kvp.Value, po, context) ? token : null;
   }

   public override void Activate(ITarget target)
   {
      Debug.Assert(target is Province, $"'{EffectName}' effect is only valid on provinces");
      Debug.Assert(Country.TryParse(_value.Val.Tag, out _).Ignore(), "The country must always exist to use it in execution");
      ((Province)target).PermanentClaims.Remove(_value.Val.Tag);
   }

   public override string GetTokenName() => EffectName;
   public override string GetTokenDescription() => EffectDescription;
   public override string GetTokenExample() => EffectExample;
}

public class AddPermanentProvinceModifierEffect : IToken
{
   public static readonly string EffectName = "add_permanent_province_modifier";
   public static readonly string EffectDescription = $"Adds an event modifier to the current province scope as a province modifier. ";
   public static readonly string EffectExample = $"add_permanent_province_modifier = {{\n\tname = annoyed_people\n\tduration = -1\n\tdesc = annoyed_people_tooltip\n\thidden = yes\n}}";
   

   internal Value<string> Name= new(string.Empty);
   internal Value<string> Desc = new(string.Empty);
   internal Value<int> Duration = new(-1);
   internal Value<bool> Hidden = new(false);

   public static IToken? CreateEffect(EnhancedBlock? block, LineKvp<string, string>? kvp, ParsingContext context, PathObj po)
   {
      Debug.Assert(block is not null, "At this point the block must not be null. This must be filtered earlier in the pipeline");

      AddPermanentProvinceModifierEffect token = new();
      return token.Parse(block, po, context) ? token : null;
   }

   public bool Parse(EnhancedBlock block, PathObj po, ParsingContext context)
   {
      foreach (var element in block.GetContentElements(true, po))
      {
         foreach (var kvp in element.GetLineKvpEnumerator(po))
         {
            switch (kvp.Key)
            {
               case "name":
                  {
                     if (!GeneralFileParser.ParseSingleTriggerVal(ref Name, kvp, po, context))
                        return false;
                     break;
                  }
               case "duration":
                  {
                     if (!GeneralFileParser.ParseSingleTriggerVal(ref Duration, kvp, po, context))
                        return false;
                     break;
                  }
               case "hidden":
                  {
                     if (!GeneralFileParser.ParseSingleTriggerVal(ref Hidden, kvp, po, context))
                        return false;
                     break;
                  }
               case "desc":
                  {
                     if (!GeneralFileParser.ParseSingleTriggerVal(ref Name, kvp, po, context))
                        return false;
                     break;
                  }
               default:
                  _ = new LoadingError(po, $"Invalid key in {EffectName}: {kvp.Key}", line: kvp.Line, type: ErrorType.PCFL_TokenValidationError);
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

      ((Province)target).PermanentProvinceModifiers.Add(new (Name.Val, Duration.Val)); // TODO proper modifier implementation
   }

   public void GetTokenString(int tabs, ref StringBuilder sb)
   {
      SavingUtil.OpenBlock(ref tabs, EffectName, ref sb);
      SavingUtil.AddString(tabs, Name.Val, "name", ref sb);
      SavingUtil.AddInt(tabs, Duration.Val, "duration", ref sb);
      SavingUtil.AddBoolIfYes(tabs, Hidden.Val, "hidden", ref sb);
      SavingUtil.AddString(tabs, Desc.Val, "desc", ref sb);
      SavingUtil.CloseBlock(ref tabs, ref sb);
   }

   public string GetTokenName() => EffectName;
   public string GetTokenDescription() => EffectDescription;
   public string GetTokenExample() => EffectExample;
}

public class AddTradeCompanyInvestmentModifierEffect : IToken
{
   public static readonly string EffectName = "add_trade_company_investment";
   public static readonly string EffectDescription = $"Adds a trade company investment in the provinces area.";
   public static readonly string EffectExample = $"add_trade_company_investment = {{\n\t\tinvestment = local_quarter\n\t\tinvestor = NED\n\t}}";
   

   internal Value<string> Investment = new(string.Empty);
   internal Value<string> Investor = new(string.Empty);

   public static IToken? CreateEffect(EnhancedBlock? block, LineKvp<string, string>? kvp, ParsingContext context, PathObj po)
   {
      Debug.Assert(block is not null, "At this point the block must not be null. This must be filtered earlier in the pipeline");

      AddTradeCompanyInvestmentModifierEffect token = new();
      return token.Parse(block, po, context) ? token : null;
   }

   public bool Parse(EnhancedBlock block, PathObj po, ParsingContext context)
   {
      foreach (var element in block.GetContentElements(true, po))
      {
         foreach (var kvp in element.GetLineKvpEnumerator(po))
         {
            switch (kvp.Key)
            {
               case "investment":
                  if (!GeneralFileParser.ParseSingleTriggerVal(ref Investment, kvp, po, context))
                     return false;
                  break;
               case "investor":
                  if (!GeneralFileParser.ParseSingleTriggerVal(ref Investor, kvp, po, context))
                     return false;
                  break;
               default:
                  _ = new LoadingError(po, $"Invalid key in {EffectName}: {kvp.Key}", line: kvp.Line, type: ErrorType.PCFL_TokenValidationError);
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

      ((Province)target).TradeCompanyInvestments.Add(Investment.Val); // TODO proper modifier implementation
   }

   public void GetTokenString(int tabs, ref StringBuilder sb)
   {
      SavingUtil.OpenBlock(ref tabs, EffectName, ref sb);
      SavingUtil.AddString(tabs, Investment.Val, "name", ref sb);
      SavingUtil.AddString(tabs, Investment.Val, "desc", ref sb);
      SavingUtil.CloseBlock(ref tabs, ref sb);
   }

   public string GetTokenName() => EffectName;
   public string GetTokenDescription() => EffectDescription;
   public string GetTokenExample() => EffectExample;
}

public class NativeFerocityEffect() : SimpleEffect<float>(0)
{
   public static readonly string EffectName = "native_ferocity";
   public static readonly string EffectDescription = $"Adds to the Native Ferocity within an uncolonized province. ";
   public static readonly string EffectExample = $"native_ferocity = 5";
   

   public static IToken? CreateEffect(EnhancedBlock? block, LineKvp<string, string>? kvp, ParsingContext context, PathObj po)
   {
      Debug.Assert(kvp is not null, "At this point the kvp must not be null. This must be filtered earlier in the pipeline");

      NativeFerocityEffect token = new();
      return token.Parse(kvp.Value, po, context) ? token : null;
   }

   public override void Activate(ITarget target)
   {
      Debug.Assert(target is Province, $"'{EffectName}' effect is only valid on provinces");
      ((Province)target).NativeFerocity = _value.Val;
   }

   public override string GetTokenName() => EffectName;
   public override string GetTokenDescription() => EffectDescription;
   public override string GetTokenExample() => EffectExample;
}
public class NativeHostilnessEffect() : SimpleEffect<int>(0)
{
   public static readonly string EffectName = "native_hostileness";
   public static readonly string EffectDescription = $"Adds to the Native Hostileness within an uncolonized province. ";
   public static readonly string EffectExample = $"native_hostileness = 5";
   

   public static IToken? CreateEffect(EnhancedBlock? block, LineKvp<string, string>? kvp, ParsingContext context, PathObj po)
   {
      Debug.Assert(kvp is not null, "At this point the kvp must not be null. This must be filtered earlier in the pipeline");

      NativeHostilnessEffect token = new();
      return token.Parse(kvp.Value, po, context) ? token : null;
   }

   public override void Activate(ITarget target)
   {
      Debug.Assert(target is Province, $"'{EffectName}' effect is only valid on provinces");
      ((Province)target).NativeHostileness = _value.Val;
   }

   public override string GetTokenName() => EffectName;
   public override string GetTokenDescription() => EffectDescription;
   public override string GetTokenExample() => EffectExample;
}
public class AddProvinceTriggeredModifierEffect() : SimpleEffect<string>(string.Empty)
{
   public static readonly string EffectName = "add_province_triggered_modifier";
   public static readonly string EffectDescription = $"Adds a province triggered modifier to the current province scope. ";
   public static readonly string EffectExample = $"add_province_triggered_modifier = test_modifier";
   

   public static IToken? CreateEffect(EnhancedBlock? block, LineKvp<string, string>? kvp, ParsingContext context, PathObj po)
   {
      Debug.Assert(kvp is not null, "At this point the kvp must not be null. This must be filtered earlier in the pipeline");

      AddProvinceTriggeredModifierEffect token = new();
      return token.Parse(kvp.Value, po, context) ? token : null;
   }

   public override void Activate(ITarget target)
   {
      Debug.Assert(target is Province, $"'{EffectName}' effect is only valid on provinces");
      ((Province)target).ProvinceTriggeredModifiers.Add(_value.Val);
   }

   public override string GetTokenName() => EffectName;
   public override string GetTokenDescription() => EffectDescription;
   public override string GetTokenExample() => EffectExample;
}
public class RemoveProvinceTriggeredModifierEffect() : SimpleEffect<string>(string.Empty)
{
   public static readonly string EffectName = "remove_province_triggered_modifier";
   public static readonly string EffectDescription = $"Removes a province triggered modifier to the current province scope. ";
   public static readonly string EffectExample = $"remove_province_triggered_modifier = test_modifier";
   

   public static IToken? CreateEffect(EnhancedBlock? block, LineKvp<string, string>? kvp, ParsingContext context, PathObj po)
   {
      Debug.Assert(kvp is not null, "At this point the kvp must not be null. This must be filtered earlier in the pipeline");

      RemoveProvinceTriggeredModifierEffect token = new();
      return token.Parse(kvp.Value, po, context) ? token : null;
   }

   public override void Activate(ITarget target)
   {
      Debug.Assert(target is Province, $"'{EffectName}' effect is only valid on provinces");
      ((Province)target).ProvinceTriggeredModifiers.Remove(_value.Val);
   }

   public override string GetTokenName() => EffectName;
   public override string GetTokenDescription() => EffectDescription;
   public override string GetTokenExample() => EffectExample;
}

public class AddToTradeCompanyEffect() : SimpleEffect<Country>(Country.Empty)
{
   public static readonly string EffectName = "add_to_trade_company";
   public static readonly string EffectDescription = $"The defined scope gains a claim on the current province scope. ";
   public static readonly string EffectExample = $"add_to_trade_company = FRA";
   

   public static IToken? CreateEffect(EnhancedBlock? block, LineKvp<string, string>? kvp, ParsingContext context, PathObj po)
   {
      Debug.Assert(kvp is not null, "At this point the kvp must not be null. This must be filtered earlier in the pipeline");

      AddToTradeCompanyEffect token = new();
      return token.Parse(kvp.Value, po, context) ? token : null;
   }

   public override void Activate(ITarget target)
   {
      Debug.Assert(target is Province, $"'{EffectName}' effect is only valid on provinces");
      Debug.Assert(Country.TryParse(_value.Val.Tag, out _).Ignore(), "The country must always exist to use it in execution");
      // TODO
   }

   public override string GetTokenName() => EffectName;
   public override string GetTokenDescription() => EffectDescription;
   public override string GetTokenExample() => EffectExample;
}
public class ExtraCostEffect() : SimpleEffect<int>(0)
{
   public static readonly string EffectName = "extra_cost";
   public static readonly string EffectDescription = "Sets an additional cost to the province in custom nation creation. ONLY in province histrory files.";
   public static readonly string EffectExample = "extra_cost = 25";


   public static IToken? CreateEffect(EnhancedBlock? block, LineKvp<string, string>? kvp, ParsingContext context, PathObj po)
   {
      Debug.Assert(kvp is not null, "At this point the kvp must not be null. This must be filtered earlier in the pipeline");

      ExtraCostEffect token = new();
      return token.Parse(kvp.Value, po, context) ? token : null;
   }

   public override void Activate(ITarget target)
   {
      Debug.Assert(target is Province, $"'{EffectName}' effect is only valid on provinces");
      ((Province)target).ExtraCost = _value.Val;
   }

   public override string GetTokenName() => EffectName;
   public override string GetTokenDescription() => EffectDescription;
   public override string GetTokenExample() => EffectExample;
}
public class CenterOfTradeEffect() : SimpleEffect<int>(0)
{
   public static readonly string EffectName = "center_of_trade";
   public static readonly string EffectDescription = "Creates a center of trade in the province. Only works if there is no existing center of trade.";
   public static readonly string EffectExample = "center_of_trade = 2";


   public static IToken? CreateEffect(EnhancedBlock? block, LineKvp<string, string>? kvp, ParsingContext context, PathObj po)
   {
      Debug.Assert(kvp is not null, "At this point the kvp must not be null. This must be filtered earlier in the pipeline");

      CenterOfTradeEffect token = new();
      return token.Parse(kvp.Value, po, context) ? token : null;
   }

   public override void Activate(ITarget target)
   {
      Debug.Assert(target is Province, $"'{EffectName}' effect is only valid on provinces");
      ((Province)target).CenterOfTrade = _value.Val;
   }

   public override string GetTokenName() => EffectName;
   public override string GetTokenDescription() => EffectDescription;
   public override string GetTokenExample() => EffectExample;
}
public class RemoveProvinceModifierEffect() : SimpleEffect<string>(string.Empty)
{
   public static readonly string EffectName = "remove_province_modifier";
   public static readonly string EffectDescription = "Removes an already assigned province modifier from the current province scope.";
   public static readonly string EffectExample = "remove_province_modifier = example_modifier";


   public static IToken? CreateEffect(EnhancedBlock? block, LineKvp<string, string>? kvp, ParsingContext context, PathObj po)
   {
      Debug.Assert(kvp is not null, "At this point the kvp must not be null. This must be filtered earlier in the pipeline");

      RemoveProvinceModifierEffect token = new();
      return token.Parse(kvp.Value, po, context) ? token : null;
   }

   public override void Activate(ITarget target)
   {
      Debug.Assert(target is Province, $"'{EffectName}' effect is only valid on provinces");
      var mod = ((Province)target).ProvinceModifiers.FirstOrDefault(x => x.Name == _value.Val);
      if (mod is not null)
         ((Province)target).ProvinceModifiers.Remove(mod);
   }

   public override string GetTokenName() => EffectName;
   public override string GetTokenDescription() => EffectDescription;
   public override string GetTokenExample() => EffectExample;
}
public class SetGlobalFlagEffect() : SimpleEffect<string>(string.Empty)
{
   public static readonly string EffectName = "set_global_flag";
   public static readonly string EffectDescription = "Defines a global flag.";
   public static readonly string EffectExample = "set_global_flag = example_flag";
   
   public static IToken? CreateEffect(EnhancedBlock? block, LineKvp<string, string>? kvp, ParsingContext context, PathObj po)
   {
      Debug.Assert(kvp is not null, "At this point the kvp must not be null. This must be filtered earlier in the pipeline");

      SetGlobalFlagEffect token = new();
      return token.Parse(kvp.Value, po, context) ? token : null;
   }

   public override void Activate(ITarget target)
   {
      Debug.Assert(target is Province, $"'{EffectName}' effect is only valid on provinces");
      // TODO implement flag tracking
   }

   public override string GetTokenName() => EffectName;
   public override string GetTokenDescription() => EffectDescription;
   public override string GetTokenExample() => EffectExample;
}
public class ChangeProvinceNameEffect() : SimpleEffect<string>(string.Empty)
{
   public static readonly string EffectName = "change_province_name";
   public static readonly string EffectDescription = "Sets a new province name.";
   public static readonly string EffectExample = "change_province_name = \"New Berlin\"";
   
   public static IToken? CreateEffect(EnhancedBlock? block, LineKvp<string, string>? kvp, ParsingContext context, PathObj po)
   {
      Debug.Assert(kvp is not null, "At this point the kvp must not be null. This must be filtered earlier in the pipeline");

      ChangeProvinceNameEffect token = new();
      return token.Parse(kvp.Value, po, context) ? token : null;
   }

   public override void Activate(ITarget target)
   {
      Debug.Assert(target is Province, $"'{EffectName}' effect is only valid on provinces");
      Localisation.AddOrModifyLocObjectSilent(((Province)target).TitleKey, _value.Val);
   }

   public override string GetTokenName() => EffectName;
   public override string GetTokenDescription() => EffectDescription;
   public override string GetTokenExample() => EffectExample;
}
public class RenameCapitalEffect() : SimpleEffect<string>(string.Empty)
{
   public static readonly string EffectName = "rename_capital";
   public static readonly string EffectDescription = "Sets a new capital name for the province.";
   public static readonly string EffectExample = "rename_capital = \"Mega Berlin\"";
   
   public static IToken? CreateEffect(EnhancedBlock? block, LineKvp<string, string>? kvp, ParsingContext context, PathObj po)
   {
      Debug.Assert(kvp is not null, "At this point the kvp must not be null. This must be filtered earlier in the pipeline");

      RenameCapitalEffect token = new();
      return token.Parse(kvp.Value, po, context) ? token : null;
   }

   public override void Activate(ITarget target)
   {
      Debug.Assert(target is Province, $"'{EffectName}' effect is only valid on provinces");
      ((Province)target).Capital = _value.Val;
   }

   public override string GetTokenName() => EffectName;
   public override string GetTokenDescription() => EffectDescription;
   public override string GetTokenExample() => EffectExample;
}
public class AddProsperityEffect() : SimpleEffect<float>(0)
{
   public static readonly string EffectName = "add_prosperity";
   public static readonly string EffectDescription = "Adds prosperity to the current province scope.";
   public static readonly string EffectExample = "add_prosperity = 25";
   
   public static IToken? CreateEffect(EnhancedBlock? block, LineKvp<string, string>? kvp, ParsingContext context, PathObj po)
   {
      Debug.Assert(kvp is not null, "At this point the kvp must not be null. This must be filtered earlier in the pipeline");

      AddProsperityEffect token = new();
      return token.Parse(kvp.Value, po, context) ? token : null;
   }

   public override void Activate(ITarget target)
   {
      Debug.Assert(target is Province, $"'{EffectName}' effect is only valid on provinces");
      ((Province)target).Prosperity += _value.Val;
   }

   public override string GetTokenName() => EffectName;
   public override string GetTokenDescription() => EffectDescription;
   public override string GetTokenExample() => EffectExample;
}
public class AddDevastationEffect() : SimpleEffect<float>(0)
{
   public static readonly string EffectName = "add_devastation";
   public static readonly string EffectDescription = "Adds devastation to the current province scope.";
   public static readonly string EffectExample = "add_devastation = 25";
   
   public static IToken? CreateEffect(EnhancedBlock? block, LineKvp<string, string>? kvp, ParsingContext context, PathObj po)
   {
      Debug.Assert(kvp is not null, "At this point the kvp must not be null. This must be filtered earlier in the pipeline");

      AddDevastationEffect token = new();
      return token.Parse(kvp.Value, po, context) ? token : null;
   }

   public override void Activate(ITarget target)
   {
      Debug.Assert(target is Province, $"'{EffectName}' effect is only valid on provinces");
      ((Province)target).Devastation += _value.Val;
   }

   public override string GetTokenName() => EffectName;
   public override string GetTokenDescription() => EffectDescription;
   public override string GetTokenExample() => EffectExample;
}

public class SetLocalAutonomyEffect() : SimpleEffect<float>(0)
{
   public static readonly string EffectName = "set_local_autonomy";
   public static readonly string EffectDescription = "Sets local autonomy to the current province scope.";
   public static readonly string EffectExample = "set_local_autonomy = 25";
   
   public static IToken? CreateEffect(EnhancedBlock? block, LineKvp<string, string>? kvp, ParsingContext context, PathObj po)
   {
      Debug.Assert(kvp is not null, "At this point the kvp must not be null. This must be filtered earlier in the pipeline");

      SetLocalAutonomyEffect token = new();
      return token.Parse(kvp.Value, po, context) ? token : null;
   }

   public override void Activate(ITarget target)
   {
      Debug.Assert(target is Province, $"'{EffectName}' effect is only valid on provinces");
      ((Province)target).LocalAutonomy = _value.Val;
   }

   public override string GetTokenName() => EffectName;
   public override string GetTokenDescription() => EffectDescription;
   public override string GetTokenExample() => EffectExample;
}
public class ChangeTradeGoodsEffect() : SimpleEffect<TradeGood>(TradeGood.Empty)
{
   public static readonly string EffectName = "change_trade_goods";
   public static readonly string EffectDescription = "Sets the trade good for the current province scope.";
   public static readonly string EffectExample = "change_trade_goods = <trade_good>";
   
   public static IToken? CreateEffect(EnhancedBlock? block, LineKvp<string, string>? kvp, ParsingContext context, PathObj po)
   {
      Debug.Assert(kvp is not null, "At this point the kvp must not be null. This must be filtered earlier in the pipeline");

      ChangeTradeGoodsEffect token = new();
      return token.Parse(kvp.Value, po, context) ? token : null;
   }

   public override void Activate(ITarget target)
   {
      Debug.Assert(target is Province, $"'{EffectName}' effect is only valid on provinces");
      ((Province)target).TradeGood = _value.Val;
   }

   public override string GetTokenName() => EffectName;
   public override string GetTokenDescription() => EffectDescription;
   public override string GetTokenExample() => EffectExample;
}
public class AddScaledLocalAdmPowerEffect() : SimpleEffect<int>(0)
{
   public static readonly string EffectName = "add_scaled_local_adm_power";
   public static readonly string EffectDescription = "Awards x adm power per base_tax of the current scope";
   public static readonly string EffectExample = "add_scaled_local_adm_power = <int>";
   
   public static IToken? CreateEffect(EnhancedBlock? block, LineKvp<string, string>? kvp, ParsingContext context, PathObj po)
   {
      Debug.Assert(kvp is not null, "At this point the kvp must not be null. This must be filtered earlier in the pipeline");

      AddScaledLocalAdmPowerEffect token = new();
      return token.Parse(kvp.Value, po, context) ? token : null;
   }

   public override void Activate(ITarget target)
   {
      Debug.Assert(target is Province, $"'{EffectName}' effect is only valid on provinces");
      // TODO further country gamestate needed
   }

   public override string GetTokenName() => EffectName;
   public override string GetTokenDescription() => EffectDescription;
   public override string GetTokenExample() => EffectExample;
}

public class AddScaledLocalDipPowerEffect() : SimpleEffect<int>(0)
{
   public static readonly string EffectName = "add_scaled_local_dip_power";
   public static readonly string EffectDescription = "Awards x dip power per base_tax of the current scope";
   public static readonly string EffectExample = "add_scaled_local_dip_power = <int>";
   
   public static IToken? CreateEffect(EnhancedBlock? block, LineKvp<string, string>? kvp, ParsingContext context, PathObj po)
   {
      Debug.Assert(kvp is not null, "At this point the kvp must not be null. This must be filtered earlier in the pipeline");

      AddScaledLocalDipPowerEffect token = new();
      return token.Parse(kvp.Value, po, context) ? token : null;
   }

   public override void Activate(ITarget target)
   {
      Debug.Assert(target is Province, $"'{EffectName}' effect is only valid on provinces");
      // TODO further country gamestate needed
   }

   public override string GetTokenName() => EffectName;
   public override string GetTokenDescription() => EffectDescription;
   public override string GetTokenExample() => EffectExample;
}
public class AddScaledLocalMilPowerEffect() : SimpleEffect<int>(0)
{
   public static readonly string EffectName = "add_scaled_local_mil_power";
   public static readonly string EffectDescription = "Awards x mil power per base_tax of the current scope";
   public static readonly string EffectExample = "add_scaled_local_mil_power = <int>";
   
   public static IToken? CreateEffect(EnhancedBlock? block, LineKvp<string, string>? kvp, ParsingContext context, PathObj po)
   {
      Debug.Assert(kvp is not null, "At this point the kvp must not be null. This must be filtered earlier in the pipeline");

      AddScaledLocalMilPowerEffect token = new();
      return token.Parse(kvp.Value, po, context) ? token : null;
   }

   public override void Activate(ITarget target)
   {
      Debug.Assert(target is Province, $"'{EffectName}' effect is only valid on provinces");
      // TODO further country gamestate needed
   }

   public override string GetTokenName() => EffectName;
   public override string GetTokenDescription() => EffectDescription;
   public override string GetTokenExample() => EffectExample;
}
public class CancelConstructionEffect() : SimpleEffect<bool>(false)
{
   public static readonly string EffectName = "cancel_construction";
   public static readonly string EffectDescription = "Cancels all active construction in the currenct scope";
   public static readonly string EffectExample = "cancel_construction = <bool>";
   
   public static IToken? CreateEffect(EnhancedBlock? block, LineKvp<string, string>? kvp, ParsingContext context, PathObj po)
   {
      Debug.Assert(kvp is not null, "At this point the kvp must not be null. This must be filtered earlier in the pipeline");

      CancelConstructionEffect token = new();
      return token.Parse(kvp.Value, po, context) ? token : null;
   }

   public override void Activate(ITarget target)
   {
      Debug.Assert(target is Province, $"'{EffectName}' effect is only valid on provinces");
      // TODO implement construction in Provinces
   }

   public override string GetTokenName() => EffectName;
   public override string GetTokenDescription() => EffectDescription;
   public override string GetTokenExample() => EffectExample;
}
