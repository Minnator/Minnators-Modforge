using System.Collections;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Editor.DataClasses.Commands;
using Editor.DataClasses.Misc;
using Editor.Helper;
using Newtonsoft.Json;

namespace Editor.Saving;

public enum ObjEditingStatus
{
   Unchanged,
   Modified,
   Immutable,
   Deleted
}

public abstract class Saveable : IDisposable
{
   protected ObjEditingStatus _editingStatus = ObjEditingStatus.Unchanged;
   [Browsable(false)]
   [JsonIgnore]
   public PathObj Path = PathObj.Empty;

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
         if (Equals(value, ObjEditingStatus.Modified))
            SaveMaster.AddToBeHandled(this);
         if (Equals(value, ObjEditingStatus.Deleted))
            SaveMaster.AddToBeHandled(this);
         //TODO: what if we undo here?
         _editingStatus = value;
      }
   }
   public abstract void OnPropertyChanged([CallerMemberName] string? propertyName = null);

   public bool SetIfModifiedEnumerable<T, Q>(ref T field, T value, [CallerMemberName] string? propertyName = null) where T: IEnumerable<Q>
   {
      if (field is null || value is null || field.SequenceEqual(value))
         return false;
      return InternalFieldSet(ref field, value, propertyName);
   }

   protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
   {
      if (EqualityComparer<T>.Default.Equals(field, value)) 
         return false;
      return InternalFieldSet(ref field, value, propertyName);
   }

   private bool InternalFieldSet<T>(ref T field, T value, string? propertyName)
   {
      if (Globals.State == State.Running)
      {
         Globals.HistoryManager.AddCommand(new CModifyProperty<T>(propertyName, this, value, field, EditingStatus));
         OnPropertyChanged(propertyName);
         EditingStatus = ObjEditingStatus.Modified;
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

   public abstract string GetSavePromptString();

   /// <summary>
   /// If overwritten also call base.Dispose() to remove the object from the dictionary in SaveMaster
   /// </summary>
   public virtual void Dispose()
   {
      SaveMaster.RemoveFromDictionary(this);
   }
}