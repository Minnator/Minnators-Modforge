namespace Editor.Helper
{
   public static class Localisation
   {
      
      public static string GetLoc (string key)
      {
         return Globals.Localisation.GetValueOrDefault(key, key);
      }

   }
}