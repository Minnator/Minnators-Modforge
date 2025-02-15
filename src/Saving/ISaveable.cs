using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using Editor.DataClasses.Commands;
using Editor.DataClasses.GameDataClasses;
using Editor.DataClasses.MapModes;
using Editor.Events;
using Editor.Helper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Editor.Saving;

public enum ObjEditingStatus
{
   Unchanged,
   Modified,
   Immutable,
   ToBeDeleted,
   Deleted
}

public abstract class Saveable : IDisposable
{
   protected ObjEditingStatus _editingStatus = ObjEditingStatus.Unchanged;
   internal bool Suppressed;
   [Browsable(false)]
   [JsonIgnore]
   public PathObj Path = PathObj.Empty;

   // Says if the object should be saved as a file or if for all objects of this type GetSavingString is called
   public virtual bool GetSaveStringIndividually => true;

   [Browsable(false)]
   public virtual ObjEditingStatus EditingStatus
   {
      get => _editingStatus;
      set
      {
         if (_editingStatus == ObjEditingStatus.Immutable)
            return;
         if (Equals(value, _editingStatus))
            return;
         switch (value)
         {
            case ObjEditingStatus.Modified:
            case ObjEditingStatus.ToBeDeleted:
               SaveMaster.AddToBeHandled(this);
               break;
            case ObjEditingStatus.Unchanged:
            case ObjEditingStatus.Deleted:
               SaveMaster.RemoveFromToBeHandled(this);
               break;
         }

         //TODO: what if we undo here? - ~no.
         _editingStatus = value;
      }
   }



   public abstract void OnPropertyChanged([CallerMemberName] string? propertyName = null);
   //public abstract void AddToPropertyChanged(EventHandler<string> handler);
   //public abstract void RemoveFromPropertyChanged(EventHandler<string> handler);

   public PropertyInfo? GetPropertyInfo(string propertyName)
   {
      return GetType().GetProperty(propertyName);
   }

   public object GetProperty(PropertyInfo? info)
   {
      Debug.Assert(info != null, "info != null");
      return info.GetValue(this);
   }

   public T GetProperty<T>(PropertyInfo info)
   {
      Debug.Assert(info != null, "info != null");
      return (T)info.GetValue(this);
   }

   public T GetProperty<T>(string propertyName)
   {
      Debug.Assert(GetPropertyInfo(propertyName) != null, $"Property {propertyName} not found in {GetType().Name}");
      return (T)GetProperty(GetPropertyInfo(propertyName)!);
   }

   public object GetPropertyValue(string propName)
   {
      return GetType().GetProperty(propName)?.GetValue(this) ?? string.Empty;
   }

   /// <summary>
   /// TEMP used to set collections as we have not yet implemented proper usage of observable collections
   /// </summary>
   /// <typeparam name="T"></typeparam>
   /// <typeparam name="Q"></typeparam>
   /// <param name="field"></param>
   /// <param name="value"></param>
   /// <param name="propertyName"></param>
   /// <returns></returns>
   public bool SetIfModifiedEnumerable<T, Q>(ref T field, T value, [CallerMemberName] string? propertyName = null) where T : IEnumerable<Q>
   {
      Debug.Assert(field is not null && value is not null, "field is not null && value is not null in SetIfModifiedEnumerable");
      if (field.SequenceEqual(value))
         return false;
      return InternalFieldSet(ref field, value, GetPropertyInfo(propertyName!)!);
   }

   public void SetProperty<T>(string propertyName, T value)
   {
      Debug.Assert(GetPropertyInfo(propertyName) != null, $"Property {propertyName} not found in {GetType().Name}");
      PropertyInfo info;
      lock (this)
      {
         info = GetPropertyInfo(propertyName)!;
         info.SetValue(this, value);
      }
   }

   /// <summary>
   /// 
   /// </summary>
   /// <typeparam name="T"></typeparam>
   /// <param name="propertyName"></param>
   /// <param name="value"></param>
   public void SetFieldSilent<T> (string propertyName, T value)
   {
      Debug.Assert(GetPropertyInfo(propertyName) != null, $"Property {propertyName} not found in {GetType().Name}");
      PropertyInfo info;
      lock (this)
      {
         Suppressed = true;
         info = GetPropertyInfo(propertyName)!;
         info.SetValue(this, value);
         Suppressed = false;
      }
      LoadGuiEvents.TriggerGuiUpdate(GetType(), info);
   }

   /// <summary>
   /// 
   /// </summary>
   /// <typeparam name="T"></typeparam>
   /// <param name="info"></param>
   /// <param name="value"></param>
   private void SetFieldSilent<T>(PropertyInfo info, T value, bool update = true)
   {
      lock (this)
      {
         Suppressed = true;
         info.SetValue(this, value);
         Suppressed = false;
      }
      if (update)
         LoadGuiEvents.TriggerGuiUpdate(GetType(), info);
   }

   public static void SetFieldMultipleCollection<TS, T>(ICollection<TS> targets, T value, PropertyInfo property) where TS : ProvinceComposite where T : ProvinceCollection<TS>
   {
      if (Globals.State == State.Running)
         value.NewAddRange(targets);
      foreach (var target in targets)
         target.OnPropertyChanged(property.Name);
   }

