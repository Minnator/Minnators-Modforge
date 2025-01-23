﻿using Editor.Helper;
using Editor.Saving;

namespace Editor.DataClasses.Commands
{
   public class CAdvancedPropertiesEditing(object preEditing, object postEditing) : ICommand
   {
      public void Execute()
      {
         // Don't do anything here, as the object is edited by the PropertyGrid directly
      }

      public void Undo()
      {
         ProvColHelper.SetObjectInCollectionIfExists(preEditing);
      }

      public void Redo()
      {
         ProvColHelper.SetObjectInCollectionIfExists(postEditing);
      }

      public List<Saveable> GetTargets() => [];

      public string GetDescription()
      {
         return $"Modified {Convert.ChangeType(preEditing, preEditing.GetType())} in advanced Prop Editor";
      }

      public string GetDebugInformation(int indent)
      {
         return $"Modified {Convert.ChangeType(preEditing, preEditing.GetType())} in advanced Prop Editor";
      }
   }
}