namespace Editor.Interfaces;

public enum ObjEditingStatus
{
   Unchanged,
   Modified,
   New,
}
   
public interface ISaveable
{
   public ObjEditingStatus EditingStatus { get; set; }
   public int FileIndex { get; set; }
}