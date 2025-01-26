using System.Collections;
using System.Reflection;
using Editor.Saving;
using Newtonsoft.Json.Linq;

namespace Editor.DataClasses.Commands
{
   public class CModifyProperty<T> : SaveableCommandBasic
   {
      private T _newValue;
      private readonly List<Saveable> _targets;
      private readonly List<T> _oldValue;
      private readonly PropertyInfo _propInfo;

      public CModifyProperty(PropertyInfo property, List<Saveable> targets, T newValue, out bool change, bool executeOnInit = true)
      {
         _propInfo = property;
         _targets = targets;

         _oldValue = new(targets.Count);
         for (var i = _targets.Count - 1; i >= 0; i--)
         {
            if (_targets[i].GetProperty<T>(property)!.Equals(newValue))
               _targets.RemoveAt(i);
            else
               _oldValue.Insert(0, (T)_propInfo?.GetValue(_targets[i])!); 
         }
         change = _targets.Count > 0;
         _newValue = newValue;

         if (executeOnInit && change)
            Execute();
      }

      public override List<int> GetTargetHash() => [.. _targets.Select(x => x.GetHashCode())];
      public CModifyProperty(PropertyInfo property, Saveable target, T newValue, T oldValue, bool executeOnInit = true)
      {
         _propInfo = property;
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
         Saveable.SetFieldMultipleSilent(_targets, _newValue, _propInfo);
      }

      public override void Undo()
      {
         base.Undo();
         Saveable.SetFieldMultipleDifferentSilent(_targets, _oldValue, _propInfo);
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
         var text = _targets.Count < 5 ? string.Join(", ", _targets) : $"[{_targets.Count}...]";
         return _newValue is not List<string> list ? $"Modify property {_propInfo.Name} of {text} to {_newValue}" : $"Modify property {_propInfo.Name} of {_targets} to {string.Join(", ", GetDiff())}";
      }

      public override string GetDebugInformation(int indent)
      {
         if (_newValue is not IList list)
            return $"Changed {_propInfo.Name} from {_oldValue} to {_newValue} in {_targets.First().WhatAmI()} object ({_targets})";
         return $"Changed {_propInfo.Name} from {_oldValue} to {string.Join(", ", GetDiff())} in {_targets.First().WhatAmI()} object ({_targets})";
      }
   }
}