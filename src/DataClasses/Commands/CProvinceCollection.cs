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

      // TODO if parent does not have any left remove it from globals
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
         //Todo fix
         parent.EditingStatus = ObjEditingStatus.Modified;
         oldParent.Add(parent);
      }
   }

   public class CRemoveProvinceCollection<T>(ProvinceCollection<T> oldParent, bool remove) : ICommand where T : ProvinceComposite
   {
      private List<T> Composites = [];
      private ProvinceCollection<T> oldParent = oldParent;
      private bool _removeFromGlobal = remove;
      private ObjEditingStatus previous_state;
      private bool flag_set;
      public CRemoveProvinceCollection(ProvinceCollection<T> oldParent, bool remove, bool executeOnInit = false) : this(oldParent, remove)
      {
         if (executeOnInit)
            Execute();
      }

      public void Execute()
      {
         for (var i = 0; i < Composites.Count; i++)
         {
            var composite = Composites[i];
            oldParent.InternalRemove(composite);
            composite.Parents.Remove(oldParent);
         }
         oldParent.SetBounds();
         if (_removeFromGlobal)
         {
            oldParent.RemoveGlobal();
            oldParent.Invoke(new(ProvinceCollectionType.RemoveGlobal, Composites));
         }
         else
            oldParent.Invoke(new(ProvinceCollectionType.Remove, Composites));
         previous_state = oldParent.EditingStatus;
         flag_set = (Globals.SaveableType & oldParent.WhatAmI()) != 0;
         oldParent.EditingStatus = ObjEditingStatus.Deleted;
      }


      public void Undo()
      {
         for (var i = 0; i < Composites.Count; i++)
         {
            var composite = Composites[i];
            oldParent.InternalAdd(composite);
            composite.Parents.Add(oldParent);
         }
         oldParent.SetBounds();
         if (_removeFromGlobal)
         {
            oldParent.AddGlobal();
            oldParent.Invoke(new(ProvinceCollectionType.AddGlobal, Composites));
         }
         else
            oldParent.Invoke(new(ProvinceCollectionType.Add, Composites));
         if (oldParent.EditingStatus == ObjEditingStatus.Deleted)
         {
            oldParent.EditingStatus = previous_state;
            if (!flag_set)
               Globals.SaveableType &= ~oldParent.WhatAmI();
         }
         else
            oldParent.EditingStatus = ObjEditingStatus.Modified;
      }

      public void Redo()
      {
         Execute();
      }

      public string GetDescription()
      {
         var numbers = Composites.Count;
         var objType = Composites[0].WhatAmI() + (numbers > 1 ? "s" : "");
         return $"Removed {numbers} {objType}";
      }

      public string GetDebugInformation(int indent)
      {
         return $"Removed {Composites.Count} {Composites[0].WhatAmI()}";
      }

      public void RemoveNewComposite(T composite)
      {
         Composites.Add(composite);
      }
   }

}