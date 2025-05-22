using System.Collections;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using Editor.Events;
using Editor.Saving;

namespace Editor.DataClasses.Commands
{
   public class CModifyListProperty<TSaveable, TCollection, TValue> : SaveableCommandBasic where TCollection : ICollection<TValue> where TSaveable: Saveable
   {
      // Simple Add
      // Simple Remove
      // Complex Add (Add but remove something else e.g. provinceCollection)
      // Add to globals and so on but again provinceCollection

      private readonly TSaveable[] _targets;
      private readonly TValue[][] _deltaValuesRemove;
      private readonly TValue[][] _deltaValuesAdd;
      private readonly PropertyInfo _property;

      public CModifyListProperty(PropertyInfo property, List<TSaveable> targets, HashSet<TValue> deltaAdd, HashSet<TValue> deltaRemove, out bool change, bool executeOnInit = true)
      {
         change = false;
         _targets = new TSaveable[targets.Count];
         _deltaValuesAdd = new TValue[targets.Count][];
         _deltaValuesRemove = new TValue[targets.Count][];
         for (var i = 0; i < targets.Count; i++)
         {
            _targets[i] = targets[i];
            var collection = (TCollection)property.GetValue(targets[i])!;
            _deltaValuesAdd[i] = deltaAdd.Except(collection).ToArray();
            _deltaValuesRemove[i] = collection.Intersect(deltaRemove).ToArray();
            if(!change && (_deltaValuesAdd[i].Length != 0 
               || _deltaValuesRemove[i].Length != 0))
               change = true;
         }
         _property = property;
         //TODO in the future saveables without change shouldnt be entered into the SaveableCommand, since they did not change

         if (executeOnInit && change)
            Execute();
      }

      public override List<int> GetTargetHash() => [.. _targets.Select(x => x.GetHashCode())];

      public void InternalExecute()
      {
         for (var i = 0; i < _targets.Length; i++)
         {
            //TODO Cache Collections
            var collection = (TCollection)_property.GetValue(_targets[i])!;
            var temp = _deltaValuesAdd[i];
            for (var j = 0; j < temp.Length; j++)
               collection.Add(temp[j]);
            temp = _deltaValuesRemove[i];
            for (var j = 0; j < temp.Length; j++)
               collection.Remove(temp[j]);
         }
         LoadGuiEvents.TriggerGuiUpdate(typeof(TSaveable), _property);
      }

      public override void Undo()
      {
         base.Undo();
         for (var i = 0; i < _targets.Length; i++)
         {
            var collection = (TCollection)_property.GetValue(_targets[i])!;
            var temp = _deltaValuesAdd[i];
            for (var j = 0; j < temp.Length; j++)
               collection.Remove(temp[j]);
            temp = _deltaValuesRemove[i];
            for (var j = 0; j < temp.Length; j++)
               collection.Add(temp[j]);
         }
         LoadGuiEvents.TriggerGuiUpdate(_property.DeclaringType, _property);
      }

      public override void Redo()
      {
         base.Redo();
         InternalExecute();
      }

      public override void Execute()
      {
         base.Execute(_targets);
         InternalExecute();
      }

      // returns a string which has information of what has changed
      public override string GetDescription()
      {
         return "Did some modifications TODO";
      }

      public override string GetDebugInformation(int indent)
      {
         return "Did some modifications TODO";
      }
   }


   public class CInsertInListProperty<TSaveable, TCollection, TValue> : SaveableCommandBasic where TCollection : List<TValue> where TSaveable : Saveable
   {
      // Simple Insert
      // Complex Insert into multiple targets at different positions

      private readonly TSaveable[] _targets;
      private readonly PropertyInfo _property;
      private readonly int[] _indices;
      private readonly TValue _value;


      public CInsertInListProperty(PropertyInfo property, List<TSaveable> targets, TValue value, List<int> indices, bool executeOnInit = true)
      {
         Debug.Assert(indices.Count == targets.Count, "indices.Count == targets.Count");

         _value = value;
         _targets = targets.ToArray();
         _indices = indices.ToArray();
         _property = property;

         if (executeOnInit)
            Execute();
      }

