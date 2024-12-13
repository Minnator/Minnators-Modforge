namespace Editor.Controls
{
   public class SmartTextBox : TextBox
   {
      private string _oldText = string.Empty;
      public event EventHandler<string>? ContentModified;

      public string OldText => _oldText;

      public SmartTextBox()
      {
         LostFocus += OnFocusLost;
         KeyDown += OnKeyDown;
         Enter += OnFocusGained;
         TextChanged += OnTextChanged;
      }

      private void OnKeyDown(object? sender, KeyEventArgs e)
      {
         if (e.KeyCode == Keys.Enter)
         {
            e.SuppressKeyPress = true;
            OnContentModified();
         }
         else if (e.KeyCode == Keys.Escape)
         {
            Text = _oldText;
         }
      }

      private void OnFocusGained(object? sender, EventArgs e)
      {
         _oldText = Text;
      }

      private void OnTextChanged(object? sender, EventArgs e)
      {

      }

      private void OnFocusLost(object? sender, EventArgs e)
      {
         OnContentModified();
      }

      protected virtual void OnContentModified()
      {
         if (_oldText.Equals(Text) || string.IsNullOrWhiteSpace(Text))
            return;
         _oldText = Text;
         ContentModified?.Invoke(this, Text);
      }

      public new void Clear()
      {
         base.Text = string.Empty;
      }
   }
}