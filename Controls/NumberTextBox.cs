using Editor.Events;

namespace Editor.Controls
{
   public sealed class NumberTextBox : TextBox
   {
      public NumberTextBox()
      {
         Dock = DockStyle.Fill;
         TextAlign = HorizontalAlignment.Left;
         Text = "0";
      }

      public bool GetValueAsInt(out int value)
      {
         if (int.TryParse(Text, out value))
         {
            return true;
         }
         value = 0;
         return false;
      }

      public bool GetValueAsFloat(out float value)
      {
         if (float.TryParse(Text, out value))
         {
            return true;
         }
         value = 0.0f;
         return false;
      }


      // restricts the input to numbers only
      protected override void OnKeyPress(KeyPressEventArgs e)
      {
         if (e.KeyChar == (char)Keys.Enter)
         {
            // Custom Data changed event
            ProvinceEditedEventArgs args = new (Globals.Selection.GetSelectedProvinces, Text);
            OnTextChanged(args);
            e.Handled= true;
            return;
         }

         if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
         {
            e.Handled = true;
         }
         // only allow one decimal point
         if (e.KeyChar == '.' && Text.IndexOf('.') > -1)
         {
            e.Handled = true;
         }
         base.OnKeyPress(e);

      }

      protected override void OnLostFocus(EventArgs e)
      {
         // Custom Data changed event
         ProvinceEditedEventArgs args = new (Globals.Selection.GetSelectedProvinces, Text);
         OnTextChanged(args);
         base.OnLostFocus(e);
      }

      // Custom Data changed event
      public EventHandler<ProvinceEditedEventArgs>? OnDataChanged = delegate { };
      public void OnTextChanged(ProvinceEditedEventArgs e)
      {
         OnDataChanged?.Invoke(this, e);
         base.OnTextChanged(e);
      }
   }
}