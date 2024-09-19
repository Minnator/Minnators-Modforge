﻿using Editor.Events;

namespace Editor.Controls
{
   public sealed class ExtendedComboBox : ComboBox
   {
      public EventHandler<ProvinceEditedEventArgs>? OnDataChanged = delegate { };
      public EventHandler<ProvinceCollectionEventArgs>? OnDataChangedC = delegate { };

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
         OnDataChanged?.Invoke(this, new (Globals.Selection.GetSelectedProvinces, Text));
         OnDataChangedC?.Invoke(this, new (Text, Globals.Selection.GetSelectedProvincesIds));
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