using System.Collections;
using Editor.Saving;

namespace Editor.DataClasses.Commands
{
   public class CModifyProperty<T> : SaveableCommandBasic
   {
      private T _newValue;
      private readonly string _property;
      private readonly Saveable _target;
      private readonly T _oldValue;

      public CModifyProperty(string property,
         Saveable target,
         T newValue,
         T oldValue,
         bool executeOnInit = true)
      {
         _property = property;
         _target = target;
         _oldValue = oldValue;
         _newValue = newValue;
         if (executeOnInit)
            Execute();
      }

      protected void SetValue(T value)
      {
         _newValue = value;
      }

      public override void Execute()
      {
         base.Execute([_target]);
         InternalExecute();
      }

      private void InternalExecute()
      {
         _target.SetFieldSilent(_property, _newValue);
      }

      public override void Undo()
      {
         base.Undo();
         _target.SetFieldSilent(_property, _oldValue);
      }

      public override void Redo()
      {
         base.Redo();
         InternalExecute();
      }


      private List<string> GetDiff()
      {
         if (_newValue is not List<string> list || _oldValue is not List<string> old)
            return [];
         return list.Except(old).ToList();
      }

      public override string GetDescription()
      {
         return _newValue is not List<string> list ? $"Modify property {_property} of {_target} to {_newValue}" : $"Modify property {_property} of {_target} to {string.Join(", ", GetDiff())}";
      }

      public override string GetDebugInformation(int indent)
      {
         if (_newValue is not IList list)
            return $"Changed {_property} from {_oldValue} to {_newValue} in {_target.WhatAmI()} object ({_target})";
         return $"Changed {_property} from {_oldValue} to {string.Join(", ", GetDiff())} in {_target.WhatAmI()} object ({_target})";
      }
   }
}