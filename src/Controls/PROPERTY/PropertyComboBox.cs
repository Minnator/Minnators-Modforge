using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Editor.DataClasses.DataStructures;
using Editor.DataClasses.Saveables;
using Editor.ErrorHandling;
using Editor.Events;
using Editor.Helper;
using Editor.Saving;

namespace Editor.Controls.PROPERTY
{
   public class PropertyComboBox<TSaveable, TProperty> : ComboBox, IPropertyControl<TSaveable, TProperty> where TSaveable : Saveable 
   {
      public PropertyInfo PropertyInfo { get; init; }
      protected readonly Func<List<TSaveable>> GetSaveables;

      public PropertyComboBox(PropertyInfo? propertyInfo, ref LoadGuiEvents.LoadAction<TSaveable> loadHandle, Func<List<TSaveable>> getSaveables)
      {
         Debug.Assert(propertyInfo is not null && propertyInfo.DeclaringType == typeof(TSaveable), $"PropInfo: {propertyInfo} declaring type is not of type {typeof(TSaveable)} but of type {propertyInfo.DeclaringType}");
         Debug.Assert(propertyInfo.PropertyType == typeof(TProperty), $"PropInfo: {propertyInfo} is not of type {typeof(TProperty)} but of type {propertyInfo.PropertyType}");

         GetSaveables = getSaveables;
         PropertyInfo = propertyInfo;
         loadHandle += ((IPropertyControl<TSaveable, TProperty>)this).LoadToGui;

         MouseDown += (sender, e) =>
         {
            if (CopyPasteHandler.OnMouseDown(sender, e, ModifierKeys))
            {
               if (AttributeHelper.GetSharedAttribute(PropertyInfo, out TProperty value, GetSaveables.Invoke()))
                  SetValue(value);
            }
         };
      }

      public virtual IErrorHandle GetFromGui(out TProperty value)
      {
         return Converter.Convert(Text, out value);
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

      public virtual void SetFromGui()
      {
         if (Globals.State == State.Running && GetFromGui(out var value).Log())
            Saveable.SetFieldMultiple(GetSaveables.Invoke(), value, PropertyInfo);
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

   public class BindablePropertyComboBox<TSaveable, TProperty, TKey> : PropertyComboBox<TSaveable, TProperty> where TSaveable : Saveable where TProperty : notnull where TKey : notnull
   {
      private readonly BindingDictionary<TKey, TProperty> _items;

      public BindablePropertyComboBox(PropertyInfo? propertyInfo,
                                      ref LoadGuiEvents.LoadAction<TSaveable> loadHandle,
                                      Func<List<TSaveable>> getSaveables,
                                      BindingDictionary<TKey, TProperty> items) : base(propertyInfo, ref loadHandle, getSaveables)
      {
         _items = items;
         DataSource = new BindingSource(_items, null);
         _items.AddControl(this);
      }

      ~BindablePropertyComboBox()
      {
         _items.RemoveControl(this);
      }

      public new void SetDefault()
      {
         var item = _items.EmptyItem.Key.ToString() ?? string.Empty;
         SelectedText = item;
         SelectedIndex = -1;
         Text = item;
      }

      public override IErrorHandle GetFromGui(out TProperty value)
      {
         var handle = Converter.Convert(Text, out TKey key);
         if (!handle.Ignore())
         {
            value = default!;
            return handle;
         }
         if (_items.TryGetValue(key, out value!))
            return handle;
         return new ErrorObject(ErrorType.INTERNAL_KeyNotFound, "Key not found in dictionary", LogType.Critical, addToManager:false);
      }
   }

   public class BindableFakePropertyComboBox<TSaveable, TProperty, TKey> : BindablePropertyComboBox<TSaveable, TProperty, TKey> where TSaveable : ProvinceComposite where TProperty : ProvinceCollection<TSaveable> where TKey : notnull
   {
      public BindableFakePropertyComboBox(PropertyInfo? propertyInfo,
                                      ref LoadGuiEvents.LoadAction<TSaveable> loadHandle,
                                      Func<List<TSaveable>> getSaveables,
                                      BindingDictionary<TKey, TProperty> items) : base(propertyInfo, ref loadHandle, getSaveables, items)
      {
      }

      public override void SetFromGui()
      {
         if (Globals.State == State.Running && GetFromGui(out var value).Log())
            Saveable.SetFieldMultipleCollection(GetSaveables.Invoke(), value, PropertyInfo);
      }

   }
}