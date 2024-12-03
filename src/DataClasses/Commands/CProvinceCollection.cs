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

   public class CAddToCountryProvinceCollection : CAddProvinceCollectionGeneral<Province>
   {
      private SaveablesCommandHelper _saveables;

      public CAddToCountryProvinceCollection(ProvinceCollection<Province> newParent, bool add, bool executeOnInit = false) : base(newParent, add, executeOnInit)
      {
      }

      public override void Execute()
      {
         _saveables = new([.. NewComposites.Select(x => x.Key)]);
         _saveables.Execute();
         base.Execute();
      }

      public override void Undo()
      {
         _saveables.Undo();
         base.Undo();
      }

      public override void Redo()
      {
         _saveables.Redo();
         base.Redo();
      }
   }

   public class CAddProvinceCollection<T> : CAddProvinceCollectionGeneral<T> where T : ProvinceComposite
   {
      private SaveablesCommandHelper _saveables;

      public CAddProvinceCollection(ProvinceCollection<T> newParent, bool add, bool executeOnInit = false) : base(newParent, add, executeOnInit)
      {

      }

      public override void Execute()
      {
         _saveables = new([.. OldParentsNotNull, NewParent]);
         _saveables.Execute();
         base.Execute();
      }

      public override void Undo()
      {
         _saveables.Undo();
         base.Undo();
      }

      public override void Redo()
      {
         _saveables.Redo();
         base.Redo();
      }
   }

   public class CRemoveCountryProvinceCollection<T> : CRemoveProvinceCollectionGeneral<T> where T : ProvinceComposite
   {
      private SaveablesCommandHelper _saveables;
      public CRemoveCountryProvinceCollection(ProvinceCollection<T> oldParent, bool remove) : base(oldParent, remove)
      {
      }

      public override void Execute()
      {
         _saveables = new([.. Composites]);
         _saveables.Execute();
         base.Execute();
      }

      public override void Undo()
      {
         _saveables.Undo();
         base.Undo();
      }

      public override void Redo()
      {
         _saveables.Redo();
         base.Redo();
      }
   }

   public class CRemoveProvinceCollection<T> : CRemoveProvinceCollectionGeneral<T> where T : ProvinceComposite
   {
      private SaveablesCommandHelper _saveables;

      public CRemoveProvinceCollection(ProvinceCollection<T> oldParent, bool remove) : base(oldParent, remove)
      {
      }

      public override void Execute()
      {
         _saveables = new([.. Composites]);
         _saveables.Execute();
         base.Execute();
      }

      public override void Undo()
      {
         _saveables.Undo();
         base.Undo();
      }

      public override void Redo()
      {
         _saveables.Redo();
         base.Redo();
      }
   }

   public abstract class CAddProvinceCollectionGeneral<T> : ICommand where T : ProvinceComposite
   {
      protected List<ProvinceCollection<T>> OldParentsNotNull = [];
      protected readonly List<KeyValuePair<T, ProvinceCollection<T>>> NewComposites = [];
      protected readonly ProvinceCollection<T> NewParent;
      private readonly bool _addToGlobal;
      public CAddProvinceCollectionGeneral(ProvinceCollection<T> newParent, bool add, bool executeOnInit = false)
      {
         _addToGlobal = add;
         NewParent = newParent;
         if (executeOnInit)
            Execute();
      }

      public virtual void Execute()
      {
         // No longer needed
         OldParentsNotNull = null!;
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


      public virtual void Undo()
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
      }

      public virtual void Redo()
      {
         InternalExecute();
      }

      public string GetDescription()
      {
         var numbers = NewComposites.Count;
         var objType = NewComposites[0].Key.WhatAmI() + (numbers > 1 ? "s" : "");
         return $"Added {numbers} {objType} to {NewParent.Name}";
      }

      public string GetDebugInformation(int indent)
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

   public abstract class CRemoveProvinceCollectionGeneral<T> : ICommand where T : ProvinceComposite
   {
      protected List<T> Composites = [];
      protected ProvinceCollection<T> _oldParent;
      protected bool _removeFromGlobal;

      public CRemoveProvinceCollectionGeneral(ProvinceCollection<T> oldParent, bool remove)
      {
         _oldParent = oldParent;
         _removeFromGlobal = remove;
      }

      public virtual void Execute()
      {
         InternalExecute();
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

      public virtual void Undo()
      {
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

      public virtual void Redo()
      {
         InternalExecute();
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