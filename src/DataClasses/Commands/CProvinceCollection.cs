using Editor.DataClasses.GameDataClasses;
using Editor.Saving;

namespace Editor.DataClasses.Commands
{

   public enum ProvinceCollectionType
   {
      Add,
      Remove,
      AddGlobal,
      RemoveGlobal,
   }

   public class CAddProvinceCollection<T> : ICommand where T : ProvinceComposite
   {
      private List<ProvinceCollection<T>> _oldParentsNotNull = [];
      private List<KeyValuePair<T, ProvinceCollection<T>>> NewComposites = [];
      private SaveablesCommandHelper _saveables;
      private ProvinceCollection<T> _newParent;
      private bool _addToGlobal;
      public CAddProvinceCollection(ProvinceCollection<T> newParent, bool add, bool executeOnInit = false)
      {
         _addToGlobal = add;
         _newParent = newParent;
         if (executeOnInit)
            Execute();
      }

      public void Execute()
      {
         _saveables = new([.. _oldParentsNotNull, _newParent]);
         // No longer needed
         _oldParentsNotNull = null;
         _saveables.Execute();
         InternalExecute();
      }

      private void InternalExecute()
      {
         foreach (var (composite, parent) in NewComposites)
         {
            if (parent != null!)
            {
               parent.InternalRemove(composite);
               parent.SetBounds();
               composite.Parents.Remove(parent);
            }
            composite.Parents.Add(_newParent);
            _newParent.InternalAdd(composite);
         }
         _newParent.SetBounds();
         if (_addToGlobal)
         {
            _newParent.AddGlobal();
            _newParent.Invoke(new(ProvinceCollectionType.AddGlobal, [.. NewComposites.Select(x=>x.Key)]));
         }
         else
            _newParent.Invoke(new(ProvinceCollectionType.Add, [.. NewComposites.Select(x => x.Key)]));
      }


      public void Undo()
      {
         _saveables.Undo();
         foreach (var (composite, parent) in NewComposites)
         {
            if (parent != null!)
            {
               parent.InternalAdd(composite);
               parent.SetBounds();
               composite.Parents.Add(parent);
            }
            composite.Parents.Remove(_newParent);
            _newParent.InternalRemove(composite);
         }
         _newParent.SetBounds();
         if (_addToGlobal)
         {
            _newParent.RemoveGlobal();
            _newParent.Invoke(new(ProvinceCollectionType.RemoveGlobal, [.. NewComposites.Select(x => x.Key)]));
         }
         else
            _newParent.Invoke(new(ProvinceCollectionType.Remove, [.. NewComposites.Select(x => x.Key)]));
      }

      public void Redo()
      {
         _saveables.Redo();
         InternalExecute();
      }

      public string GetDescription()
      {
         var numbers = NewComposites.Count;
         var objType = NewComposites[0].Key.WhatAmI() + (numbers > 1 ? "s" : "");
         return $"Added {numbers} {objType} to {_newParent.Name}";
      }

      public string GetDebugInformation(int indent)
      {
         return $"Added {NewComposites.Count} {NewComposites[0].Key.WhatAmI()} to {_newParent.Name}";
      }

      public void AddNewComposite(T composite)
      {
         if (composite.GetFirstParentOfType(_newParent.WhatAmI()) is ProvinceCollection<T> parent)
            _oldParentsNotNull.Add(parent);
         else
            parent = null!;
         NewComposites.Add(new(composite, parent));
      }
   }

   public class CRemoveProvinceCollection<T> : ICommand where T : ProvinceComposite
   {
      private List<T> Composites = [];
      private SaveableCommandHelper _oldParentSaveable;
      private ProvinceCollection<T> _oldParent;
      private bool _removeFromGlobal;

      public CRemoveProvinceCollection(ProvinceCollection<T> oldParent, bool remove)
      {
         _oldParent = oldParent;
         _oldParentSaveable = new(oldParent);
         _removeFromGlobal = remove;
      }

      public void Execute()
      {
         _oldParentSaveable.Execute();
         internalExecute();
      }

      private void internalExecute()
      {
         foreach (var composite in Composites)
         {
            _oldParent.InternalRemove(composite);
            composite.Parents.Remove(_oldParent);
         }
         _oldParent.SetBounds();
         if (_removeFromGlobal)
         {
            _oldParent.RemoveGlobal();
            _oldParent.Invoke(new(ProvinceCollectionType.RemoveGlobal, Composites));
         }
         _oldParent.Invoke(new(ProvinceCollectionType.Remove, Composites));
      }

      public void Undo()
      {
         _oldParentSaveable.Undo();
         foreach (var composite in Composites)
         {
            _oldParent.InternalAdd(composite);
            composite.Parents.Add(_oldParent);
         }
         if (_removeFromGlobal)
         {
            _oldParent.RemoveGlobal();
            _oldParent.Invoke(new(ProvinceCollectionType.RemoveGlobal, Composites));
         }
         else
            _oldParent.Invoke(new(ProvinceCollectionType.Add, Composites));
         
         
      }

      public void Redo()
      {
         _oldParentSaveable.Redo();
         internalExecute();
      }

      public void RemoveNewComposite(T composite)
      {
         Composites.Add(composite);
      }

      public string GetDescription()
      {
         var numbers = Composites.Count;
         var objType = Composites.First().WhatAmI() + (numbers > 1 ? "s" : "");
         return $"Removed {numbers} {objType}";
      }

      public string GetDebugInformation(int indent)
      {
         return $"Removed {Composites.Count} {Composites.First().WhatAmI()}";
      }
   }

}