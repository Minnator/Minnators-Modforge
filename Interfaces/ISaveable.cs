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
   public PathObj Path { get; set; } = PathObj.Empty;
   public ObjEditingStatus EditingStatus
   {
      get => _editingStatus;
      set
      {
         if (Equals(value, _editingStatus))
            return;
         FileManager.AddToBeHandled(this);
         _editingStatus = value;
      } 
   }

   public abstract string SavingComment();
   public abstract PathObj GetDefaultSavePath();

   public abstract string GetSaveString(int tabs);
}