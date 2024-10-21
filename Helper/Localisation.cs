using Editor.Interfaces;

namespace Editor.Helper;

public static class Localisation
{
   public static string GetLoc (string key)
   {
      foreach (var loc in Globals.ReplaceLocalisation.Values)
      {
         if (loc.TryGetValue(key, out var value))
            return value;
      }

      foreach (var loc in Globals.ModLocalisation.Values)
      {
         if (loc.TryGetValue(key, out var value))
            return value;
      }

      foreach (var loc in Globals.VanillaLocalisation.Values)
      {
         if (loc.TryGetValue(key, out var value))
            return value;
      }

      return key;
   }

   public static bool GetFileForKey(string key, out string file)
   {
      foreach (var loc in Globals.ReplaceLocalisation)
      {
         if (loc.Value.ContainsKey(key))
         {
            file = loc.Key;
            return true;
         }
      }

      foreach (var loc in Globals.ModLocalisation)
      {
         if (loc.Value.ContainsKey(key))
         {
            file = loc.Key;
            return true;
         }
      }

      foreach (var loc in Globals.VanillaLocalisation)
      {
         if (loc.Value.ContainsKey(key))
         {
            file = loc.Key;
            return true;
         }
      }

      file = string.Empty;
      return false;
   }

}

public class LocObject(string key, string value) : ISaveable
{
   public string Key { get; } = key;
   public string Value { get; set; } = value;
   public ObjEditingStatus EditingStatus { get; set; }
   public int FileIndex { get; set; }

   public override int GetHashCode()
   {
      return Key.GetHashCode();
   }
}