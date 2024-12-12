using Editor.DataClasses.Commands;
using Editor.Saving;

namespace Editor.Controls
{
   public class TextSaveableTextBox<T> : TextBox where T : CTextEditingWrapper
   {
      private T? _command;
      private string _oldText = string.Empty;
      private Func<ICollection<Saveable>> _getSaveables;
      private readonly CTextEditingFactory<T> _factory;

      public TextSaveableTextBox(Func<ICollection<Saveable>> getSaveables, CTextEditingFactory<T> factory)
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
         _command ??= _factory.Create(_getSaveables.Invoke());
         _command.SetValue(Text);
      }

      private void OnFocusGained(object? sender, EventArgs e)
      {
         _oldText = Text;
         _command = null;
      }

      private void OnKeyDown(object? sender, KeyEventArgs e)
      {
         if (e.KeyCode == Keys.Enter)
         {
            e.SuppressKeyPress = true;
            _command?.Execute();
            Globals.ZoomControl.Focus();
         }
         else if (e.KeyCode == Keys.Escape)
         {
            Text = _oldText;
            _command = null;
            Globals.ZoomControl.Focus();
         }
      }


      private void ExecuteCommand()
      {
         if (_command is null || _oldText.Equals(Text))
            return;
         _command.Execute();
         Globals.HistoryManager.AddCommand(_command);
         _command = null;
      }

      private void OnFocusLost(object? sender, EventArgs e)
      {
         ExecuteCommand();
      }

      public new void Clear()
      {
         ExecuteCommand();
         base.Clear();
      }

   }
}