using System.Reflection;
using Editor.ErrorHandling;
using Editor.Saving;
using Editor.Events;
using System.Diagnostics;
namespace Editor.Controls.NewControls
{
   public class PropertyLabel<TSaveable> : Label, IPropertyControl<TSaveable, string> where TSaveable : Saveable
   {
      public PropertyInfo PropertyInfo { get; init; }

      public PropertyLabel(PropertyInfo? propertyInfo, ref LoadGuiEvents.LoadAction<TSaveable> loadHandle)
      {
         Debug.Assert(propertyInfo is not null && propertyInfo.DeclaringType == typeof(TSaveable), $"PropInfo: {propertyInfo} declaring type is not of type {typeof(TSaveable)} but of type {propertyInfo.DeclaringType}");
         Debug.Assert(propertyInfo.PropertyType == typeof(string), $"PropInfo: {propertyInfo} is not of type {typeof(string)} but of type {propertyInfo.PropertyType}");
         SetDefault();
         PropertyInfo = propertyInfo;
         loadHandle += ((IPropertyControl<TSaveable, string>)this).LoadToGui;
      }

      public void SetFromGui()
      {
         throw new EvilActions("Lol, Why do this?");
      }

      public void SetDefault()
      {
         Text = "-|-";   
      }

      public IErrorHandle GetFromGui(out string value)
      {
         throw new EvilActions("Lol, Why do this?");
      }

      public void SetValue(string value)
      {
         Text = value;
      }
   }
}