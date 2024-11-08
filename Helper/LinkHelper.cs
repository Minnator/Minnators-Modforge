using System.Diagnostics;

namespace Editor.Helper
{
   public static class LinkHelper
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
   }
}