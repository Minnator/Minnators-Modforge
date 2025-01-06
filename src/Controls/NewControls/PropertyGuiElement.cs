using System.Collections;
using System.Diagnostics;
using System.Reflection;
using Editor.Helper;
using Editor.Saving;

namespace Editor.Controls.NewControls
{
   public interface IPropertyControl
   {
      public PropertyInfo PropertyInfo { get; init; }
      
      public bool GetFromGui();
      public void SetFromGui();
      public void SetDefault();
   }

   public interface IPropertyControl<T, Q> : IPropertyControl where T : Saveable
   {
      public void LoadToGui(List<T> list, PropertyInfo propInfo, bool force)
      {
         if (force || PropertyInfo.Equals(propInfo))
            if (AttributeComparer.GetSharedAttribute(PropertyInfo, out Q value, list))
               SetValue(value);
            else
               SetDefault();
      }
      public void SetValue(Q value);
   }

   public interface IPropertyControlList<T, Q, R> : IPropertyControl where T : Saveable where Q : ICollection<R> where R : notnull
   {
      protected void LoadToGui(List<T> list, PropertyInfo propInfo, bool force)
      {
         if (force || propInfo.Equals(PropertyInfo))
            if (AttributeComparer.GetSharedAttributeList<T, Q, R>(PropertyInfo, out Q value, list))
               SetValue(value);
            else
               SetDefault();
      }
      protected void SetValue(Q value);
   }

   public static class AttributeComparer
   {
      public static bool GetSharedAttribute<T, Q>(PropertyInfo propertyInfo, out Q value, ICollection<T> objects)
      {
         var commonValue = propertyInfo.GetValue(objects.First());
         var result = objects.All((obj) => propertyInfo.GetValue(obj)!.Equals(commonValue));
         value = (Q)commonValue!;
         return result;
      }

      public static bool GetSharedAttributeList<T, Q, R>(PropertyInfo propertyInfo, out Q value, ICollection<T> objects) where Q : ICollection<R> where R : notnull
      {
         List<Q> lists = [];
         foreach (var obj in objects)
            lists.Add((Q)propertyInfo.GetValue(obj)!);

         if (AreAllListsEquals<Q, R>(lists))
         {
            value = lists[0];
            return true;
         }
         value = default!;
         return false;
      }

      public static bool ScrambledListsEquals<T>(ICollection<T> list1, ICollection<T> list2) where T : notnull
      {
         var dict = new Dictionary<T, int>(list1.Count);
         foreach (var s in list1)
            if (!dict.TryAdd(s, 1))
               dict[s]++;
         foreach (var s in list2)
            if (dict.TryGetValue(s, out var cnt))
               dict[s] = --cnt;
            else
               return false;
         return dict.Values.All(c => c == 0);
      }      
      
      public static bool ScrambledListsEquals<T>(ICollection<T> list1, ICollection<T> list2, Dictionary<T, int> compDictionary) where T : notnull
      {
         foreach (var s in list1)
            if (!compDictionary.TryAdd(s, 1))
               compDictionary[s]++;
         foreach (var s in list2)
            if (compDictionary.TryGetValue(s, out var cnt))
               compDictionary[s] = --cnt;
            else
               return false;
         return compDictionary.Values.All(c => c == 0);
      }

      public static bool AreAllListsEquals<T, Q>(List<T> lists) where T : ICollection<Q> where Q : notnull
      {
         if (lists.Count < 2)
            return true;

         var compDictionary = new Dictionary<Q, int>(lists[0].Count);
         foreach (var s in lists[0])
            if (!compDictionary.TryAdd(s, 1))
               compDictionary[s]++;

         compDictionary.TrimExcess();

         for (var i = 1; i < lists.Count; i++)
            if (!ScrambledListsEquals(lists[0], lists[i],new (compDictionary)))
               return false;

         return true;
      }
   }

   public class PropertyCheckBox<T> : CheckBox, IPropertyControl<T, bool> where T : Saveable
   {
      private readonly ICollection<T> _objects;
      public PropertyInfo PropertyInfo { get; init; }

      public PropertyCheckBox(PropertyInfo? propertyInfo, ref Action<List<T>, PropertyInfo, bool> loadHandle, ICollection<T> objects)
      {
         Debug.Assert(propertyInfo is not null && propertyInfo.DeclaringType == typeof(T), $"PropInfo: {propertyInfo} declaring type is not of type {typeof(T)} but of type {propertyInfo.DeclaringType}");
         Debug.Assert(propertyInfo.PropertyType == typeof(bool), $"PropInfo: {propertyInfo} is not of type {typeof(bool)} but of type {propertyInfo.PropertyType}");
         PropertyInfo = propertyInfo;
         _objects = objects;
         loadHandle += ((IPropertyControl<T, bool>)this).LoadToGui;
      }
      
      public PropertyCheckBox(string propName, ref Action<List<T>, PropertyInfo, bool> loadHandle, ICollection<T> objects) : this(typeof(T).GetProperty(propName), ref loadHandle, objects) { }

      public bool GetFromGui()
      {
         return Checked;
      }

      public void SetFromGui()
      {
         if (Globals.State == State.Running)
            foreach (var obj in _objects)
               PropertyInfo.SetValue(obj, Checked);
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