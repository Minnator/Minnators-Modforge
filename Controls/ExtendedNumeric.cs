using System.Reflection;

namespace Editor.Controls
{
   public sealed class ExtendedNumeric : NumericUpDown
   {
      // Event to handle the up button press
      public event EventHandler<ProvinceEditedEventArgs> UpButtonPressedSmall = delegate { };
      public event EventHandler<ProvinceEditedEventArgs> DownButtonPressedSmall = delegate { };
      public event EventHandler<ProvinceEditedEventArgs> UpButtonPressedMedium = delegate { };
      public event EventHandler<ProvinceEditedEventArgs> DownButtonPressedMedium = delegate { };
      public event EventHandler<ProvinceEditedEventArgs> UpButtonPressedLarge = delegate { };
      public event EventHandler<ProvinceEditedEventArgs> DownButtonPressedLarge = delegate { };
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
         if ((ModifierKeys & Keys.ControlKey) == Keys.ControlKey)
            DownButtonPressedLarge.Invoke(this, new (Globals.Selection.GetSelectedProvinces, Value));
         else if ((ModifierKeys & Keys.Shift) == Keys.Shift)
            DownButtonPressedMedium.Invoke(this, new (Globals.Selection.GetSelectedProvinces, Value));
         else 
            UpButtonPressedSmall.Invoke(this, new (Globals.Selection.GetSelectedProvinces, Value));
      }

      public override void DownButton()
      {
         base.DownButton();
         if ((ModifierKeys & Keys.ControlKey) == Keys.ControlKey)
            DownButtonPressedLarge.Invoke(this, new (Globals.Selection.GetSelectedProvinces, Value));
         else if ((ModifierKeys & Keys.Shift) == Keys.Shift)
            DownButtonPressedMedium.Invoke(this, new (Globals.Selection.GetSelectedProvinces, Value));
         else 
            DownButtonPressedSmall.Invoke(this, new (Globals.Selection.GetSelectedProvinces, Value));
      }
   }
}