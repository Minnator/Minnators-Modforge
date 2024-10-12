using Editor.Events;
using Editor.Helper;
using ProvinceCollectionEventArgs = Editor.Events.ProvinceCollectionEventArgs;

namespace Editor.Controls
{
   public sealed class ExtendedComboBox : ComboBox
   {
      public EventHandler<ProvinceEditedEventArgs>? OnDataChanged = delegate { };
      public EventHandler<ProvinceCollectionEventArgs>? OnCollectionDataChanged = delegate { };

      public ExtendedComboBox()
      {
         AutoCompleteMode = AutoCompleteMode.SuggestAppend;
         AutoCompleteSource = AutoCompleteSource.ListItems;
         Dock = DockStyle.Fill;
         Height = 21;
      }

      protected override void OnSelectedIndexChanged(EventArgs e)
      {
         base.OnSelectedIndexChanged(e);
         OnDataChanged?.Invoke(this, new (Selection.GetSelectedProvinces, Text));
         OnCollectionDataChanged?.Invoke(this, new (Text, Selection.GetSelectedProvinces));
      }

      public void ReplaceItems(List<string> items)
      {
         Items.Clear();
         if (items.Count == 0)
            return;
         Items.AddRange([..items]);
      }
   }
}