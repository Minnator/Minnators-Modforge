using Editor.DataClasses.GameDataClasses;
using Editor.Events;
using Editor.Helper;

namespace Editor.Controls
{
   public sealed class TagComboBox : ComboBox
   {
      public EventHandler<ProvinceEditedEventArgs>? OnTagChanged = delegate { };
      public bool IgnoreEmpty { get; set; } = false;
      public readonly string PropertyName;
      public TagComboBox(string propName)
      {
         PropertyName = propName;

         Dock = DockStyle.Fill;
         Height = 21;
         
         DataSource = new BindingSource
         {
            DataSource = Globals.Countries
         };
         Globals.Countries.AddControl(this);
         AutoCompleteSource = AutoCompleteSource.ListItems;
         AutoCompleteMode = AutoCompleteMode.SuggestAppend;
      }

      protected override void OnSelectedIndexChanged(EventArgs e)
      {
         if (IgnoreEmpty && (Tag)Text == DataClasses.GameDataClasses.Tag.Empty)
         {
            Text = string.Empty;
            SelectedIndex = -1;
            return;
         }

         base.OnSelectedIndexChanged(e);
         OnTagChanged?.Invoke(this, new (Selection.GetSelectedProvinces, Text));
      }


      protected override void OnKeyPress(KeyPressEventArgs e)
      {
         if (!char.IsLetter(e.KeyChar) && e.KeyChar != (char)Keys.Back)
         {
            e.Handled = true; // Ignore non-letter keys except backspace
            return;
         }

         string newText;

         if (e.KeyChar == (char)Keys.Back)
         {
            // Handle backspace: remove selected text or last character
            if (SelectionLength > 0)
               newText = Text.Remove(SelectionStart, SelectionLength);
            else if (Text.Length > 0)
               newText = Text[..^1];
            else
               newText = string.Empty;
         }
         else
            newText = Text.Remove(SelectionStart, SelectionLength)
                         .Insert(SelectionStart, char.ToUpper(e.KeyChar).ToString());

         // Enforce max length of 3
         if (newText.Length > 3)
         {
            e.Handled = true;
            return;
         }

         Text = newText;
         SelectionStart = Text.Length;
         SelectionLength = 0;
         e.Handled = true;
      }
   }
   
   public static class ComboBoxExtensions
   {
      public static void Clear(this ComboBox box)
      {
         box.Text = "";
         box.SelectedIndex = -1;
      }
   }
}