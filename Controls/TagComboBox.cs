using Editor.Helper;

namespace Editor.Controls
{
   public class TagComboBox : ComboBox
   {
      public TagComboBox()
      {
         AutoCompleteMode = AutoCompleteMode.SuggestAppend;
         AutoCompleteSource = AutoCompleteSource.ListItems;
         
         GenerateOptions();
         GlobalEventHandlers.OnCountryListChanged += (sender, args) => GenerateOptions();
      }
      
      private void GenerateOptions()
      {
         Items.Clear();
         Items.AddRange([Globals.Countries.Keys]);
      }

      protected override void OnTextChanged(EventArgs e)
      {
         if (Text.Length > 3)
         {
            Text = Text[..3];
         }
         base.OnTextChanged(e);
      }

      // Do not allow illegal Tags
      protected override void OnKeyDown(KeyEventArgs e)
      {
         if (e.KeyCode == Keys.Enter)
         {
            e.Handled = true;
            e.SuppressKeyPress = true;

            if (!Globals.Countries.ContainsKey(Text))
               Text = string.Empty;
         }
      }
   }
}