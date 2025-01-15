
using Editor.ErrorHandling;
using Editor.Saving;

namespace Editor.Helper
{

   [AttributeUsage(AttributeTargets.Property)]
   public class GameIcon(GameIcons icon, bool showNumber = false) : Attribute
   {
      public GameIcons Icon { get; set; } = icon;
      public bool ShowNumber {get; set; } = showNumber;
   }

   public enum GameIcons
   {
      None = -1,
      Yes,
      No,
      Building,
      Core,
      Claim,
      DiscoverAchievement,
   }


   public class GameIconDefinition
   {
      public static Dictionary<GameIcons, GameIconDefinition> Icons { get; set; } = new();
      
      public string[] IconPath { get; set; }
      public Bitmap Icon { get; set; }
      public GameIcons IconType { get; }

      public static void Initialize()
      {
         FromIconStrip(GameIcons.Yes, 0, 2, "gfx", "interface", "eligible_noneligible_strip.dds");
         FromIconStrip(GameIcons.No, 1, 2, "gfx", "interface", "eligible_noneligible_strip.dds");
         FromPath(GameIcons.None, "gfx", "interface", "default_fallback_texture.dds");
         FromPath(GameIcons.Building, "gfx", "interface", "technologyView_show_buildings.dds");
         FromPath(GameIcons.Core, "gfx", "interface", "core_icon.dds");
         FromPath(GameIcons.Claim, "gfx", "interface", "ideas_EU4", "fabricate_claims_cost.dds");
         FromPath(GameIcons.DiscoverAchievement, "gfx", "interface", "achievements", "achievement_world_discoverer.dds");
      }

      public GameIconDefinition(GameIcons iconEnum)
      {
         IconPath = [];
         IconType = iconEnum;
         Icon = new (1, 1);

         if (!Icons.TryAdd(iconEnum, this))
            Globals.ErrorLog.Write($"Error: Icon {iconEnum} already exists in Icon Dictionary!");
      }

      public static Bitmap GetIcon(GameIcons iconEnum)
      {
         if (Icons.TryGetValue(iconEnum, out var icon))
            return icon.Icon;
         _ = new ErrorObject(ErrorType.INTERNAL_KeyNotFound, $"Trying to access a non existing game icon! {icon}", LogType.Critical);
         if (Icons.TryGetValue(GameIcons.None, out icon))
            return icon.Icon;
         throw new EvilActions($"Trying to access a non existing game icon! {icon}. FALLBACK icon not defined");
      }

      public static GameIconDefinition FromPath(GameIcons iconEnum, params string[] path)
      {
         if (!VerifyInputs(path, iconEnum))
            return null!;
         return new (iconEnum)
         {
            Icon = ReadImage(Path.Combine(Globals.VanillaPath, Path.Combine(path))),
            IconPath = path
         };
      }

      private static Bitmap ReadImage(string path) => ImageReader.ReadImage(path);

      private static bool VerifyInputs(string[] path, GameIcons iconEnum)
      {
         if (path.Length == 0)
         {
            Globals.ErrorLog.Write($"Error: Empty Icon Path {path}");
            return false;
         }
         if (Icons.TryGetValue(iconEnum, out _))
            return false;
         if (!path[^1].EndsWith(".dds") && !path[^1].EndsWith(".tga"))
         {
            Globals.ErrorLog.Write($"Error: Illegal Icon type {path}");
            return false;
         }
         return true;
      }

      public static void FromIconStrip(GameIcons iconEnum, int index, int framesCount, params string[] path)
      {
         if (!VerifyInputs(path, iconEnum))
            return;
         FilesHelper.GetVanillaPath(out var vPath, path);

         var rawImage = ReadImage(vPath);

         var icon = new Bitmap(rawImage.Width / framesCount, rawImage.Height);
         using var graphics = Graphics.FromImage(icon);
         graphics.DrawImage(rawImage, new Rectangle(0, 0, icon.Width, icon.Height), new (index * icon.Width, 0, icon.Width, icon.Height), GraphicsUnit.Pixel);

         GameIconDefinition _ = new (iconEnum)
         {
            Icon = icon,
            IconPath = path
         };

      }
   }
}