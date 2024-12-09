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

      protected static int globalState = 0;

      public static void SaveTest()
      {
         globalState++;
      }

      #endregion

      private readonly List<ObjEditingStatus> _previousState = [];
      private int _internalState = 0;
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

      public abstract void Execute();

      protected virtual void Execute(ICollection<Saveable> saveables, SaveableOperation operation = SaveableOperation.Default)
      {
         Execute([new([.. saveables], operation)]);
      }

      protected virtual void Execute(List<KeyValuePair<ICollection<Saveable>, SaveableOperation>> actions)
      {
         this._actions = actions;
         foreach (var saveable in GetAllSaveables())
            _previousState.Add(saveable.EditingStatus);
         foreach (var action in _actions)
            SetAllEditingStates(action.Key, GetEditingState(true, action.Value));
      }

      public virtual void Undo()
      {
         reasignStates(false);
      }

      public virtual void Redo()
      {
         reasignStates(true);
      }

      public abstract string GetDescription();
      public abstract string GetDebugInformation(int indent);

      #endregion

      #region Helper methods

      private void SetAllPreviousStates(List<ObjEditingStatus> states)
      {
         var cnt = 0;
         foreach (var state in states)
            _previousState[cnt++] = state;
      }

      private void SetAllEditingStates(ICollection<Saveable> saveables, List<ObjEditingStatus> states)
      {
         var cnt = 0;
         foreach (var saveable in saveables)
            saveable.EditingStatus = states[cnt++];
      }

      private void SetAllEditingStates(ICollection<Saveable> saveables, ObjEditingStatus state)
      {
         foreach (var saveable in saveables)
            saveable.EditingStatus = state;
      }

      private ObjEditingStatus GetEditingState(bool forwards, SaveableOperation operation)
      {
         return operation switch
         {
            SaveableOperation.Default => ObjEditingStatus.Modified,
            SaveableOperation.Deleted => forwards ? ObjEditingStatus.ToBeDeleted : ObjEditingStatus.Modified,
            SaveableOperation.Created => forwards ? ObjEditingStatus.Modified : ObjEditingStatus.ToBeDeleted,
            _ => throw new ArgumentOutOfRangeException(nameof(operation), "Help.")
         };
      }

      private void reasignStates(bool forwards)
      {
         List<ObjEditingStatus> states = [.. GetAllSaveables().Select(x => x.EditingStatus)];
         foreach (var action in _actions)
         {
            
            if (_internalState != globalState)
               // Not current
               SetAllEditingStates(action.Key, GetEditingState(forwards, action.Value));
            else
               SetAllEditingStates(action.Key, _previousState);
            
         }
         SetAllPreviousStates(states);
         _internalState = globalState;
      }
      #endregion
   }

   public abstract class SaveableHelper
   {
      static SaveableHelper()
      {
         SaveMaster.Saving += (_, _) => SaveTest();
      }

      protected static int globalState = 0;

      public static void SaveTest()
      {
         globalState++;
      }
   }



   public class SaveableCommandHelper(Saveable saveable) : SaveableHelper
   {
      private ObjEditingStatus _previousState;
      private int _internalState = globalState;

      private void ReasignStates()
      {
         var state = saveable.EditingStatus;
         if (_internalState != globalState)
         {
            // Not current

            saveable.EditingStatus = ObjEditingStatus.Modified;

            _internalState = globalState;
         }
         else
            saveable.EditingStatus = _previousState;
         _previousState = state;
      }

      public virtual void Execute()
      {
         if (_internalState != globalState)
            throw new EvilActions("History Commands are not in Sync with global state!");
         saveable.EditingStatus = ObjEditingStatus.Modified;
      }

      public virtual void Undo()
      {
         ReasignStates();
      }

      public virtual void Redo()
      {
         ReasignStates();
      }
   }

   public class SaveablesCommandHelper(ICollection<Saveable> saveables) : SaveableHelper
   {
      // caches the previous state of the object
      private List<ObjEditingStatus> _previousState = [];
      private int internalState = globalState;
      private Saveable test_saveable = saveables.First();

      public virtual void Execute()
      {
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

      private void reasignStates()
      {
         List<ObjEditingStatus> states = [.. saveables.Select(x => x.EditingStatus)];
         if (internalState != globalState)
         {
            // Not current

            SetAllEditingStates(ObjEditingStatus.Modified);

            internalState = globalState;
         }
         else
            SetAllEditingStates(_previousState);
         SetAllPreviousStates(states);
      }

      public virtual void Undo()
      {
         reasignStates();
      }

      public virtual void Redo()
      {
         reasignStates();
      }
   }
}