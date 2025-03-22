using System.Reflection;
using Editor.Saving;

namespace Editor.DataClasses.Commands
{

   public class CModifyPropertyExternal<T, TObject> : SaveableCommandBasic
   {
      private T _newValue;
      private readonly Saveable _target;
      private readonly T _oldValue;
      private readonly PropertyInfo _propInfo;
      private readonly TObject _changedObject;
      public override List<int> GetTargetHash() => [_target.GetHashCode()];
      public CModifyPropertyExternal(PropertyInfo property, Saveable target, TObject changedObject , T newValue, T oldValue, bool executeOnInit = true)
      {
         _propInfo = property;
         _target = target;
         _changedObject = changedObject;
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
         Saveable.SetFieldSilentExternal(_target, _changedObject, _newValue, _propInfo);
      }

      public override void Undo()
      {
         base.Undo();
         Saveable.SetFieldSilentExternal(_target, _changedObject, _oldValue, _propInfo);
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
         return $"Modify property {_propInfo.Name} of {_target} to {string.Join(", ", GetDiff())}";
      }

      public override string GetDebugInformation(int indent)
      {
         return $"Modify property {_propInfo.Name} of {_target} to {string.Join(", ", GetDiff())}";
      }
   }
}