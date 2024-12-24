using System.Media;
using Editor.Helper;

namespace Editor.Forms.Feature.Crash_Reporter
{
   public partial class CrashReporter : Form
   {
      private const string BUG_REPORT_FORUM_LINK = "https://discord.com/channels/1288668689922527262/1296524536342118450";

      public string Description = string.Empty;
      public string ModLink = string.Empty;

      public CrashReporter()
      {
         InitializeComponent();

         if (Globals.Settings.Saving.PlayCrashSound)
         {
            using SoundPlayer player = new(Properties.Resources.CrashSound);
            player.Play();
         }
      }

      private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
      {
         ProcessHelper.OpenDiscordLinkIfDiscordRunning(BUG_REPORT_FORUM_LINK);
      }

      private void SaveButton_Click(object sender, EventArgs e)
      {
         Description = DescriptionBox.Text;
         ModLink = ModLinkBox.Text;
         Close();
      }

      private void CancelButton_Click(object sender, EventArgs e)
      {
         Description = DescriptionBox.Text;
         ModLink = ModLinkBox.Text;
         Close();
      }

   }
}
