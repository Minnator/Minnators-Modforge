using System.Windows.Forms;

namespace Editor.Forms
{
   public partial class ConsoleForm : Form
   {
      public ConsoleForm()
      {
         InitializeComponent();
      }

      private void InputTextBox_KeyUp(object sender, KeyEventArgs e)
      {
         switch (e.KeyCode)
         {
            case Keys.Enter:
               // Execute the command
               Output.Text += InputTextBox.Text + "\n";
               break;
            case Keys.Up:
               // Get the previous command
               // if in autocomplete mode, get the previous suggestion
               break;
            case Keys.Down:
               // Get the next command
               // if in autocomplete mode, get the next suggestion
               break;
            case Keys.Tab: 
               //show auto complete options for the current context
               break;
         }
      }
   }
}
