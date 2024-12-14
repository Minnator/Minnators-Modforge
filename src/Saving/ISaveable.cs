using System.ComponentModel;
using System.Runtime.CompilerServices;
using Editor.DataClasses.Commands;
using Editor.DataClasses.Misc;
using Newtonsoft.Json;

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
         Globals.HistoryManager.AddCommand(new CModifyProperty<T>(propertyName, this, value, field));
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
   public virtual void Dispose()
   {
      SaveMaster.RemoveFromDictionary(this);
   }
}