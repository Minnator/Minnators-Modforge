using System.Diagnostics;
using System.Text;
using Discord;
using Editor.Helper;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Activity = Discord.Activity;

namespace Editor.Forms.Crash_Reporter
{
   public partial class CrashReporter : Form
   {
      private const string BUG_REPORT_FORUM_LINK = "https://discord.com/channels/1288668689922527262/1296524536342118450";

      public CrashReporter()
      {
         InitializeComponent();
      }

      private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
      {
         LinkHelper.OpenDiscordLinkIfDiscordRunning(BUG_REPORT_FORUM_LINK);
      }

      private void SaveButton_Click(object sender, EventArgs e)
      {

      }

      private void CancelButton_Click(object sender, EventArgs e)
      {
         
      }

      

      private string ExportDataToJSON()
      {
         return "|";
      }
   }
}
