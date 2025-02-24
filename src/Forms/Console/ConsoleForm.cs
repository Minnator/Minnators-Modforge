using Editor.DataClasses.ConsoleCommands;

namespace Editor.src.Forms.Console
{
   public partial class ConsoleForm : Form
   {
      public readonly CommandHandler CommandHandler;

      public ConsoleForm()
      {
         InitializeComponent();
         CommandHandler = new (OutputBox);

         OutputBox.TabIndex = 2;
         //keydown on input TextBox
         InputBox.Text = "> ";
         InputBox.SelectionLength = 0;
         InputBox.SelectionStart = InputBox.Text.Length;
         InputBox.TabIndex = 0;

         InputBox.KeyDown += ConsoleTest_KeyDown;
         InputBox.TextChanged += InputBox_TextChanged;
         OutputBox.SelectionStart = OutputBox.Text.Length;
         
         InputBox.Focus();
      }


      private void ConsoleTest_KeyDown(object? sender, KeyEventArgs e)
      {
         if (e.KeyCode == Keys.Enter)
         {
            CommandHandler.ExecuteCommand(InputBox.Text);
            InputBox.Text = "";
            e.SuppressKeyPress = true;
            e.Handled = true;
         }
         else if (e.KeyCode == Keys.Back)
         {
            if (InputBox.Text.Length == 2) // we dont allow the user to delete the > char
            {
               e.SuppressKeyPress = true;
               e.Handled = true;
            }
         }
         else if (e.KeyCode == Keys.Up)
         {
            if (CommandHandler.HistoryIndex > 0)
               InputBox.Text = "> " + CommandHandler.History[--CommandHandler.HistoryIndex];
            e.SuppressKeyPress = true;
            e.Handled = true;
            InputBox.SelectionStart = InputBox.Text.Length;
         }
         else if (e.KeyCode == Keys.Down)
         {
            if (CommandHandler.HistoryIndex < CommandHandler.History.Count - 1)
               InputBox.Text = "> " + CommandHandler.History[++CommandHandler.HistoryIndex];
            else
               InputBox.Text = "> ";
            e.SuppressKeyPress = true;
            e.Handled = true;
            InputBox.SelectionStart = InputBox.Text.Length;
         }
      }

      private void InputBox_TextChanged(object? sender, EventArgs e)
      {
         if (InputBox.Text.Length < 2 || InputBox.Text.Length == 2 && InputBox.Text[0] != '>' || InputBox.Text.Length < 2)
         {
            InputBox.Text = "> ";
            InputBox.SelectionLength = 0;
            InputBox.SelectionStart = InputBox.Text.Length;
         }

         if (!InputBox.Text.StartsWith("> "))
         {
            InputBox.Text = "> " + InputBox.Text;
            InputBox.SelectionLength = 0;
            InputBox.SelectionStart = InputBox.Text.Length;
         }
      }
   }
}
