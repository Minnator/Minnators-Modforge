namespace Editor.src.Forms.PopUps
{
   public sealed partial class ImprovedMessageBox : Form
   {
      public ImprovedMessageBox(string message, string title, MessageBoxButtons buttons, MessageBoxIcon icon)
      {
         StartPosition = FormStartPosition.CenterParent;
         InitializeComponent();

         DescLabel.Text = message;
         Text = title;
         IconBox.Image = GetIcon(icon);

         ArrangeButtons(buttons);
      }

      public static DialogResult Show(string text, string caption, ref bool doShowAgain, MessageBoxButtons buttons = MessageBoxButtons.OK,
                                    MessageBoxIcon icon = MessageBoxIcon.None)
      {
         if (!doShowAgain)
         {
            // Return OK without showing if the user has chosen "Do not show again"
            return DialogResult.OK;
         }

         using (var messageBox = new ImprovedMessageBox(text, caption, buttons, icon))
         {
            var result = messageBox.ShowDialog();
            doShowAgain = !messageBox.DoShowAgainCheckBox.Checked;
            return result;
         }
      }

      private Image GetIcon(MessageBoxIcon icon)
      {
         switch (icon)
         {
            case MessageBoxIcon.Information:
               return SystemIcons.Information.ToBitmap();
            case MessageBoxIcon.Warning:
               return SystemIcons.Warning.ToBitmap();
            case MessageBoxIcon.Error:
               return SystemIcons.Error.ToBitmap();
            case MessageBoxIcon.Question:
               return SystemIcons.Question.ToBitmap();
            default:
               return null;
         }
      }

      private void AddButton(DialogResult result, Button button)
      {
         button.Text = result.ToString();
         button.DialogResult = result;
         button.Visible = true;
      }

      private void ArrangeButtons(MessageBoxButtons buttons)
      {
         switch (buttons)
         {

            case MessageBoxButtons.OK:
               AddButton(DialogResult.OK, TertiaryButton);
               break;
            case MessageBoxButtons.OKCancel:
               AddButton(DialogResult.Cancel, PrimaryButton);
               AddButton(DialogResult.OK, TertiaryButton);
               break;
            case MessageBoxButtons.AbortRetryIgnore:
               AddButton(DialogResult.Abort, PrimaryButton);
               AddButton(DialogResult.Ignore, SecondaryButton);
               AddButton(DialogResult.Retry, TertiaryButton);
               break;
            case MessageBoxButtons.YesNoCancel:
               AddButton(DialogResult.Cancel, PrimaryButton);
               AddButton(DialogResult.No, SecondaryButton);
               AddButton(DialogResult.Yes, TertiaryButton);
               break;
            case MessageBoxButtons.YesNo:
               AddButton(DialogResult.No, PrimaryButton);
               AddButton(DialogResult.Yes, TertiaryButton);
               break;
            case MessageBoxButtons.RetryCancel:
               AddButton(DialogResult.Cancel, PrimaryButton);
               AddButton(DialogResult.Retry, TertiaryButton);
               break;
            case MessageBoxButtons.CancelTryContinue:
               AddButton(DialogResult.Cancel, PrimaryButton);
               AddButton(DialogResult.Retry, SecondaryButton);
               AddButton(DialogResult.Continue, TertiaryButton);
               break;
            default:
               throw new NotImplementedException($"MessageBoxButtons.{buttons} is not implemented.");
         }
      }
   }
}
