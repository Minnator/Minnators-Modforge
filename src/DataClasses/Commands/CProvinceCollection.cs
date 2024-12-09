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

   public class CAddToCountryProvinceCollection(ProvinceCollection<Province> newParent, bool add)
      : CAddProvinceCollectionGeneral<Province>(newParent, add)
   {
      public override void Execute() => base.Execute([.. NewComposites.Select(x => x.Key)]);
   }

   public class CAddProvinceCollection<T>(ProvinceCollection<T> newParent, bool add)
      : CAddProvinceCollectionGeneral<T>(newParent, add)
      where T : ProvinceComposite
   {
      public override void Execute()
      {
         if (add)
            base.Execute([new([.. OldParentsNotNull], SaveableOperation.Default), new([NewParent], SaveableOperation.Created)]);
         else
            base.Execute([.. OldParentsNotNull, NewParent], SaveableOperation.Default);
      }
   }

   public class CRemoveCountryProvinceCollection<T>(ProvinceCollection<T> oldParent, bool remove)
      : CRemoveProvinceCollectionGeneral<T>(oldParent, remove)
      where T : ProvinceComposite
   {
      private SaveablesCommandHelper _saveables;

      public override void Execute()
      {
         base.Execute([.. Composites]);
      }

   }

   public class CRemoveProvinceCollection<T>(ProvinceCollection<T> oldParent, bool remove)
      : CRemoveProvinceCollectionGeneral<T>(oldParent, remove)
      where T : ProvinceComposite
   {
      private SaveablesCommandHelper _saveables;

      public override void Execute() => base.Execute([_oldParent], remove ? SaveableOperation.Deleted : SaveableOperation.Default);
      

   }

   public abstract class CAddProvinceCollectionGeneral<T> : SaveableCommand where T : ProvinceComposite
   {
      protected List<ProvinceCollection<T>> OldParentsNotNull = [];
      protected readonly List<KeyValuePair<T, ProvinceCollection<T>>> NewComposites = [];
      protected readonly ProvinceCollection<T> NewParent;
      private readonly bool _addToGlobal;
      public CAddProvinceCollectionGeneral(ProvinceCollection<T> newParent, bool add)
      {
         _addToGlobal = add;
         NewParent = newParent;
      }

      protected override void Execute(List<KeyValuePair<ICollection<Saveable>, SaveableOperation>> actions)
      {
         // No longer needed
         OldParentsNotNull = null!;
         InternalExecute();
         base.Execute(actions);
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
            composite.Parents.Add(NewParent);
            NewParent.InternalAdd(composite);
         }
         NewParent.SetBounds();
         if (_addToGlobal)
         {
            NewParent.AddGlobal();
            NewParent.Invoke(new(ProvinceCollectionType.AddGlobal, [.. NewComposites.Select(x=>x.Key)]));
         }
         else
            NewParent.Invoke(new(ProvinceCollectionType.Add, [.. NewComposites.Select(x => x.Key)]));
      }


      public override void Undo()
      {
         foreach (var (composite, parent) in NewComposites)
         {
            NewParent.InternalRemove(composite);
            if (parent != null!)
            {
               parent.InternalAdd(composite);
               parent.SetBounds();
               composite.Parents.Add(parent);
            }
            composite.Parents.Remove(NewParent);
            
         }
         NewParent.SetBounds();
         if (_addToGlobal)
         {
            NewParent.RemoveGlobal();
            NewParent.Invoke(new(ProvinceCollectionType.RemoveGlobal, [.. NewComposites.Select(x => x.Key)]));
         }
         else
            NewParent.Invoke(new(ProvinceCollectionType.Remove, [.. NewComposites.Select(x => x.Key)]));
         base.Undo();
      }

      public override void Redo()
      {
         InternalExecute();
         base.Redo();
      }

      public override string GetDescription()
      {
         var numbers = NewComposites.Count;
         var objType = NewComposites[0].Key.WhatAmI() + (numbers > 1 ? "s" : "");
         return $"Added {numbers} {objType} to {NewParent.Name}";
      }

      public override string GetDebugInformation(int indent)
      {
         return $"Added {NewComposites.Count} {NewComposites[0].Key.WhatAmI()} to {NewParent.Name}";
      }

      public void AddNewComposite(T composite)
      {
         if (composite.GetFirstParentOfType(NewParent.WhatAmI()) is ProvinceCollection<T> parent)
         {
            if (!OldParentsNotNull.Contains(parent))
               OldParentsNotNull.Add(parent);
         }
         else
            parent = null!;
         NewComposites.Add(new(composite, parent));
      }
   }

   public abstract class CRemoveProvinceCollectionGeneral<T> : SaveableCommand where T : ProvinceComposite
   {
      protected List<T> Composites = [];
      protected ProvinceCollection<T> _oldParent;
      protected bool _removeFromGlobal;

      public CRemoveProvinceCollectionGeneral(ProvinceCollection<T> oldParent, bool remove)
      {
         _oldParent = oldParent;
         _removeFromGlobal = remove;
      }

      protected override void Execute(ICollection<Saveable> saveables, SaveableOperation operation = SaveableOperation.Default)
      {
         InternalExecute();
         base.Execute(saveables, operation);
      }

      private void InternalExecute()
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

      public override void Undo()
      {
         foreach (var composite in Composites)
         {
            _oldParent.InternalAdd(composite);
            composite.Parents.Add(_oldParent);
         }
         if (_removeFromGlobal)
         {
            _oldParent.AddGlobal();
            _oldParent.Invoke(new(ProvinceCollectionType.AddGlobal, Composites));
         }
         else
            _oldParent.Invoke(new(ProvinceCollectionType.Add, Composites));

         base.Undo();
      }

      public override void Redo()
      {
         InternalExecute();
         base.Redo();
      }

      public void RemoveNewComposite(T composite)
      {
         Composites.Add(composite);
      }

      public override string GetDescription()
      {
         var numbers = Composites.Count;
         var objType = Composites.First().WhatAmI() + (numbers > 1 ? "s" : "");
         return $"Removed {numbers} {objType}";
      }

      public override string GetDebugInformation(int indent)
      {
         return $"Removed {Composites.Count} {Composites.First().WhatAmI()}";
      }
   }

}