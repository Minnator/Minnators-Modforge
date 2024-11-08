using Editor.Helper;

namespace Editor.Forms
{
   public partial class InformationForm : Form
   {
      public InformationForm()
      {
         InitializeComponent();
         textBox1.Text = Globals.GITHUB_LINK;
         textBox2.Text = Globals.DISCORD_INVITATION_LINK;
      }

      private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
      {

      }

      private void OpenGitButton_Click(object sender, EventArgs e)
      {
         LinkHelper.OpenLink(Globals.GITHUB_LINK);
      }

      private void OpenDiscordButton_Click(object sender, EventArgs e)
      {
         LinkHelper.OpenLink(Globals.DISCORD_INVITATION_LINK);
      }
   }
}
