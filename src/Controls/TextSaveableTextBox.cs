using Editor.DataClasses.Commands;
using Editor.Helper;
using Editor.Saving;

namespace Editor.Controls
{
   public class TextSaveableTextBox<T, C> : TextBox where C : SaveableCommand
   {
      private string _oldText = string.Empty;
      private string _newText = string.Empty;
      private Func<ICollection<Saveable>> _getSaveables;
      private readonly CTextEditingFactory<C, T> _factory;


      public bool DigitOnly { get; set; }

      public TextSaveableTextBox(Func<ICollection<Saveable>> getSaveables, CTextEditingFactory<C, T> factory)
      {
         _getSaveables = getSaveables;
         _factory = factory;
         LostFocus += OnFocusLost;
         KeyDown += OnKeyDown;
         Enter += OnFocusGained;
         TextChanged += OnTextChanged;
      }

      private void OnTextChanged(object? sender, EventArgs e)
      {
         if (_oldText.Equals(Text) || Globals.EditingStatus == EditingStatus.LoadingInterface)
            return;
         _newText = Text;
      }

      private void OnFocusGained(object? sender, EventArgs e)
      {
         _oldText = Text;
         _newText = Text;
      }

      private void OnKeyDown(object? sender, KeyEventArgs e)
      {
         if (e.KeyCode == Keys.Enter)
         {
            e.SuppressKeyPress = true;
            if (!ExecuteCommand())
            {
               e.Handled = true;
               Text = _oldText;
               _newText = _oldText;
            }
            Globals.ZoomControl.Focus();
         }
         else if (e.KeyCode == Keys.Escape)
         {
            Text = _oldText;
            Globals.ZoomControl.Focus();
         }
         else if (DigitOnly)
         {
            if (!char.IsDigit((char)e.KeyCode))
               switch (e.KeyCode) // WHY TF IS THERE NO BETTER WAY
               {
                  case Keys.Back:
                  case Keys.Delete:
                  case Keys.Left:
                  case Keys.Right:
                  case Keys.NumPad0:
                  case Keys.NumPad1:
                  case Keys.NumPad2:
                  case Keys.NumPad3:
                  case Keys.NumPad4:
                  case Keys.NumPad5:
                  case Keys.NumPad6:
                  case Keys.NumPad7:
                  case Keys.NumPad8:
                  case Keys.NumPad9:
                     break;
                  default:
                     e.SuppressKeyPress = true;
                     return;
               }
            
            // Do your numeric stuff here
            // I am doing nothing.

         }
      }


      private bool ExecuteCommand()
      {
         if (_oldText.Equals(_newText))
            return false;

         if (!Converter.Convert<T>(_newText, out var value))
            return false;
         if (_factory is not CCountryPropertyChangeFactory<T> ccfactory)
         {
            ICommand command = _factory.Create(_getSaveables.Invoke(), value);
            command.Execute();
            Globals.HistoryManager.AddCommand(command);
         }
         else
         {
            var newObject = _getSaveables.Invoke().First();
            newObject.SetProperty(ccfactory.PropName, value);
         }
         
         _oldText = _newText;
         return true;
      }

      private void OnFocusLost(object? sender, EventArgs e)
      {
         if (ExecuteCommand() && Text.Equals(_newText)) 
            Text = _oldText;
      }

      public new void Clear()
      {
         ExecuteCommand();
         base.Clear();
      }

   }
}