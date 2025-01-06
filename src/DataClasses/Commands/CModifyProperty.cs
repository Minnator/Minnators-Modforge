using System.Collections;
using System.Reflection;
using Editor.Saving;

namespace Editor.DataClasses.Commands
{
   public class CModifyProperty<T> : SaveableCommandBasic
   {
      private T _newValue;
      private readonly List<Saveable> _targets;
      private readonly List<T> _oldValue;
      private readonly PropertyInfo propInfo;

      public CModifyProperty(PropertyInfo property, List<Saveable> targets, T newValue, bool executeOnInit = true)
      {
         propInfo = property;
         _targets = targets;
         _oldValue = targets.Select(saveable => (T)propInfo?.GetValue(saveable)!).ToList();
         _newValue = newValue;
         if (executeOnInit)
            Execute();
      }

      public CModifyProperty(PropertyInfo property, Saveable target, T newValue, T oldValue, bool executeOnInit = true)
      {
         propInfo = property;
         _targets = [target];
         _oldValue = [oldValue];
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
         base.Execute(_targets);
         InternalExecute();
      }

      private void InternalExecute()
      {
         Saveable.SetFieldMultipleSilent(_targets, _newValue, propInfo);
      }

      public override void Undo()
      {
         base.Undo();
         Saveable.SetFieldMultipleDifferentSilent(_targets, _oldValue, propInfo);
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
         return _newValue is not List<string> list ? $"Modify property {propInfo.Name} of {_targets} to {_newValue}" : $"Modify property {propInfo.Name} of {_targets} to {string.Join(", ", GetDiff())}";
      }

      public override string GetDebugInformation(int indent)
      {
         if (_newValue is not IList list)
            return $"Changed {propInfo.Name} from {_oldValue} to {_newValue} in {_targets.First().WhatAmI()} object ({_targets})";
         return $"Changed {propInfo.Name} from {_oldValue} to {string.Join(", ", GetDiff())} in {_targets.First().WhatAmI()} object ({_targets})";
      }
   }
}