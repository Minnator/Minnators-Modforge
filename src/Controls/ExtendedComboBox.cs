using Editor.Events;
using Editor.Helper;
namespace Editor.Controls
{
   public sealed class ExtendedComboBox : ComboBox
   {
      public static bool AllowEvents
      {
         get => _allowEvents;
         set => _allowEvents = value;
      }

      public EventHandler<ProvinceEditedEventArgs>? OnDataChanged = delegate { };
      public EventHandler<ProvinceCollectionEventArgs>? OnCollectionDataChanged = delegate { };
      private static bool _allowEvents = true;

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
         if (!AllowEvents)
            return;
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