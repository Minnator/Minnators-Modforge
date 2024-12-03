using Editor.Saving;

namespace Editor.DataClasses.Commands
{
   public class SaveableCommandHelper(Saveable saveable)
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
   }

   public class SaveablesCommandHelper(ICollection<Saveable> saveables)
   {
      // caches the previous state of the object
      private List<ObjEditingStatus> _previousState = [];
      // saves if theis object type is marked as modified
      private bool _flagSet;

      private Saveable test_saveable = saveables.First();

      public virtual void Execute()
      {
         _flagSet = (Globals.SaveableType & test_saveable.WhatAmI()) != 0; 
         foreach (var saveable in saveables) 
            _previousState.Add(saveable.EditingStatus);
         SetAllEditingStates(ObjEditingStatus.Modified);
      }

      private void SetAllPreviousStates(List<ObjEditingStatus> states)
      {
         var cnt = 0;
         foreach (var state in states)
            _previousState[cnt++] = state;
      }

      private void SetAllEditingStates(List<ObjEditingStatus> states)
      {
         var cnt = 0;
         foreach (var saveable in saveables)
            saveable.EditingStatus = states[cnt++];
      }

      private void SetAllEditingStates(ObjEditingStatus state)
      {
         foreach (var saveable in saveables) 
            saveable.EditingStatus = state;
      }
      
      public virtual void Undo()
      {
         List<ObjEditingStatus> states = [..saveables.Select(x => x.EditingStatus)];
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
            SaveMaster.RemoveFromToBeHandled(saveables);
         }
         SetAllPreviousStates(states);
         
      }

      public virtual void Redo()
      {
         List<ObjEditingStatus> states = [.. saveables.Select(x => x.EditingStatus)];
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
            SaveMaster.RemoveFromToBeHandled(saveables);
         }
         SetAllPreviousStates(states);
      }
   }
}