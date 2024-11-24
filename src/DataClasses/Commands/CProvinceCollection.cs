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

   public class CAddProvinceCollection<T>(ProvinceCollection<T> newParent, bool add) : ICommand where T : ProvinceComposite
   {
      private List<T> Composites = [];
      private List<ProvinceCollection<T>> oldParent = [];
      private ProvinceCollection<T> newParent = newParent;
      private bool _addToGlobal = add;
      public CAddProvinceCollection(ProvinceCollection<T> newParent, bool add, bool executeOnInit = false) : this(newParent, add)
      {
         if (executeOnInit)
            Execute();
      }

      public void Execute()
      {
         //Todo fix
         newParent.EditingStatus = ObjEditingStatus.Modified;
         for (var i = 0; i < Composites.Count; i++)
         {
            var composite = Composites[i];
            var parent = oldParent[i];
            if (parent != null!)
            {
               parent.InternalRemove(composite);
               parent.SetBounds();
               composite.Parents.Remove(parent);
            }
            composite.Parents.Add(newParent);
            newParent.InternalAdd(composite);
         }
         newParent.SetBounds();
         if (_addToGlobal)
         {
            newParent.AddGlobal();
            newParent.Invoke(new(ProvinceCollectionType.AddGlobal, Composites));
         }
         else
            newParent.Invoke(new(ProvinceCollectionType.Add, Composites));

      }


      public void Undo()
      {
         for (var i = 0; i < Composites.Count; i++)
         {
            var composite = Composites[i];
            var parent = oldParent[i];
            if (parent != null!)
            {
               parent.InternalAdd(composite);
               parent.SetBounds();
               composite.Parents.Add(parent);
            }
            composite.Parents.Remove(newParent);
            newParent.InternalRemove(composite);
         }
         newParent.SetBounds();
         if (_addToGlobal)
         {
            newParent.RemoveGlobal();
            newParent.Invoke(new(ProvinceCollectionType.RemoveGlobal, Composites));
         }
         else
            newParent.Invoke(new(ProvinceCollectionType.Remove, Composites));
      }

      public void Redo()
      {
         Execute();
      }

      public string GetDescription()
      {
         var numbers = Composites.Count;
         var objType = Composites[0].WhatAmI() + (numbers > 1 ? "s" : "");
         return $"Added {numbers} {objType} to {newParent.Name}";
      }

      public string GetDebugInformation(int indent)
      {
         return $"Added {Composites.Count} {Composites[0].WhatAmI()} to {newParent.Name}";
      }

      public void AddNewComposite(T composite)
      {
         Composites.Add(composite);
         if (composite.GetFirstParentOfType(newParent.WhatAmI()) is not ProvinceCollection<T> parent)
         {
            parent = null!;
         }
         else
         {
            parent.EditingStatus = ObjEditingStatus.Modified;
         }
         oldParent.Add(parent);
      }
   }

   public class CRemoveProvinceCollection<T> : ICommand where T : ProvinceComposite
   {
      private HashSet<T> Composites = [];
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