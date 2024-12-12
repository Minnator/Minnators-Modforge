using Editor.DiscordGame_SDK;
using Activity = Editor.DiscordGame_SDK.Activity;

namespace Editor.Helper
{
   public static class DiscordActivityManager
   {
      private static readonly Discord Discord = new(1303505754300485692, (UInt64)CreateFlags.Default);

      static DiscordActivityManager()
      {
         Discord.SetLogHook(LogLevel.Debug, (level, message) =>
         {
            System.Diagnostics.Debug.WriteLine("Log[{0}] {1}", level, message);
         });
      }


      public static void StartDiscordActivity()
      {
         var discordBackground = new Thread(() =>
         {
            var disposeDiscord = true;
            try
            {
               UpdateActivity();
               while (true)
               {
                  Discord.RunCallbacks();
                  Thread.Sleep(50);
                  if (!Globals.Settings.Misc.UseDiscordRichPresence)
                     return;
               }
            }
            catch (ResultException)
            {
               disposeDiscord = false; // Prevent disposing in finally
               MessageBox.Show("Discord Activity will no longer be available as the application has been closed.\nFor it to be available again, restart the Modforge.",
                  "Error loading Discord Activity", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
               if (disposeDiscord)
               {
                  Discord.Dispose();
               }
            }
         })
         {
            IsBackground = true
         };
         discordBackground.Start();
      }

      private static void UpdateActivity()
      {
         var activityManager = Discord.GetActivityManager();
         var activity = new Activity
         {
            State = $"Working on the \"{Globals.DescriptorData.Name}\" mod", 
            Details = $"Forging a mod in the EU4 Modforge",
            Assets =
            {
               LargeImage = "provincefilecreator1024x1024",
               LargeText = "Minnator's Modforge",
               SmallImage = "eu4logo",
               SmallText = "Europa Universalis 4"
            },
         };

         activityManager.UpdateActivity(activity, _ => { });
      }

      public static void ActivateActivity()
      {
         if (Globals.Settings.Misc.UseDiscordRichPresence)
            StartDiscordActivity();
      }
   }
}