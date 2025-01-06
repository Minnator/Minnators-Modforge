using System.Diagnostics;
using System.Text.RegularExpressions;
using Editor.DataClasses.Commands;
using Editor.DataClasses.Misc;
using Editor.Helper;
using Editor.Saving;

namespace Editor.Controls
{
   public enum InputType
   {
      Text,
      UnsignedNumber,
      SignedNumber,
      DecimalNumber
   }
   public partial class TextSaveableTextBox<T, C> : TextBox where C : SaveableCommand
   {
      private readonly Regex _unsignedNumberRegex = NumberRegex();
      private readonly Regex _signedNumberRegex = SignedNumberRegex();
      private readonly Regex _decimalNumberRegex = DecimalNumberRegex();

      [GeneratedRegex(@"^[-+]?(?:0|[1-9]\d*)?\.?\d*$", RegexOptions.Compiled)]
      private static partial Regex DecimalNumberRegex();
      [GeneratedRegex(@"^(?:0|[1-9]\d*)$", RegexOptions.Compiled)]
      private static partial Regex NumberRegex();

      [GeneratedRegex(@"^[-+]?(?:0|[1-9]\d*)$", RegexOptions.Compiled)]
      private static partial Regex SignedNumberRegex();

      

      private string _oldText = string.Empty;
      private string _newText = string.Empty;
      private Func<ICollection<Saveable>> _getSaveables;
      private readonly CTextEditingFactory<C, T> _factory;

      private bool _silentSet = false;

      public InputType Input { get; set; } = InputType.Text;

      public TextSaveableTextBox(Func<ICollection<Saveable>> getSaveables, CTextEditingFactory<C, T> factory)
      {

         _getSaveables = getSaveables;
         _factory = factory;
         LostFocus += OnFocusLost;
         KeyDown += OnKeyDown;
         Enter += OnFocusGained;
         TextChanged += OnTextChanged;
         KeyPress += OnKeyPress;
      }

      private void SaveableChanged(object o, string name)
      {
         if (_silentSet)
            return;
         Debug.Assert(o is Saveable, "o is not Saveable");
         if (_factory is CCountryPropertyChangeFactory<T> tempFactory) // TODO Remove after new province implementation
            if (tempFactory.PropName.Equals(name) || _getSaveables.Invoke().Contains(o))
            {
               var value = ((Saveable)o).GetProperty<T>(tempFactory.PropName)?.ToString();
               if (string.Equals(Text, value))
               {
                  Text = string.Empty;
               }
            }
      }

      private void OnKeyPress(object? sender, KeyPressEventArgs e)
      {
         var keyChar = e.KeyChar;

         if (char.IsControl(e.KeyChar))
            return;

         if (!CheckText(Text + keyChar))
         {
            e.Handled = true;
         }
      }

      private void OnTextChanged(object? sender, EventArgs e)
      {
         if (_oldText.Equals(Text) || Globals.State == State.Loading)
            return;

         _newText = Text;
      }

      private void OnFocusGained(object? sender, EventArgs e)
      {
         _oldText = Text;
         _newText = Text;
      }


      private bool CheckText(string text)
      {
         if (text.Length == 0)
            return true;
         return Input switch
         {
            InputType.Text => true,
            InputType.SignedNumber => _signedNumberRegex.IsMatch(text),
            InputType.UnsignedNumber => _unsignedNumberRegex.IsMatch(text),
            InputType.DecimalNumber => _decimalNumberRegex.IsMatch(text),
            _ => throw new ArgumentOutOfRangeException()
         };
      }

      private void OnKeyDown(object? sender, KeyEventArgs e)
      {
         var keyCode = e.KeyCode;

         if (keyCode == Keys.Enter)
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
         else if (keyCode == Keys.Escape)
         {
            Text = _oldText;
            Globals.ZoomControl.Focus();
         }
         
      }


      private bool ExecuteCommand()
      {
         if (_newText.Length == 0 || _oldText.Equals(_newText) || !CheckText(Text) || !Converter.Convert<T>(_newText, out T value).Log())
            return false;

         if (_factory is not CCountryPropertyChangeFactory<T> ccfactory)
         {
            ICommand command = _factory.Create(_getSaveables.Invoke(), value);
            _silentSet = true;
            command.Execute();
            _silentSet = false;
            HistoryManager.AddCommand(command);
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