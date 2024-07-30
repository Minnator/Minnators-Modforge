using System.Reflection;

namespace Editor.Controls
{
   public sealed class ExtendedNumeric : NumericUpDown
   {
      // Event to handle the up button press
      public event EventHandler<ProvinceEditedEventArgs> UpButtonPressed = delegate { };
      public event EventHandler<ProvinceEditedEventArgs> DownButtonPressed = delegate { };
      public event EventHandler<ProvinceEditedEventArgs> OnTextValueChanged = delegate { };

      private TextBox _textBox;

      public ExtendedNumeric()
      {
         Dock = DockStyle.Fill;
         Height = 21;

         // Access the internal TextBox control using reflection
         var type = GetType().BaseType!;
         var textBoxProperty = type.GetProperty("TextBox", BindingFlags.Instance | BindingFlags.NonPublic);
         _textBox = (TextBox)textBoxProperty?.GetValue(this, null)!;
         _textBox.KeyPress += TextBox_KeyPress;
      }

      private void TextBox_KeyPress(object? sender, KeyPressEventArgs e)
      {
         if (!char.IsDigit(e.KeyChar) && e.KeyChar != '\b' && e.KeyChar != (char)Keys.Enter) 
            e.Handled = true;

         // Raise the OnTextValueChanged event when the Enter key is pressed == the value has been changed
         if (e.KeyChar == (char)Keys.Enter)
            OnTextValueChanged?.Invoke(this, new (Globals.Selection.GetSelectedProvinces, Value));
      }

      public override void UpButton()
      {
         base.UpButton();
         RaiseUpButtonEvent(this, new (Globals.Selection.GetSelectedProvinces, Value));
      }

      public override void DownButton()
      {
         base.DownButton();
         RaiseDownButtonEvent(this, new (Globals.Selection.GetSelectedProvinces, Value));
      }

      private void RaiseDownButtonEvent(object? sender, ProvinceEditedEventArgs e)
      {
         DownButtonPressed?.Invoke(sender, e);
      }

      private void RaiseUpButtonEvent(object sender, ProvinceEditedEventArgs e)
      {
         UpButtonPressed?.Invoke(sender, e);
      }
   }
}