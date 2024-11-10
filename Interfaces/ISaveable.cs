using System.ComponentModel;
using Editor.DataClasses;
using Editor.Helper;
using Newtonsoft.Json;

namespace Editor.Interfaces;

public enum ObjEditingStatus
{
   Unchanged,
   Modified,
   Immutable
}


public abstract class Saveable
{
   protected ObjEditingStatus _editingStatus = ObjEditingStatus.Unchanged;
   private PathObj _path = PathObj.Empty;
   [Browsable(false)]
   [JsonIgnore]
   public PathObj Path => _path;
   public void SetPath(ref PathObj path) => _path = path;

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
            FileManager.AddToBeHandled(this);
         _editingStatus = value;
      } 
   }

   public abstract SaveableType WhatAmI();

   public abstract string SavingComment();
   /// <summary>
   /// The internal default path
   /// </summary>
   /// <returns></returns>
   public abstract PathObj GetDefaultSavePath();
   public virtual string GetHeader()
   {
      return string.Empty;
   }
   public abstract string GetSaveString(int tabs);
   public abstract string GetSavePromptString();
}