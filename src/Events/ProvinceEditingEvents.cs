using Editor.Controls;
using Editor.DataClasses.GameDataClasses;
using Editor.Helper;
using Editor.Saving;

namespace Editor.Events
{
   public class ProvinceEditedEventArgs(List<Province> provinces, object value) : EventArgs
   {
      public List<Province> Provinces { get; set; } = provinces;
      public object Value { get; set; } = value;
   }

   public static class ProvinceEditingEvents
   {
      public static void OnTagComboBoxSelectedIndexChanged(object? sender, EventArgs e)
      {
         if (sender is TagComboBox { SelectedItem: not null } box)
            Saveable.SetFieldMultiple(Selection.GetSelectedProvincesAsSaveable(), box.SelectedItem.ToString(),
               box.PropertyName);
      }

      public static void OnExtendedComboBoxSelectedIndexChanged(object? sender, EventArgs e)
      {
         if (sender is ExtendedComboBox { SelectedItem: not null } box)
            Saveable.SetFieldMultiple(Selection.GetSelectedProvincesAsSaveable(), box.SelectedItem.ToString(),
               box.PropertyName);
      }

      public static void OnItemAddedModified(object? sender, EventArgs e)
      {
         if (sender is ItemList list)
            Saveable.SetFieldMultiple(Selection.GetSelectedProvincesAsSaveable(), list.GetItems(), list.PropertyName);
      }

      public static void OnItemRemoveModified(object? sender, EventArgs e)
      {
         if (sender is ItemList list)
            Saveable.SetFieldMultiple(Selection.GetSelectedProvincesAsSaveable(), list.GetItems(), list.PropertyName);
      }

      public static void OnExtendedNumericValueChanged(object? sender, int value)
      {
         if (sender is ExtendedNumeric numeric)
            Saveable.SetFieldMultiple(Selection.GetSelectedProvincesAsSaveable(), value, numeric.PropertyName);
      }
   }

}