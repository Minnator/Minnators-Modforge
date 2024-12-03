using Editor.Helper;

namespace Editor.DataClasses.Commands
{
   public class CModifyLocalisation : ICommand
   {
      private readonly LocObject _locObject;
      private readonly SaveableCommandHelper _locObjectSaveableHelper;
      private readonly string _newLoc;
      private readonly string _oldLoc;

      public CModifyLocalisation(LocObject obj, string newLoc, bool executeOnInit = true)
      {
         _newLoc = newLoc;
         _locObjectSaveableHelper = new(obj);
         _locObject = obj;
         _oldLoc = _locObject.Value;

         if (executeOnInit)
            Execute();
      }


      public void Execute()
      {
         _locObjectSaveableHelper.Execute();
         InternalExecute();
      }

      public void Undo()
      {
         _locObjectSaveableHelper.Undo();
         _locObject.SilentSet(_oldLoc);
      }

      public void Redo()
      {
         _locObjectSaveableHelper.Redo();
         InternalExecute();
      }

      private void InternalExecute()
      {
         _locObject.SilentSet(_newLoc);
      }

      public string GetDescription()
      {
         return $"Modified \"{_locObject.Key[..Math.Min(_locObject.Key.Length, 20)]}\" to \"{_newLoc[..Math.Min(_newLoc.Length, 20)]}\"";
      }

      public string GetDebugInformation(int indent)
      {
         return $"Changed \"{_locObject.Key}\" from \"{_oldLoc}\" to \"{_newLoc}\"";
      }
   }

   public class CAddDelLocalisation : ICommand
   {
      private readonly LocObject _locObject;
      private readonly SaveableCommandHelper _locObjectSaveableHelper;
      private readonly bool _add;

      public CAddDelLocalisation(LocObject obj, bool add, bool executeOnInit = true)
      {
         _locObject = obj;
         _locObjectSaveableHelper = new(obj);
         _add = add;

         if (executeOnInit)
            Execute();
      }

      public void Execute()
      {
         _locObjectSaveableHelper.Execute();
         InternalExecute();
      }

      public void Undo()
      {
         _locObjectSaveableHelper.Undo();
         if (_add)
            Globals.Localisation.Remove(_locObject);
         else
            Globals.Localisation.Add(_locObject);
      }

      public void Redo()
      {
         _locObjectSaveableHelper.Redo();
         InternalExecute();
      }

      public void InternalExecute()
      {
         if (_add)
            Globals.Localisation.Add(_locObject);
         else
            Globals.Localisation.Remove(_locObject);
      }

      public string GetDescription()
      {
         return _add ? $"Added \"{_locObject.Key[..Math.Min(_locObject.Key.Length, 20)]}\"" : $"Deleted \"{_locObject.Key[..Math.Min(_locObject.Key.Length, 20)]}\"";
      }

      public string GetDebugInformation(int indent)
      {
         return _add ? $"Added \"{_locObject.Key}\"" : $"Deleted \"{_locObject.Key}\"";
      }
   }
}