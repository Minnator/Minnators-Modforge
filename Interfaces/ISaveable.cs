using Editor.Helper;

namespace Editor.Interfaces;

public enum ObjEditingStatus
{
   Unchanged,
   Modified,
}


public abstract class Saveable
{
   private ObjEditingStatus _editingStatus = ObjEditingStatus.Unchanged;
   private PathObj _path = PathObj.Empty;
   public PathObj Path => _path;
   public void SetPath(ref PathObj path) => _path = path;

   public ObjEditingStatus EditingStatus
   {
      get => _editingStatus;
      set
      {
         if (Equals(value, _editingStatus))
            return;
         if (!Equals(value, ObjEditingStatus.Unchanged))
            FileManager.AddToBeHandled(this);
         _editingStatus = value;
      } 
   }

   public abstract string SavingComment();
   public abstract PathObj GetDefaultSavePath();

   public abstract string GetSaveString(int tabs);
}