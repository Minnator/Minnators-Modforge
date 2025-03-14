using System.Collections;
using System.Reflection.Metadata.Ecma335;

namespace Editor.Saving;


public class PathObj
{
   private readonly string[] _path;
   public bool IsModPath;
   public readonly bool IsExternal = false;
   public bool IsLocalisation => _path.Contains("localisation");
   public static readonly PathObj Empty = new([], false);

   public PathObj(string[] path, bool isModPathInit, bool isExternal = false)
   {
      _path = path;
      IsModPath = isModPathInit;
      IsExternal = isExternal;
   }


   public PathObj Copy(bool modPath, bool addReplacePath = false)
   {
      if (!addReplacePath)
         return new(_path, modPath);
      var pathParts = new string[_path.Length + 1];
      Array.Copy(_path, pathParts, _path.Length - 1);
      pathParts[^2] = "replace";
      pathParts[^1] = _path[^1];
      return new(pathParts, modPath);
   }

   public static PathObj FromPath(string path) => FromPath(path, path.StartsWith(Globals.ModPath));

   public static PathObj FromExternalPath(string path) => new(path.Split(Path.DirectorySeparatorChar), false, true);

   public static PathObj FromPath(string path, bool isModPath)
   {
      return new(isModPath
         ? path.Remove(0, Globals.ModPath.Length + Path.DirectorySeparatorChar.ToString().Length).Split(Path.DirectorySeparatorChar)
         : path.Remove(0, Globals.VanillaPath.Length + Path.DirectorySeparatorChar.ToString().Length).Split(Path.DirectorySeparatorChar)
         , isModPath);
   }

   public string GetPath()
   {
      return Path.Combine(IsModPath ? Globals.ModPath : Globals.VanillaPath, Path.Combine(_path));
   }

   public string GetInternalPath() => Path.Combine(_path);

   public string GetFolderPath()
   {
      return Path.Combine(IsModPath ? Globals.ModPath : Globals.VanillaPath, Path.Combine(_path[..^1]));
   }

   public string ToModPath()
   {
      return Path.Combine(Globals.ModPath, Path.Combine(_path));
   }

   public string GetFileName() => _path[^1];

   public static bool operator ==(PathObj a, PathObj b)
   {
      return a._path == b._path && a.IsModPath == b.IsModPath;
   }

   public static bool operator !=(PathObj a, PathObj b)
   {
      return a._path != b._path || a.IsModPath != b.IsModPath;
   }

   public override int GetHashCode()
   {
      return HashCode.Combine(IsModPath.GetHashCode(), StructuralComparisons.StructuralEqualityComparer.GetHashCode(_path));
   }

   public override bool Equals(object? obj)
   {
      if (obj is not PathObj other)
         return false;
      return IsModPath == other.IsModPath && _path.SequenceEqual(other._path);
   }

   public override string ToString()
   {
      return (IsModPath ? $"Mod: " : $"Vanilla: ") + Path.Combine(_path);
   }

}
