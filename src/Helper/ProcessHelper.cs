using System.Diagnostics;
using Editor.ErrorHandling;
using Editor.Saving;

namespace Editor.Helper
{
   public static class ProcessHelper
   {

      public static bool OpenDiscordLinkIfDiscordRunning(string link)
      {
         try
         {
            Process.Start(new ProcessStartInfo
            {
               UseShellExecute = true,
               FileName = IsDiscordRunning ? "discord:" + link : link
            });
            return true;
         }
         catch (Exception ex)
         {
            MessageBox.Show($"Failed to open the browser: {ex.Message}{Environment.NewLine}Please open the url yourself {link}");
            return false;
         }
      }

      public static void OpenLink(string link)
      {
         try
         {
            Process.Start(new ProcessStartInfo
            {
               UseShellExecute = true,
               FileName = link
            });
         }
         catch (Exception ex)
         {
            MessageBox.Show($"Failed to open the browser: {ex.Message}{Environment.NewLine}Please open the url yourself {link}");
         }
      }

      public static bool IsDiscordRunning => Process.GetProcessesByName("Discord").Length > 0;

      public static void Open(string app, string args, bool newWindow)
      {
         using var myProcess = new Process();
         myProcess.StartInfo.UseShellExecute = true;
         myProcess.StartInfo.FileName = app;
         myProcess.StartInfo.Arguments = args;
         myProcess.StartInfo.CreateNoWindow = newWindow;
         myProcess.Start();
      }

      public static void OpenVsCode(string filePath, bool newWindow)
      {
         Open("code", filePath, newWindow);
      }

      public static void OpenSaveableFolders(ICollection<Saveable> saveables)
      {
         HashSet<string> uniquePaths = [];
         foreach (var prov in saveables)
            uniquePaths.Add(prov.Path.GetFolderPath());

         OpenFolders(uniquePaths);
      }

      public static void OpenFolder(Saveable saveable)
      {
         OpenFolder(saveable.Path.GetFolderPath());
      }

      private static void OpenFolders(ICollection<string> paths)
      {
         foreach (var path in paths)
            OpenFolder(path);
      }

      public static bool OpenFolder(string path)
      {
         if (Directory.Exists(path))
         {
            Process.Start(new ProcessStartInfo
            {
               FileName = path,
               UseShellExecute = true
            });
            return true;
         }

         MessageBox.Show($"The path {path} can not be opened", "Folder can not be opened", MessageBoxButtons.OK,
            MessageBoxIcon.Warning);
         _ = new DebugError($"Could not open \"{path}\"", ErrorType.ExplorerCouldNotOpenFolder);
         return false;
      }

      public static bool OpenFile(string path)
      {
         if (File.Exists(path))
         {
            // TODO open file at line number
            Process.Start(new ProcessStartInfo
            {
               FileName = path,
               UseShellExecute = true,
            });
            return true;
         }

         MessageBox.Show($"The path {path} can not be opened", "File can not be opened", MessageBoxButtons.OK,
            MessageBoxIcon.Warning);
         _ = new DebugError($"Could not open \"{path}\"", ErrorType.ApplicationCouldNotOpenFile);
         return false;
      }

      public static void OpenSaveableFiles(ICollection<Saveable> saveables)
      {
         foreach (var prov in saveables)
            OpenFile(prov);
      }

      public static void OpenFile(Saveable saveable)
      {
         OpenFile(saveable.Path.GetPath());
      }

      public static bool OpenPathIfFileOrFolder(string path)
      {
         if (OpenFile(path))
            return true;
         if (OpenFolder(path))
            return true;
         _ = new DebugError($"Could not open \"{path}\"", ErrorType.ApplicationCouldNotOpenFile);
         return false;
      }
   }
}