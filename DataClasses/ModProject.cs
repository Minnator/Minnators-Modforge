using System.Drawing;
using System.IO;

namespace Editor.DataClasses;

public class ModProject
{
   public string Name { get; set; } = null!;
   public string ModPath { get; set; } = null!;
   public string VanillaPath { get; set; } = null!;
   public Language Language { get; set; } = Language.english;

   public Size MapSize { get; set; } = new (5632, 2048);

   public static string GetMapPath(ModProject project, string filename) => 
      Path.Combine(project.ModPath, "map", filename);

   public static string GetLocalisationsFile(ModProject project, string filename) =>
      Path.Combine(project.ModPath, "localisation", filename);


}

public enum Language
{
   english,
   french,
   german,
   spanish,
}