      public override List<int> GetTargetHash() => [.. _targets.Select(x => x.GetHashCode())];

      public void InternalExecute()
      {
         for (var i = 0; i < _targets.Length; i++)
         {
            //TODO Cache Collections
            var collection = (TCollection)_property.GetValue(_targets[i])!;
            collection.Insert(_indices[i], _value);
         }
         LoadGuiEvents.TriggerGuiUpdate(typeof(TSaveable), _property);
      }

      public override void Undo()
      {
         base.Undo();
         for (var i = 0; i < _targets.Length; i++)
         {
            var collection = (TCollection)_property.GetValue(_targets[i])!;

            var index = _indices[i];

            Debug.Assert(ReferenceEquals(collection[index], _value), "idk if this works with strings");

            collection.RemoveAt(_indices[i]);
         }
         LoadGuiEvents.TriggerGuiUpdate(_property.DeclaringType, _property);
      }

      public override void Redo()
      {
         base.Redo();
         InternalExecute();
      }

      public override void Execute()
      {
         base.Execute(_targets);
         InternalExecute();
      }

      // returns a string which has information of what has changed
      public override string GetDescription()
      {
         if (_indices.Length == 1)
            return $"Inserted 1 item at '{_indices[0]}' for {_property.Name}";
         return $"Inserted {_indices.Length} items into {_indices.Length} objects for {_property.Name}";
      }

      public override string GetDebugInformation(int indent)
      {
         var sb = new StringBuilder();
         sb.Append(' ', indent);
         sb.Append($"Modified {_property.Name}:");
         sb.AppendLine();
         foreach (var index in _indices) 
            sb.AppendLine($"Inserted {_value} at {index}");

         return sb.ToString();
      }
   }

   public class CRemoveInListProperty<TSaveable, TCollection, TValue> : SaveableCommandBasic where TCollection : List<TValue> where TSaveable : Saveable
   {
      // Simple Insert
      // Complex Insert into multiple targets at different positions

      private readonly TSaveable[] _targets;
      private readonly PropertyInfo _property;
      private readonly int[] _indices;
      private readonly TValue[] _value;


      public CRemoveInListProperty(PropertyInfo property, List<TSaveable> targets, List<int> indices, bool executeOnInit = true)
      {
         Debug.Assert(indices.Count == targets.Count, "indices.Count == targets.Count");

         _targets = targets.ToArray();
         _indices = indices.ToArray();
         _property = property;

         _value = new TValue[targets.Count];
         for (var i = 0; i < _targets.Length; i++)
         {
            var collection = (TCollection)_property.GetValue(_targets[i])!;
            _value[i] = collection[_indices[i]];
         }

         if (executeOnInit)
            Execute();
      }

      public override List<int> GetTargetHash() => [.. _targets.Select(x => x.GetHashCode())];

      public void InternalExecute()
      {
         for (var i = 0; i < _targets.Length; i++)
         {
            //TODO Cache Collections
            var collection = (TCollection)_property.GetValue(_targets[i])!;
            var index = _indices[i];
            Debug.Assert(ReferenceEquals(collection[index], _value[i]), "idk if this works with strings");
            collection.RemoveAt(_indices[i]);
         }
         LoadGuiEvents.TriggerGuiUpdate(typeof(TSaveable), _property);
      }

      public override void Undo()
      {
         base.Undo();
         for (var i = 0; i < _targets.Length; i++)
         {
            var collection = (TCollection)_property.GetValue(_targets[i])!;
            collection.Insert(_indices[i], _value[i]);
         }
         LoadGuiEvents.TriggerGuiUpdate(_property.DeclaringType, _property);
      }

      public override void Redo()
      {
         base.Redo();
         InternalExecute();
      }

      public override void Execute()
      {
         base.Execute(_targets);
         InternalExecute();
      }
      
      // returns a string which has information of what has changed
      public override string GetDescription()
      {
         if (_indices.Length == 1)
            return $"Removed 1 item at '{_indices[0]}' for {_property.Name}";
         return $"Removed {_indices.Length} items into {_indices.Length} objects for {_property.Name}";
      }

