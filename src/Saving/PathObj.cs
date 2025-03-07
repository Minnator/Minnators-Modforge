using System.Collections;
using System.Reflection.Metadata.Ecma335;

namespace Editor.Saving;


public class PathObj
{
   public string[] Path;
   public bool IsModPath;
   public readonly bool IsExternal = false;
   public bool IsLocalisation => Path.Contains("localisation");
   public static readonly PathObj Empty = new([], false);

   public PathObj(string[] path, bool isModPathInit, bool isExternal = false)
   {
      Path = path;
      IsModPath = isModPathInit;
      IsExternal = isExternal;
   }


   public PathObj Copy(bool modPath, bool addReplacePath = false)
   {
      if (!addReplacePath)
         return new(Path, modPath);
      var pathParts = new string[Path.Length + 1];
      Array.Copy(Path, pathParts, Path.Length - 1);
      pathParts[^2] = "replace";
      pathParts[^1] = Path[^1];
      return new(pathParts, modPath);
   }

   public static PathObj FromPath(string path) => FromPath(path, path.StartsWith(Globals.ModPath));

   public static PathObj FromExternalPath(string path) => new(path.Split(System.IO.Path.DirectorySeparatorChar), false, true);

   public static PathObj FromPath(string path, bool isModPath)
   {
      return new(isModPath
         ? path.Remove(0, Globals.ModPath.Length + System.IO.Path.DirectorySeparatorChar.ToString().Length).Split(System.IO.Path.DirectorySeparatorChar)
         : path.Remove(0, Globals.VanillaPath.Length + System.IO.Path.DirectorySeparatorChar.ToString().Length).Split(System.IO.Path.DirectorySeparatorChar)
         , isModPath);
   }

   public string GetPath()
   {
      return System.IO.Path.Combine(IsModPath ? Globals.ModPath : Globals.VanillaPath, System.IO.Path.Combine(Path));
   }

   public string GetInternalPath() => System.IO.Path.Combine(Path);

   public string GetFolderPath()
   {
      return System.IO.Path.Combine(IsModPath ? Globals.ModPath : Globals.VanillaPath, System.IO.Path.Combine(Path[..^1]));
   }

   public string ToModPath()
   {
      return System.IO.Path.Combine(Globals.ModPath, System.IO.Path.Combine(Path));
   }

   public static bool operator ==(PathObj a, PathObj b)
   {
      return a.Path == b.Path && a.IsModPath == b.IsModPath;
   }

   public static bool operator !=(PathObj a, PathObj b)
   {
      return a.Path != b.Path || a.IsModPath != b.IsModPath;
   }

   public override int GetHashCode()
   {
      return HashCode.Combine(IsModPath.GetHashCode(), StructuralComparisons.StructuralEqualityComparer.GetHashCode(Path));
   }

   public override bool Equals(object? obj)
   {
      if (obj is not PathObj other)
         return false;
      return IsModPath == other.IsModPath && Path.SequenceEqual(other.Path);
   }

   public override string ToString()
   {
      return (IsModPath ? $"Mod: " : $"Vanilla: ") + System.IO.Path.Combine(Path);
   }

}
