using System.Diagnostics;
using Editor;
using Editor.DataClasses.Saveables;
using Editor.Loading.Enhanced;
using Editor.Loading.Enhanced.PCFL.Implementation;
using Editor.Loading.Enhanced.PCFL.Implementation.CountryScope;
using Editor.Saving;

public class OwnerProvince_Scope() : TokenScopeSwitch(Scopes.Province)
{
   public static readonly string EFFECT_NAME = "owner";
   public static readonly string EFFECT_DESC = "Scopes to current owner of the province";
   public static readonly string EFFECT_EXAMPLE = "owner = { <Effect> }";

   public override List<ITarget> GetTargets(ITarget target)
   {
      Debug.Assert(target is Province, $"'{EFFECT_NAME}' effect is only valid on countries");

      return [((Province)target).Owner];
   }

   public override string GetTokenName() => EFFECT_NAME;
   public override string GetTokenDescription() => EFFECT_DESC;

   public override string GetTokenExample() => EFFECT_EXAMPLE;

   public static IToken CreateToken(EnhancedBlock? block, LineKvp<string, string>? kvp, ParsingContext context, PathObj po)
   {
      Debug.Assert(block is not null, "At this point the block must not be null. This must be filtered earlier in the pipeline");

      var switchScope = new OwnerProvince_Scope();
      if (switchScope.Parse(block, po, context))
         return switchScope;
      return IToken.Empty;
   }
}