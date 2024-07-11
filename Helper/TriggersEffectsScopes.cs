namespace Editor.Helper
{
   internal static class TriggersEffectsScopes
   {

      public static bool IsTrigger(string input)
      {
         return Globals.Triggers.Contains(input);
      }

      public static bool IsEffect(string input)
      {
         return Globals.Effects.Contains(input);
      }

      public static bool IsScope(string input)
      {
         if (Globals.Scopes.Contains(input))
            return true;
         if (Globals.Areas.ContainsKey(input))
            return true;
         if (Globals.Regions.ContainsKey(input))
            return true;
         if (Globals.SuperRegions.ContainsKey(input))
            return true;
         if (Globals.Continents.ContainsKey(input))
            return true;
         if (!int.TryParse(input, out var id))
            return false;
         if (Globals.Provinces.ContainsKey(id))
            return true;
         return false;
      }

      public static bool IsConditionStatement(string input)
      {
         return Globals.Conditions.Contains(input);
      }

   }
}
