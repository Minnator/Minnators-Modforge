using System.Diagnostics;
using System.Reflection;
using Editor.Events;
using Editor.Helper;

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
      public new event EventHandler<int> OnValueChanged = delegate { }; 

      private readonly TextBox _textBox;
      public readonly string PropertyName;

      public ExtendedNumeric(string propName)
      {
         PropertyName = propName;
         Dock = DockStyle.Fill;
         Height = 21;

         // Access the internal TextBox control using reflection
         var type = GetType().BaseType!;
         var textBoxProperty = type.GetProperty("TextBox", BindingFlags.Instance | BindingFlags.NonPublic);
         _textBox = (TextBox)textBoxProperty?.GetValue(this, null)!;
         _textBox.KeyPress += TextBox_KeyPress;
         _textBox.LostFocus += OnFocusLost;
         _textBox.KeyDown += TextBox_KeyDown;
      }

      private void TextBox_KeyPress(object? sender, KeyPressEventArgs e)
      {
         if (!char.IsDigit(e.KeyChar) && e.KeyChar != '\b' && e.KeyChar != (char)Keys.Enter) 
            e.Handled = true;
      }

      private void TextBox_KeyDown(object? sender, KeyEventArgs e)
      {
         if (e.KeyCode == Keys.Enter)
         {
            e.SuppressKeyPress = true;
            OnValueChanged.Invoke(this, int.Parse(Text));
         }
      }

      private void OnFocusLost(object? sender, EventArgs e)
      {
         Debug.WriteLine("FocusLost!");
      }

      public override void UpButton()
      {
         base.UpButton();
         if (ModifierKeys.HasFlag(Keys.Control))
         {
            Value = Math.Min(Value + 9, Maximum);
            UpButtonPressedLarge.Invoke(this, new (Selection.GetSelectedProvinces, Value));
         }
         else if (ModifierKeys.HasFlag(Keys.Shift))
         {
            Value = Math.Min(Value + 4, Maximum);
            UpButtonPressedMedium.Invoke(this, new (Selection.GetSelectedProvinces, Value));
         }
         else
         { 
            UpButtonPressedSmall.Invoke(this, new (Selection.GetSelectedProvinces, Value));
         }
         OnValueChanged.Invoke(this, int.Parse(Text));
      }

      public override void DownButton()
      {
         base.DownButton();
         if (ModifierKeys.HasFlag(Keys.Control))
         {
            Value = Math.Max(Value - 9, Minimum);
            DownButtonPressedLarge.Invoke(this, new (Selection.GetSelectedProvinces, Value));
         }
         else if (ModifierKeys.HasFlag(Keys.Shift))
         {
            Value = Math.Max(Value - 4, Minimum);
            DownButtonPressedMedium.Invoke(this, new (Selection.GetSelectedProvinces, Value));
         }
         else
         {
            DownButtonPressedSmall.Invoke(this, new (Selection.GetSelectedProvinces, Value));
         }
         OnValueChanged.Invoke(this, int.Parse(Text));
      }
   }
}