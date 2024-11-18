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

      public string GetDescription()
      {
         return $"Modify property {property} of {target} to {newValue}";
      }

      public string GetDebugInformation(int indent)
      {
         return $"Changed {property} from {oldValue} to {newValue} in {target.WhatAmI()} object ({target})";
      }
   }
}