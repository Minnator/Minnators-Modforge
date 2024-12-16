
using Editor.Saving;

namespace Editor.Helper
{
   public enum GameIcons
   {
      Yes,
      No,
   }


   public class GameIcon
   {
      public static Dictionary<GameIcons, GameIcon> Icons { get; set; } = new();
      
      public string[] IconPath { get; set; }
      public Bitmap Icon { get; set; }
      public GameIcons IconType { get; }

      public static void Initialize()
      {
         FromIconStrip(GameIcons.Yes, 0, 2, "gfx", "interface", "eligible_noneligible_strip.dds");
         FromIconStrip(GameIcons.No, 1, 2, "gfx", "interface", "eligible_noneligible_strip.dds");
      }

      public GameIcon(GameIcons iconEnum)
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
         throw new EvilActions($"Trying to access a non existing game icon! {icon}");
      }

      public static GameIcon FromPath(GameIcons iconEnum, params string[] path)
      {
         if (!VerifyInputs(path, iconEnum))
            return null!;
         return new (iconEnum)
         {
            Icon = ReadImage(Path.Combine(path)),
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

         GameIcon iconN = new (iconEnum)
         {
            Icon = icon,
            IconPath = path
         };

      }
   }
}