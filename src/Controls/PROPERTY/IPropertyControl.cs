using System.Reflection;
using Editor.ErrorHandling;
using Editor.Helper;
using Editor.Saving;

namespace Editor.Controls.PROPERTY
{
   public interface IPropertyControl
   {
      public PropertyInfo PropertyInfo { get; init; }

      protected void SetFromGui();
      public void SetDefault();
   }

   public interface IPropertyControl<T, Q> : IPropertyControl, ICopyable where T : Saveable
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

      void ICopyable.SetClipboard()
      {
         Globals.ClipboardPropertyInfo = PropertyInfo;
         GetFromGui(out var value);
         Globals.ClipboardValue = value;
      }

      void ICopyable.Paste()
      {
         if (Globals.ClipboardPropertyInfo == null || Globals.ClipboardValue == null)
            return;

         // depending on the tab we are in we either target the selected country or the selected provinces

         var targets = Globals.MapWindow.GetCurrentSaveables().ToList();

         // check if the saveable has the given property
         if (PropertyInfo.PropertyType != Globals.ClipboardPropertyInfo.PropertyType)
            return;

         Saveable.SetFieldMultiple(targets, Globals.ClipboardValue, PropertyInfo);
      }
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