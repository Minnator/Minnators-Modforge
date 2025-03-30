using System.Diagnostics;
using Editor.DataClasses.GameDataClasses;
using Editor.DataClasses.Saveables;
using Editor.Saving;

namespace Editor.Loading.Enhanced.PCFL.Implementation.ProvinceScope
{
   public static class RuntimeEffects
   {
      public static void GenerateBuildingEffects()
      {
         foreach (var building in Globals.Buildings) 
            Scopes.Province.Effects.Add(building.Name, GenerateBuildingEffect(building));
      }

      public class BuildingEffect(Building building) : SimpleEffect<bool>(false) 
      {
         private const string EFFECT_DESCRIPTION = $"Adds or removes the specified building. ONLY valid in province history files.";
         private const string EFFECT_EXAMPLE = $"<building> = <bool>";

         public BuildingEffect() : this(Building.Empty)
         {

         }

         public static IToken? CreateEffect(EnhancedBlock? block, LineKvp<string, string>? kvp, ParsingContext context, PathObj po, Building building)
         {
            Debug.Assert(kvp is not null, "At this point the kvp must not be null. This must be filtered earlier in the pipeline");

            BuildingEffect token = new(building);
            return token.Parse(kvp.Value, po, context) ? token : null;
         }

         public override void Activate(ITarget target)
         {
            Debug.Assert(target is Province, $"'{building.Name}' effect is only valid on provinces");
            if(_value.Val)
               ((Province)target).Buildings.Add(building);
            else
               ((Province)target).Buildings.Remove(building);
         }

         public override string GetTokenName() => building.Name;
         public override string GetTokenDescription() => EFFECT_DESCRIPTION;
         public override string GetTokenExample() => EFFECT_EXAMPLE;
      }

      private static PCFL_Scope.PCFL_TokenParseDelegate GenerateBuildingEffect(Building building)
      {
         return (block, kvp, context, po) => BuildingEffect.CreateEffect(block, kvp, context, po, building);
      }

   }
}