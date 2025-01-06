using System.Diagnostics;
using System.Reflection;
using Editor.ErrorHandling;
using Editor.Events;
using Editor.Saving;

namespace Editor.Controls.NewControls
{
   public class PropertyCheckBox<T> : CheckBox, IPropertyControl<T, bool> where T : Saveable
   {
      private readonly Func<List<T>> getSaveables;
      public PropertyInfo PropertyInfo { get; init; }

      public PropertyCheckBox(PropertyInfo? propertyInfo, ref LoadGuiEvents.LoadAction<T> loadHandle, Func<List<T>> getSaveables)
      {
         Debug.Assert(propertyInfo is not null && propertyInfo.DeclaringType == typeof(T), $"PropInfo: {propertyInfo} declaring type is not of type {typeof(T)} but of type {propertyInfo.DeclaringType}");
         Debug.Assert(propertyInfo.PropertyType == typeof(bool), $"PropInfo: {propertyInfo} is not of type {typeof(bool)} but of type {propertyInfo.PropertyType}");
         PropertyInfo = propertyInfo;
         this.getSaveables = getSaveables;
         loadHandle += ((IPropertyControl<T, bool>)this).LoadToGui;
      }
      
      public PropertyCheckBox(string propName, ref LoadGuiEvents.LoadAction<T> loadHandle, Func<List<T>> getSaveables) : this(typeof(T).GetProperty(propName), ref loadHandle, getSaveables) { }

      public IErrorHandle GetFromGui(out bool value)
      {
         value = Checked;
         return ErrorHandle.Sucess;
      }

      protected override void OnCheckedChanged(EventArgs e)
      {
         base.OnCheckedChanged(e);
         SetFromGui();
      }

      public void SetFromGui()
      {
         if (Globals.State == State.Running)
            Saveable.SetFieldMultiple(getSaveables.Invoke(), Checked, PropertyInfo);
      }

      public void SetDefault()
      {
         Checked = false;
      }

      public void SetValue(bool value)
      {
         Checked = value;
      }


   }
}