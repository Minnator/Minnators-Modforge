using System.Diagnostics;
using Editor.Helper;
using Editor.Saving;
using System.Reflection;
using Editor.DataClasses.GameDataClasses;
using Editor.ErrorHandling;

namespace Editor.Controls.NewControls
{
   public interface IPropertyControl
   {
      public PropertyInfo PropertyInfo { get; init; }

      protected void SetFromGui();
      public void SetDefault();
   }

   public interface IPropertyControl<T, Q> : IPropertyControl where T : Saveable
   {
      public IErrorHandle GetFromGui(out Q value);
      public void LoadToGui(List<T> list, PropertyInfo propInfo, bool force)
      {
         if (force || PropertyInfo.Equals(propInfo))
            if (AttributeHelper.GetSharedAttribute(PropertyInfo, out Q value, list))
               SetValue(value);
            else
               SetDefault();
      }
      public void SetValue(Q value);
   }

   public interface IPropertyControlList<T, Q, R> : IPropertyControl where T : Saveable where Q : ICollection<R>, new() where R : notnull
   {
      public void LoadToGui(List<T> list, PropertyInfo propInfo, bool force)
      {
         if (force || propInfo.Equals(PropertyInfo))
            if (AttributeHelper.GetSharedAttributeList<T, Q, R>(PropertyInfo, out Q value, list))
               SetValue(value);
            else
               SetDefault();
      }
      protected void SetValue(Q value);
   }

}