using Editor.Saving;

namespace Editor.DataClasses.Commands
{

   public abstract class SaveableCommand : ICommand
   {
      public abstract void Execute();
      public abstract void Undo();
      public abstract void Redo();
      public abstract string GetDescription();
      public abstract string GetDebugInformation(int indent);
   }

   public abstract class TestSaveableHelper
   {
      static TestSaveableHelper()
      {
         SaveMaster.Saving += (_, _) => SaveTest();
      }

      protected static int globalState = 0;

      public static void SaveTest()
      {
         globalState++;
      }
   }



   public class SaveableCommandHelper(Saveable saveable) : TestSaveableHelper
   {
      private ObjEditingStatus _previousState;
      private int internalState = globalState;

      private void ReasignStates()
      {
         var state = saveable.EditingStatus;
         if (internalState != globalState)
         {
            // Not current

            saveable.EditingStatus = ObjEditingStatus.Modified;

            internalState = globalState;
         }
         else
            saveable.EditingStatus = _previousState;
         _previousState = state;
      }

      public virtual void Execute()
      {
         if (internalState != globalState)
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

   public class SaveablesCommandHelper(ICollection<Saveable> saveables) : TestSaveableHelper
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