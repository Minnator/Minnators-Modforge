using System.Diagnostics;
using System.Reflection;
using Editor.Controls.NewControls;
using Editor.ErrorHandling;
using Editor.Events;
using Editor.Helper;
using Editor.Saving;
using Editor.src.Forms.Feature;

namespace Editor.Controls
{
   public sealed class PropertyCollectionSelector<TSaveable, TProperty, TPropertyItem> : Button, IPropertyControlList<TSaveable, TProperty, TPropertyItem>
      where TSaveable : Saveable where TProperty : List<TPropertyItem> where TPropertyItem : notnull
   {
      public PropertyInfo PropertyInfo { get; init; }
      private readonly Func<List<TSaveable>> _getSaveables;
      private CollectionSelectorBase _collectionSelectorBase;
      private PropertyInfo _displayMember;
      private List<TPropertyItem> _startList = [];


      public PropertyCollectionSelector(PropertyInfo? propertyInfo, ref LoadGuiEvents.LoadAction<TSaveable> loadHandle, Func<List<TSaveable>> getSaveables,
                                        List<TPropertyItem> sourceItems, PropertyInfo displayMember)
      {
         Debug.Assert(propertyInfo is not null && propertyInfo.DeclaringType == typeof(TSaveable),
                      $"PropInfo: {propertyInfo} declaring type is not of type {typeof(TSaveable)} but of type {propertyInfo.DeclaringType}");
         Debug.Assert(propertyInfo.PropertyType == typeof(TProperty),
                      $"PropInfo: {propertyInfo} is not of type {typeof(TProperty)} but of type {propertyInfo.PropertyType}");
         if (sourceItems.Count >= 1)
            Debug.Assert(_displayMember!.GetValue(sourceItems[0]) != null, $"{typeof(TPropertyItem)} does not define a display member {displayMember}");

         PropertyInfo = propertyInfo;
         _getSaveables = getSaveables;
         loadHandle += ((IPropertyControlList<TSaveable, TProperty, TPropertyItem>)this).LoadToGui;
         _displayMember = displayMember;

         _collectionSelectorBase = new(GetDisplayMember(sourceItems, displayMember));
         if (AttributeComparer.GetSharedAttributeList<TSaveable, TProperty, TPropertyItem>(PropertyInfo, out var items, getSaveables.Invoke()))
            _collectionSelectorBase.SetSelectedItems(GetDisplayMember(items, displayMember));

         Text = "Modify";
         Click += (sender, e) =>
         {
            _startList.Clear();
            GetFromGui(out var startList);
            _startList = startList;
            _collectionSelectorBase.ShowDialog();
            SetFromGui();
         };
      }


      private List<string> GetDisplayMember(List<TPropertyItem> items, PropertyInfo displayMember)
      {
         var displayMembers = new List<string>();
         foreach (var item in items)
            displayMembers.Add(displayMember.GetValue(item)!.ToString()!);
         return displayMembers;
      }


      public void SetFromGui()
      {
         if (Globals.State == State.Running && GetFromGui(out var value).Log())
         {
            if (AttributeComparer.ScrambledListsEquals(_startList, value))
               return;
            Saveable.SetFieldMultiple(_getSaveables.Invoke(), value, PropertyInfo);
         }
      }

      public void SetDefault()
      {
         SetValue((TProperty)new List<TPropertyItem>());
      }

      public IErrorHandle GetFromGui(out TProperty value)
      {
         var strs = _collectionSelectorBase.GetSelectedItems();
         List<TPropertyItem> items = new(strs.Count);
         
         foreach (var str in strs)
            if (Converter.Convert<TPropertyItem>(str, PropertyInfo, out var partValue).Log())
               items.Add(partValue);

         value = (TProperty)items;
         return ErrorHandle.Sucess;
      }

      public void SetValue(TProperty value)
      {
         _collectionSelectorBase.SetSelectedItems(GetDisplayMember(value, _displayMember));
      }
   }
}