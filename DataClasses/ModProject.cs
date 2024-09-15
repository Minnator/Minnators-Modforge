using System.Drawing;
using System.IO;
using Editor.Helper;

namespace Editor.DataClasses;

public class ModProject
{
   public ColorProviderRgb ColorProvider = new ();

   // Mod data
   public string Name { get; set; } = null!;
   public string ModPath { get; set; } = null!;
   public string VanillaPath { get; set; } = null!;
   public Language Language { get; set; } = Language.English;

   public Size MapSize { get; set; }

   public static string GetMapPath(ModProject project, string filename) => 
      Path.Combine(project.ModPath, "map", filename);

   public static string GetLocalisationsFile(ModProject project, string filename) =>
      Path.Combine(project.ModPath, "name", filename);


}

public enum Language
{
   English,
   French,
   German,
   Spanish,
}