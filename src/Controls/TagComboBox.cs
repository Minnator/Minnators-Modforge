using Editor.DataClasses.GameDataClasses;
using Editor.Events;
using Editor.Helper;

namespace Editor.Controls
{
   public sealed class TagComboBox : ComboBox
   {
      public EventHandler<ProvinceEditedEventArgs>? OnTagChanged = delegate { };
      public bool IgnoreEmpty { get; set; } = false;
      public TagComboBox()
      {
         Dock = DockStyle.Fill;
         Height = 21;
         
         DataSource = new BindingSource
         {
            DataSource = Globals.Countries
         };
         Globals.Countries.AddControl(this);
         //TODO broken https://stackoverflow.com/questions/11780558/c-sharp-winforms-combobox-dynamic-autocomplete
         //AutoCompleteMode = AutoCompleteMode.SuggestAppend;
         //AutoCompleteSource = AutoCompleteSource.CustomSource;


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
         if (Text.Length >= 3)
         {
            e.Handled = true;
            return;
         }
         base.OnKeyPress(e);
         e.KeyChar = char.ToUpper(e.KeyChar);
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