using Editor.Saving;

namespace Editor.DataClasses.Commands
{
   public abstract class CSaveableCommand(Saveable saveable) : ICommand
   {
      // caches the previous state of the object
      private ObjEditingStatus _previousState;
      // saves if theis object type is marked as modified
      private bool _flagSet;

      public virtual void Execute()
      {
         _flagSet = (Globals.SaveableType & saveable.WhatAmI()) != 0;
         _previousState = saveable.EditingStatus;
         saveable.EditingStatus = ObjEditingStatus.Modified;
      }

      public virtual void Undo()
      {
         var state = saveable.EditingStatus;
         if (state != ObjEditingStatus.Modified)
         {
            _flagSet = (Globals.SaveableType & saveable.WhatAmI()) != 0;
            saveable.EditingStatus = ObjEditingStatus.Modified;
         }
         else // default case
         {
            if (!_flagSet)
               Globals.SaveableType &= ~saveable.WhatAmI();
            saveable.EditingStatus = _previousState;
         }
         _previousState = state;
      }

      public virtual void Redo()
      {
         var state = saveable.EditingStatus;
         if (state != ObjEditingStatus.Modified) // default case
         {
            saveable.EditingStatus = ObjEditingStatus.Modified;
         }
         else
         {
            if (!_flagSet)
               Globals.SaveableType &= ~saveable.WhatAmI();
            saveable.EditingStatus = _previousState;
         }
         _previousState = state;
      }

      public abstract string GetDescription();

      public abstract string GetDebugInformation(int indent);
   }

   public abstract class CSaveablesCommand(ICollection<Saveable> saveables) : ICommand
   {
      // caches the previous state of the object
      private ObjEditingStatus _previousState;
      // saves if theis object type is marked as modified
      private bool _flagSet;

      private Saveable test_saveable = saveables.First();

      public virtual void Execute()
      {
         _flagSet = (Globals.SaveableType & test_saveable.WhatAmI()) != 0; 
         _previousState = test_saveable.EditingStatus;
         SetAllEditingStates(ObjEditingStatus.Modified);
      }

      public void SetAllEditingStates(ObjEditingStatus state)
      {
         foreach (var saveable in saveables)
         {
            saveable.EditingStatus = state;
         }
      }

      public virtual void Undo()
      {
         var state = test_saveable.EditingStatus;
         if (state != ObjEditingStatus.Modified)
         {
            _flagSet = (Globals.SaveableType & test_saveable.WhatAmI()) != 0;
            SetAllEditingStates(ObjEditingStatus.Modified);
         }
         else // default case
         {
            if (!_flagSet)
               Globals.SaveableType &= ~test_saveable.WhatAmI();
            SetAllEditingStates(_previousState);
         }
         _previousState = state;
      }

      public virtual void Redo()
      {
         var state = test_saveable.EditingStatus;
         if (state != ObjEditingStatus.Modified) // default case
         {
            SetAllEditingStates(ObjEditingStatus.Modified);
         }
         else
         {
            if (!_flagSet)
               Globals.SaveableType &= ~test_saveable.WhatAmI();
            SetAllEditingStates(_previousState);
         }
         _previousState = state;
      }

      public abstract string GetDescription();

      public abstract string GetDebugInformation(int indent);
   }
}