   public static void SetFieldEditCollection<TSaveable, TProperty, TItem>(ICollection<TSaveable> targets, HashSet<TItem> add, HashSet<TItem> remove, PropertyInfo property) where TSaveable : Saveable where TProperty : ICollection<TItem>
   {
      if (Globals.State != State.Running)
         return;
      var command = new CModifyListProperty<TSaveable, TProperty, TItem>(property, [.. targets], add, remove, out var change);
      if (change)
         HistoryManager.AddCommand(command);
      foreach (var target in targets)
         target.OnPropertyChanged(property.Name);
   }

   /// <summary>
   /// 
   /// </summary>
   /// <typeparam name="TS"></typeparam>
   /// <typeparam name="T"></typeparam>
   /// <param name="targets"></param>
   /// <param name="value"></param>
   /// <param name="property"></param>
   public static void SetFieldMultiple<TS, T>(ICollection<TS> targets, T value, PropertyInfo property) where TS : Saveable
   {
      if (Globals.State == State.Running)
      {
         var command = new CModifyProperty<T>(property, [.. targets], value, out var change);
         if (change)
            HistoryManager.AddCommand(command);
      }
      foreach(var target in targets)
         target.OnPropertyChanged(property.Name);
   }

   public static void SetFieldMultipleSilent<TS, T>(ICollection<TS> targets, T value, PropertyInfo property) where TS : Saveable
   {
      foreach (var target in targets)
      {
         lock (target)
         {
            target.Suppressed = true;
            property.SetValue(target, value);
            target.Suppressed = false;
         }
      }
      LoadGuiEvents.TriggerGuiUpdate(typeof(TS), property);
   }

   public static void SetFieldMultipleDifferentSilent<TS, T>(ICollection<TS> targets, List<T> values, PropertyInfo property)
      where TS : Saveable
   {
      Debug.Assert(targets.Count == values.Count, $"targets and values must have the same amount of values but are: ({targets.Count}|{values.Count})!");
      var cnt = 0;
      foreach (var target in targets)
      {
         lock (target)
         {
            target.Suppressed = true;
            property.SetValue(target, values[cnt++]);
            target.Suppressed = false;
         }
      }
      LoadGuiEvents.TriggerGuiUpdate(typeof(TS), property);
   }

   protected static void SetFieldMultiple<TS, T>(ICollection<TS> targets, List<T> value, PropertyInfo property) where TS : Saveable
   {
      if (Globals.State == State.Running)
      {
         var command = new CModifyPropertyMultiple<T>(property, [.. targets], value, out var change);
         if (change)
            HistoryManager.AddCommand(command);
      }
      foreach (var target in targets)
         target.OnPropertyChanged(property.Name);
   }

   /// <summary>
   /// Is always called when a value in a saveable is changed (If the property calls SetField)
   /// </summary>
   /// <typeparam name="T"></typeparam>
   /// <param name="field"></param>
   /// <param name="value"></param>
   /// <param name="propertyName"></param>
   /// <returns></returns>
   protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
   {
      if (EqualityComparer<T>.Default.Equals(field, value)) 
         return false;
      return InternalFieldSet(ref field, value, GetPropertyInfo(propertyName!)!);
   }

   /// <summary>
   /// Is always called when a value in a saveable is changed (If the property calls SetField)
   /// Will call OnPropertyChange if it is not suppresed
   /// </summary>
   /// <typeparam name="T"></typeparam>
   /// <param name="field"></param>
   /// <param name="value"></param>
   /// <param name="property"></param>
   /// <returns></returns>
   private bool InternalFieldSet<T>(ref T field, T value, PropertyInfo property)
   {
      if (Globals.State == State.Running && !Suppressed)
      {
         HistoryManager.AddCommand(new CModifyProperty<T>(property, this, value, field));
         OnPropertyChanged(property.Name);
      }
      field = value;
      return true;
   }


   public void SetPath(ref PathObj path)
   {
      Path = path;
   }

   public virtual string GetHeader()
   {
      return string.Empty;
   }

   public virtual string GetFooter()
   {
      return string.Empty;
   }

   public virtual string GetFullFileString(List<Saveable> changed, List<Saveable> unchanged)
   {
      throw new EvilActions("NO, don't save like dis");
   }

   
   /// <summary>
   /// This MUST always return a single SaveableType and no combinations. Otherwise, Exception.
   /// </summary>
   /// <returns></returns>
   public abstract SaveableType WhatAmI();

   /// <summary>
   /// The folder where the object should be saved
   /// </summary>
   /// <returns></returns>
   public abstract string[] GetDefaultFolderPath();

   /// <summary>
   /// Formatted like: ".txt"
   /// </summary>
   /// <returns></returns>
   public abstract string GetFileEnding();

   /// <summary>
   /// Returns the default file name for a saveable, if it was not saved in vanilla and was not overwritten by the user unless it is forced
   /// </summary>
   /// <returns>KeyValuePair where the key is the default file name and the bool, which indicates if it is forced aka not overwritteable by the user</returns>
   public abstract KeyValuePair<string, bool> GetFileName();

   public abstract string SavingComment();

   public abstract string GetSaveString(int tabs);

   /// <summary>
   /// Should line up with: Please enter your input for: your string
   /// </summary>
   /// <returns></returns>
   public abstract string GetSavePromptString();

   /// <summary>
   /// If overwritten also call base.Dispose() to remove the object from the dictionary in SaveMaster
   /// </summary>
   public virtual void Dispose() => SaveMaster.RemoveFromDictionary(this);

   public virtual string ToPropertyString() => ToString() ?? string.Empty;
}