using System.Windows.Forms;
using Editor.DataClasses.ConsoleCommands;
using static System.Windows.Forms.Design.AxImporter;

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
               ConsoleCommandParser.ParseCommand(Input.Text);
               Input.Text = string.Empty;
               break;
            case Keys.Up:
               Input.Text = ConsoleCommandParser.GetPreviousCommand();
               break;
            case Keys.Down:
               Input.Text = ConsoleCommandParser.GetNextCommand();
               break;
         }

      }

      private void ConsoleForm_Load(object sender, EventArgs e)
      {
         Input.AutoCompleteCustomSource = [];
         foreach (var cmd in ConsoleCommandParser.CommandNames)
            Input.AutoCompleteCustomSource.Add(cmd);
         Input.AutoCompleteSource = AutoCompleteSource.CustomSource;
         Input.AutoCompleteMode = AutoCompleteMode.Suggest;
         Input.Focus();
      }

      private void Output_TextChanged(object sender, EventArgs e)
      {
         Output.SelectionStart = Output.Text.Length;
         Output.ScrollToCaret();
      }

   }
}
