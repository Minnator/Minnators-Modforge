using System.Reflection;
using System.Text;
using Editor.DataClasses.Commands;
using Editor.Forms.Feature.Crash_Reporter;
using Editor.Saving;
using Newtonsoft.Json;

namespace Editor.Helper
{
   public static class CrashManager
   {
      // --------------------------------------------------------------------------------------------
      //                                  He has a crash on you
      // --------------------------------------------------------------------------------------------

      private static string LogFolder;
      public const string LOG_FILE_ENDING = "fuck";

      static CrashManager()
      {
         LogFolder = Path.Combine(Globals.AppDirectory, "crash_logs");
         if (!Directory.Exists(LogFolder))
            Directory.CreateDirectory(LogFolder);
      }

      public static void EnterCrashHandler(Exception e)
      {
         var window = new CrashReporter();
         window.ShowDialog();
         var sb = new StringBuilder();
         sb.AppendLine("Source:").AppendLine(e.Source);
         sb.AppendLine();
         sb.AppendLine("Message:").AppendLine(e.Message);
         sb.AppendLine();
         sb.AppendLine("StackTrace:").AppendLine(e.StackTrace);
         sb.AppendLine();
         sb.AppendLine("InnerException:").AppendLine(e.InnerException?.ToString());
         sb.AppendLine();
         sb.AppendLine("Data:").AppendLine(JsonConvert.SerializeObject(e.Data));
         sb.AppendLine();
         sb.AppendLine("TargetSite:").AppendLine(e.TargetSite?.ToString());
         sb.AppendLine();
         sb.AppendLine("HelpLink:").AppendLine(e.HelpLink);
         sb.AppendLine();
         sb.AppendLine("HResult:").AppendLine($"0x{e.HResult:X}");
         sb.AppendLine();
         sb.AppendLine("UserDescription:").AppendLine(window.Description);
         sb.AppendLine();
         sb.AppendLine("UserModLink:").AppendLine(window.ModLink);
         sb.AppendLine();
         sb.AppendLine("LastHistory:");
         try
         {
            if (HistoryManager.Current is { } activeNode)
            {

               var baseNode = HistoryManager.GetNodeAbove(activeNode, 10);
               foreach (var (node, indent) in baseNode)
               {
                  if (node.Command is null)
                     continue;
                  if (node.Id == activeNode.Id)
                  {
                     SavingUtil.AddTabs(indent, ref sb);
                     sb.AppendLine("Active:");
                  }

                  sb.AppendLine(node.Command.GetDebugInformation(indent));
               }
            }
            else
               sb.AppendLine("Tree is null :(");
         }
         catch (Exception exception)
         {
            sb.AppendLine("Failed to Generate");
            sb.AppendLine(exception.ToString());
         }
         var path = Path.Combine(LogFolder, $"crash_log_{DateTime.Now:yyyy_MM_dd_HH_mm_ss}.{LOG_FILE_ENDING}");
         IO.WriteToFile(path, sb.ToString(), false);
      }

      public static void ClearCrashLogs()
      {
         var files = Directory.GetFiles(LogFolder, $"*.{LOG_FILE_ENDING}");
         var result = MessageBox.Show($"Are you sure you want to delete all --[{files.Length}]-- crash logs?", "Delete Crash Logs", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);

         if (result != DialogResult.OK)
            return;

         foreach (var file in files) 
            File.Delete(file);
      }
   }
}