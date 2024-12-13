using System.Collections;
using Editor.Saving;

namespace Editor.DataClasses.Commands
{
   public class CModifyProperty<T>(
      string? property,
      Saveable target,
      T newValue,
      T oldValue,
      ObjEditingStatus editingStatus)
      : ICommand
   {
      public void Execute()
      {
         SetProperty(newValue);
         target.EditingStatus = ObjEditingStatus.Modified;
      }

      public void Undo()
      {
         SetProperty(oldValue);
         target.EditingStatus = editingStatus;
      }

      public void Redo()
      {  
         Execute();
      }

      private void SetProperty(T value)
      {
         var property1 = target.GetType().GetProperty(property);
         if (property1 != null && property1.CanWrite) 
            property1.SetValue(target, value);
      }

      private List<string> GetDiff()
      {
         if (newValue is not List<string> list || oldValue is not List<string> old)
            return [];
         return list.Except(old).ToList();
      }

      public string GetDescription()
      {
         return newValue is not List<string> list ? $"Modify property {property} of {target} to {newValue}" : $"Modify property {property} of {target} to {string.Join(", ", GetDiff())}";
      }

      public string GetDebugInformation(int indent)
      {
         if (newValue is not IList list)
            return $"Changed {property} from {oldValue} to {newValue} in {target.WhatAmI()} object ({target})";
         return $"Changed {property} from {oldValue} to {string.Join(", ", GetDiff())} in {target.WhatAmI()} object ({target})";
      }
   }
}