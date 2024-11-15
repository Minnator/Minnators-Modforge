using System.Text.Json;
using Editor.Forms.Feature.Crash_Reporter;

namespace Editor.Helper
{
   public static class CrashManager
   {
      // --------------------------------------------------------------------------------------------
      //                                  He has a crash on you
      // --------------------------------------------------------------------------------------------

      public static void EnterCrashHandler(Exception e)
      {
         var window = new CrashReporter();
         // var result = window.ShowDialog();
         var executablePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
         if (Globals.HistoryManager is not null && Globals.HistoryManager.GetRoot() is { } root)
         {
            var tree = JsonSerializer.Serialize(root, options: new () { WriteIndented = true });
            IO.WriteToFile(Path.Combine(executablePath, $"crash_log_{DateTime.Now}.json"),tree,false);
         }
      }
   }
}