      public override string GetDebugInformation(int indent)
      {
         var sb = new StringBuilder();
         sb.Append(' ', indent);
         sb.Append($"Modified {_property.Name}:");
         sb.AppendLine();
         foreach (var index in _indices)
            sb.AppendLine($"Removed {_value} at {index}");

         return sb.ToString();
      }
   }

   public class CRemoveInNestedListProperty<TSaveable, TOuterItem, TCollection, TValue> : SaveableCommandBasic
   where TCollection : List<TValue>
   where TSaveable : Saveable
   {
      private readonly TSaveable[] _targets;
      private readonly PropertyInfo _outerListProperty; // e.g. Entries
      private readonly PropertyInfo _innerListProperty; // e.g. Tokens
      private readonly int[] _outerIndices;
      private readonly int[] _innerIndices;
      private readonly TValue[] _value;

      public CRemoveInNestedListProperty(
         PropertyInfo outerListProperty,
         PropertyInfo innerListProperty,
         List<TSaveable> targets,
         List<int> outerIndices,
         List<int> innerIndices,
         bool executeOnInit = true)
      {
         Debug.Assert(outerIndices.Count == targets.Count, "outerIndices.Count == targets.Count");
         Debug.Assert(innerIndices.Count == targets.Count, "innerIndices.Count == targets.Count");

         _targets = targets.ToArray();
         _outerIndices = outerIndices.ToArray();
         _innerIndices = innerIndices.ToArray();
         _outerListProperty = outerListProperty;
         _innerListProperty = innerListProperty;

         _value = new TValue[targets.Count];
         for (var i = 0; i < _targets.Length; i++)
         {
            var outerList = (IList)_outerListProperty.GetValue(_targets[i])!;
            var outerItem = outerList[_outerIndices[i]];
            var innerList = (TCollection)_innerListProperty.GetValue(outerItem)!;
            _value[i] = innerList[_innerIndices[i]];
         }

         if (executeOnInit)
            Execute();
      }

      public override List<int> GetTargetHash() => [.. _targets.Select(x => x.GetHashCode())];

      private TCollection GetInnerList(int i)
      {
         var outerList = (IList)_outerListProperty.GetValue(_targets[i])!;
         var outerItem = outerList[_outerIndices[i]];
         return (TCollection)_innerListProperty.GetValue(outerItem)!;
      }

      public void InternalExecute()
      {
         for (var i = 0; i < _targets.Length; i++)
         {
            var list = GetInnerList(i);
            var index = _innerIndices[i];
            Debug.Assert(ReferenceEquals(list[index], _value[i]), "Mismatch at removal");
            list.RemoveAt(index);
         }

         LoadGuiEvents.TriggerGuiUpdate(typeof(TSaveable), _innerListProperty);
      }

      public override void Undo()
      {
         base.Undo();
         for (var i = 0; i < _targets.Length; i++)
         {
            var list = GetInnerList(i);
            list.Insert(_innerIndices[i], _value[i]);
         }

         LoadGuiEvents.TriggerGuiUpdate(_innerListProperty.DeclaringType, _innerListProperty);
      }

      public override void Redo()
      {
         base.Redo();
         InternalExecute();
      }

      public override void Execute()
      {
         base.Execute(_targets);
         InternalExecute();

         LoadGuiEvents.TriggerGuiUpdate(typeof(TSaveable), typeof(TSaveable).GetProperty(_outerListProperty.Name)!);
      }

      public override string GetDescription()
      {
         if (_innerIndices.Length == 1)
            return $"Removed 1 item at '{_innerIndices[0]}' in nested property '{_innerListProperty.Name}'";
         return $"Removed {_innerIndices.Length} items in nested property '{_innerListProperty.Name}'";
      }

      public override string GetDebugInformation(int indent)
      {
         var sb = new StringBuilder();
         sb.Append(' ', indent);
         sb.Append($"Modified {_innerListProperty.Name}:");
         sb.AppendLine();
         for (var i = 0; i < _value.Length; i++)
            sb.AppendLine($"Removed {_value[i]} at [{_outerIndices[i]}][{_innerIndices[i]}]");
         return sb.ToString();
      }
   }

}