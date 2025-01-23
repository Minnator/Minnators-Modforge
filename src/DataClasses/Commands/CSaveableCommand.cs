using Editor.Saving;

namespace Editor.DataClasses.Commands
{

   public abstract class SaveableCommand : ICommand
   {
      public enum SaveableOperation
      {
         Default,
         Deleted,
         Created
      }

      #region Static Saveable Command

      static SaveableCommand()
      {
         SaveMaster.Saving += (_, _) => SaveTest();
      }

      public abstract List<Saveable> GetTargets();

      protected static int globalState = 0;

      public static void SaveTest()
      {
         globalState++;
      }

      #endregion

      protected readonly List<ObjEditingStatus> _previousState = [];
      protected int _internalState = globalState;


      public abstract void Execute();
      public virtual void Undo()
      {
         ReasignStates(false);
      }

      public virtual void Redo()
      {
         ReasignStates(true);
      }
      public abstract string GetDescription();
      public abstract string GetDebugInformation(int indent);

      protected abstract void ReasignStates(bool forwards);

      protected ObjEditingStatus GetEditingState(bool forwards, SaveableOperation operation)
      {
         return operation switch
         {
            SaveableOperation.Default => ObjEditingStatus.Modified,
            SaveableOperation.Deleted => forwards ? ObjEditingStatus.ToBeDeleted : ObjEditingStatus.Modified,
            SaveableOperation.Created => forwards ? ObjEditingStatus.Modified : ObjEditingStatus.ToBeDeleted,
            _ => throw new ArgumentOutOfRangeException(nameof(operation), "Help.")
         };
      }

      protected void SetAllPreviousStates(List<ObjEditingStatus> states)
      {
         var cnt = 0;
         foreach (var state in states)
            _previousState[cnt++] = state;
      }

      protected void SetAllEditingStates(ICollection<Saveable> saveables, List<ObjEditingStatus> states)
      {
         var cnt = 0;
         foreach (var saveable in saveables)
            saveable.EditingStatus = states[cnt++];
      }

      protected void SetAllEditingStates(ICollection<Saveable> saveables, ObjEditingStatus state)
      {
         foreach (var saveable in saveables)
            saveable.EditingStatus = state;
      }
   }

   public abstract class SaveableCommandComplex : SaveableCommand
   {
      private List<KeyValuePair<ICollection<Saveable>, SaveableOperation>> _actions = [];

      public IEnumerable<Saveable> GetAllSaveables()
      {
         foreach (var kvp in _actions)
         foreach (var saveable in kvp.Key)
            yield return saveable;
      }

      public IEnumerable<KeyValuePair<Saveable, SaveableOperation>> GetAllSaveablesWithOperation()
      {
         foreach (var kvp in _actions)
         foreach (var saveable in kvp.Key)
            yield return new(saveable, kvp.Value);
      }

      #region ICommand implementation

      protected virtual void Execute(ICollection<Saveable> saveables,
         SaveableOperation operation = SaveableOperation.Default)
      {
         Execute([new([.. saveables], operation)]);
      }

      protected virtual void Execute(List<KeyValuePair<ICollection<Saveable>, SaveableOperation>> actions)
      {
         this._actions = actions;
         //Needs to be identical to the GetAllSaveables Loop
         foreach (var kvp in _actions)
            //TODO Remove the test when the saveable constructor is initialised with to be deleted
            if (kvp.Value != SaveableOperation.Created)
               foreach (var saveable in kvp.Key)
                  _previousState.Add(saveable.EditingStatus);
            else
               foreach (var saveable in kvp.Key)
                  _previousState.Add(ObjEditingStatus.Deleted);

         foreach (var action in _actions)
            SetAllEditingStates(action.Key, GetEditingState(true, action.Value));
      }

      #endregion

      #region Helper methods

      protected override void ReasignStates(bool forwards)
      {
         List<ObjEditingStatus> states = [.. GetAllSaveables().Select(x => x.EditingStatus)];
         foreach (var action in _actions)
         {
            if (_internalState != globalState)
               SetAllEditingStates(action.Key, GetEditingState(forwards, action.Value));
            else
               SetAllEditingStates(action.Key, _previousState);
         }

         SetAllPreviousStates(states);
         _internalState = globalState;
      }

      #endregion
   }

   public abstract class SaveableCommandBasic : SaveableCommand
   {
      private ICollection<Saveable> _saveables = [];
      private SaveableOperation _operation;


      #region ICommand implementation

      protected virtual void Execute(ICollection<Saveable> saveables,
         SaveableOperation operation = SaveableOperation.Default)
      {

         _saveables = saveables;
         _operation = operation;

         //TODO Remove the test when the saveable constructor is initialised with to be deleted
         if (operation != SaveableOperation.Created)
            foreach (var saveable in _saveables)
               _previousState.Add(saveable.EditingStatus);
         else
            foreach (var saveable in _saveables)
               _previousState.Add(ObjEditingStatus.Deleted);

         SetAllEditingStates(_saveables, GetEditingState(true, _operation));

      }
      #endregion

      #region Helper methods

      protected override void ReasignStates(bool forwards)
      {
         List<ObjEditingStatus> states = [.. _saveables.Select(x => x.EditingStatus)];

         if (_internalState != globalState)
            SetAllEditingStates(_saveables, GetEditingState(forwards, _operation));
         else
            SetAllEditingStates(_saveables, _previousState);

         SetAllPreviousStates(states);
         _internalState = globalState;
      }

      #endregion
   }
}