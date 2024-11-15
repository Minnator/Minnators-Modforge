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
            try
            {
               UpdateActivity();
               while (true)
               {
                  Discord.RunCallbacks();
                  Thread.Sleep(50);
               }
            }
            finally
            {
               Discord.Dispose();
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
   }
}