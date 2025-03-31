using System.Reflection;
using Editor.Controls;
using Editor.DataClasses.GameDataClasses;
using Editor.DataClasses.Saveables;
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
      public static void OnExtendedComboBoxSelectedIndexChanged(object? sender, EventArgs e)
      {
         if (Globals.State == State.Running)
            if (sender is ExtendedComboBox { SelectedItem: not null } box)
             Saveable.SetFieldMultiple(Selection.GetSelectedProvincesAsSaveable(), box.SelectedItem.ToString(),
                typeof(Province).GetProperty(box.PropertyName)!);
      }

      public static void OnItemAddedModified(object? sender, EventArgs e)
      {
         if (Globals.State == State.Running)
            if (sender is ItemList list)
               Saveable.SetFieldMultiple(Selection.GetSelectedProvincesAsSaveable(), list.GetItems(), typeof(Province).GetProperty(list.PropertyName)!);
      }

      public static void OnItemRemoveModified(object? sender, EventArgs e)
      {
         if (Globals.State == State.Running)
            if (sender is ItemList list)
               Saveable.SetFieldMultiple(Selection.GetSelectedProvincesAsSaveable(), list.GetItems(), typeof(Province).GetProperty(list.PropertyName)!);
      }

      public static void OnExtendedNumericValueChanged(object? sender, int value)
      {
         if (Globals.State == State.Running)
            if (sender is ExtendedNumeric numeric)
               Saveable.SetFieldMultiple(Selection.GetSelectedProvincesAsSaveable(), value, typeof(Province).GetProperty(numeric.PropertyName)!);
      }

      public static void OnExtendedCheckBoxCheckedChanged(object? sender, EventArgs e)
      {
         if (Globals.State == State.Running)
         {
            if (!(sender is ExtendedCheckBox extendedCheckBox))
               return;
            Saveable.SetFieldMultiple(Selection.GetSelectedProvincesAsSaveable(), extendedCheckBox.Checked,
               typeof(Province).GetProperty(extendedCheckBox.PropertyName)!);
         }
      }
   }

}