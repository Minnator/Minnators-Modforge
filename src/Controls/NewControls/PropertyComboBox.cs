using System.Diagnostics;
using System.Reflection;
using Editor.Events;
using Editor.Helper;
using Editor.Saving;

namespace Editor.Controls.NewControls
{
   public class PropertyComboBox<TSaveable, TProperty> : ComboBox, IPropertyControl<TSaveable, TProperty> where TSaveable : Saveable 
   {
      public PropertyInfo PropertyInfo { get; init; }
      private readonly Func<List<TSaveable>> _getSaveables;

      public PropertyComboBox(PropertyInfo? propertyInfo, ref LoadGuiEvents.LoadAction<TSaveable> loadHandle, Func<List<TSaveable>> getSaveables)
      {
         Debug.Assert(propertyInfo is not null && propertyInfo.DeclaringType == typeof(TSaveable), $"PropInfo: {propertyInfo} declaring type is not of type {typeof(TSaveable)} but of type {propertyInfo.DeclaringType}");
         Debug.Assert(propertyInfo.PropertyType == typeof(TProperty), $"PropInfo: {propertyInfo} is not of type {typeof(TProperty)} but of type {propertyInfo.PropertyType}");

         _getSaveables = getSaveables;
         PropertyInfo = propertyInfo;
         loadHandle += ((IPropertyControl<TSaveable, TProperty>)this).LoadToGui;
      }

      public TProperty GetFromGui()
      {
         if (Converter.Convert(Text, PropertyInfo, out TProperty value).Log())
            return value;
         return default!;
      }

      protected override void OnSelectedIndexChanged(EventArgs e)
      {
         base.OnSelectedIndexChanged(e);
         SetFromGui();
      }

      protected override void OnKeyPress(KeyPressEventArgs e)
      {
         if (e.KeyChar == (char)Keys.Enter)
            SetFromGui();
         base.OnKeyPress(e);
      }

      public void SetFromGui()
      {
         if (Globals.State == State.Running && Converter.Convert(Text, PropertyInfo, out TProperty value).Log())
            Saveable.SetFieldMultiple(_getSaveables.Invoke(), value, PropertyInfo);
      }

      public void SetDefault()
      {
         SelectedText = "";
         SelectedIndex = -1;
         Text = "";
      }

      public void SetValue(TProperty value)
      {
         Debug.Assert(value != null, "value is null but must never be null");
         Text = value.ToString();
      }
   }
}