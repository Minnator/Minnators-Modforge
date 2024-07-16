using Editor.Helper;

namespace Editor.Controls
{
   public sealed class TagComboBox : ComboBox
   {
      public TagComboBox()
      {
         AutoCompleteMode = AutoCompleteMode.SuggestAppend;
         AutoCompleteSource = AutoCompleteSource.ListItems;
         Dock = DockStyle.Fill;
         
         GenerateOptions();
         GlobalEventHandlers.OnCountryListChanged += (sender, args) => GenerateOptions();
      }

      public void InitializeItems(List<string> items)
      {
         items.Sort();
         Items.Clear();
         foreach (var item in items)
            Items.Add(item);
      }

      private void GenerateOptions()
      {
         InitializeItems([.. Globals.Countries.Keys]);
      }
   }
}