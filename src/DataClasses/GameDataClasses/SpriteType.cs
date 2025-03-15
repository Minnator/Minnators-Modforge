using Editor.Helper;

namespace Editor.DataClasses.GameDataClasses
{
   public class SpriteType(string[] internalPath, string name, bool isMod)
   {
      public string[] InternalPath { get; set; } = internalPath;
      public string Name { get; } = name;
      public bool IsMod { get; set; } = isMod;

      public Bitmap Icon
      {
         get
         {
            var path = Path.Combine(InternalPath);
            path = Path.Combine(IsMod ? Globals.ModPath : Globals.VanillaPath, path);
            if (InternalPath.Length == 0 || !File.Exists(path))
               return GameIconDefinition.GetIcon(GameIcons.MissionPlaceHolder);

            return GameIconDefinition.ReadImage(path);
         }
      }

      public override string ToString() => Name;
      public override int GetHashCode() => Name.GetHashCode();
      public override bool Equals(object? obj) => obj is SpriteType spriteType && spriteType.Name == Name;
   }

   //TODO Used to group sprite types by their internal path to make it easier to filter them and save memory by not having to save the internal path for each sprite type
   public class SpriteTypeGroup(string[] internalPath)
   {
      public string[] InternalPath { get; set; } = internalPath;
      public List<SpriteType> SpriteTypes { get; set; } = [];
   }
}