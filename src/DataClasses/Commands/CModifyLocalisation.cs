using Editor.Helper;

namespace Editor.DataClasses.Commands
{
   public class CModifyLocalisation : SaveableCommandBasic
   {
      private readonly LocObject _locObject;
      private readonly string _newLoc;
      private readonly string _oldLoc;

      public CModifyLocalisation(LocObject obj, string newLoc, bool executeOnInit = true)
      {
         _newLoc = newLoc;
         _locObject = obj;
         _oldLoc = _locObject.Value;

         if (executeOnInit)
            Execute();
      }


      public override void Execute()
      {
         base.Execute([_locObject]);
         InternalExecute();
      }

      public override void Undo()
      {
         base.Undo();
         _locObject.SilentSet(_oldLoc);
      }

      public override void Redo()
      {
         base.Redo();
         InternalExecute();
      }

      private void InternalExecute()
      {
         _locObject.SilentSet(_newLoc);
      }

      public override string GetDescription()
      {
         return $"Modified \"{_locObject.Key[..Math.Min(_locObject.Key.Length, 20)]}\" to \"{_newLoc[..Math.Min(_newLoc.Length, 20)]}\"";
      }

      public override string GetDebugInformation(int indent)
      {
         return $"Changed \"{_locObject.Key}\" from \"{_oldLoc}\" to \"{_newLoc}\"";
      }
   }

   public class CAddDelLocalisation : SaveableCommandBasic
   {
      private readonly LocObject _locObject;
      private readonly bool _add;

      public CAddDelLocalisation(LocObject obj, bool add, bool executeOnInit = true)
      {
         _locObject = obj;
         _add = add;

         if (executeOnInit)
            Execute();
      }

      public override void Execute()
      {
         base.Execute([_locObject]);
         InternalExecute();
      }

      public override void Undo()
      {
         base.Undo();
         if (_add)
            Globals.Localisation.Remove(_locObject);
         else
            Globals.Localisation.Add(_locObject);
      }

      public override void Redo()
      {
         base.Redo();
         InternalExecute();
      }

      public void InternalExecute()
      {
         if (_add)
            Globals.Localisation.Add(_locObject);
         else
            Globals.Localisation.Remove(_locObject);
      }

      public override string GetDescription()
      {
         return _add ? $"Added \"{_locObject.Key[..Math.Min(_locObject.Key.Length, 20)]}\"" : $"Deleted \"{_locObject.Key[..Math.Min(_locObject.Key.Length, 20)]}\"";
      }

      public override string GetDebugInformation(int indent)
      {
         return _add ? $"Added \"{_locObject.Key}\"" : $"Deleted \"{_locObject.Key}\"";
      }
   }
}