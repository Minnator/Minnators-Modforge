using Editor.Saving;
using System.Reflection;
using Editor.DataClasses.Saveables;

namespace Editor.DataClasses.Commands
{
   public class PrvHistoryEntryCommand<T> : SaveableCommandBasic
   {


      private T _newValue;
      private readonly List<Province> _targets;
      private readonly List<T> _oldValues;
      private readonly PropertyInfo _propInfo;

      public PrvHistoryEntryCommand(PropertyInfo property, List<Province> targets, T newValue, out bool change, bool executeOnInit = true)
      {
         _propInfo = property;
         _targets = targets;

         _oldValues = new(targets.Count);
         for (var i = _targets.Count - 1; i >= 0; i--)
         {
            if (_targets[i].GetProperty<T>(property)!.Equals(newValue))
               _targets.RemoveAt(i);
            else
               _oldValues.Insert(0, (T)_propInfo?.GetValue(_targets[i])!);
         }
         change = _targets.Count > 0;
         _newValue = newValue;

         if (executeOnInit && change)
            Execute();
      }


      public sealed override void Execute()
      {
         throw new NotImplementedException();
      }

      public override List<int> GetTargetHash()
      {
         throw new NotImplementedException();
      }

      public override string GetDescription()
      {
         throw new NotImplementedException();
      }

      public override string GetDebugInformation(int indent)
      {
         throw new NotImplementedException();
      }
   }
}