using System.ComponentModel;
using System.Reflection;
using Editor.Events;
using Editor.Saving;

namespace Editor.Controls.PROPERTY
{
   public class BindableListPropertyComboBox<TSaveable, TProperty> : PropertyComboBox<TSaveable, TProperty> where TSaveable : Saveable
   {
      public BindableListPropertyComboBox(PropertyInfo? propertyInfo, ref LoadGuiEvents.LoadAction<TSaveable> loadHandle, Func<List<TSaveable>> getSaveables, BindingList<TProperty> items) : base(propertyInfo, ref loadHandle, getSaveables)
      {
         DataSource = items;
      }
   }

   public class ListPropertyComboBox<TSaveable, TProperty> : PropertyComboBox<TSaveable, TProperty> where TSaveable : Saveable
   {
      public ListPropertyComboBox(PropertyInfo? propertyInfo,
                                  ref LoadGuiEvents.LoadAction<TSaveable> loadHandle,
                                  Func<List<TSaveable>> getSaveables,
                                  List<TProperty> items) : base(propertyInfo, ref loadHandle, getSaveables)
      {
         DataSource = items;
      }
   }
}