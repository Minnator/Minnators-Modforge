﻿
using System.Diagnostics;
using System.Text;
using Editor.DataClasses.GameDataClasses;
using Editor.DataClasses.Saveables;
using Editor.DataClasses.Settings;
using Editor.ErrorHandling;
using Editor.Helper;
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
   private const string EFFECT_EXAMPLE = $"{EFFECT_NAME} = TAG";
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

public class RevoltEffect : IToken
{
   public const string EFFECT_NAME = "revolt";
   private const string EFFECT_DESCRIPTION = $"Adds a revolt";
   private const string EFFECT_EXAMPLE = $"revolt = {{\n\ttype = <rebel type>\n\tsize = <int>\n\tleader = <string>\n}}";
   private const ScopeType EFFECT_SCOPE = ScopeType.Province;

   internal Value<string> type = new(string.Empty);
   internal Value<int> size = new(-1);
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
      SavingUtil.AddString(tabs, type.Val, "type", ref sb);
      SavingUtil.AddIntIfNotValue(tabs, size.Val, "size", -1, ref sb);
      SavingUtil.AddQuotedString(tabs, leader.Val, "leader", ref sb);
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
