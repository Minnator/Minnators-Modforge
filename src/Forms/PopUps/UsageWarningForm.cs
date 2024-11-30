namespace Editor.Forms.PopUps
{
   public partial class UsageWarningForm : Form
   {
      public UsageWarningForm()
      {
         InitializeComponent();

         RichTextBox.Rtf = @"{\rtf1\ansi
{\colortbl ;\red0\green0\blue0;\red255\green10\blue10;}
{\b\fs36 {\cf2 Caution}: This Software is in Early Alpha}\line\line
This program is currently in an {\b experimental phase} and may cause unintended issues, including {\b {\cf2 corrupting} or {\cf2 deleting} files}. We highly recommend taking precautions to protect your data before proceeding.\line\line
{\b Please back up your mod files} to prevent loss of work. You can back up your files in one of the following ways:\line\line
    1. Copy your mod folder to a safe location.\line
    2. Use version control software like {\field{\*\fldinst HYPERLINK """"https://git-scm.com/""""}{\fldrslt Git}} for a more robust and secure solution.\line
}";

         RichTextBox.LinkClicked += RichTextBox_LinkClicked;
         RichTextBox.GotFocus += (_, _) => this.ActiveControl = null;
      }

      private void CancelButton_Click(object sender, EventArgs e)
      {
         DialogResult = DialogResult.Cancel;
         Close();
      }

      private void AgreeButton_Click(object sender, EventArgs e)
      {
         DialogResult = DialogResult.OK;
         Close();
      }

      private void RichTextBox_LinkClicked(object? sender, LinkClickedEventArgs e)
      {
         // Open the link in the default browser
         System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
         {
            FileName = e.LinkText,
            UseShellExecute = true
         });
      }
   }
}
