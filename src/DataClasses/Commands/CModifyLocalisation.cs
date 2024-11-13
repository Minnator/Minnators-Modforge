using Editor.Helper;

namespace Editor.DataClasses.Commands
{
   public class CModifyLocalisation : ICommand
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


      public void Execute()
      {
         _locObject.Value = _newLoc;
      }

      public void Undo()
      {
         _locObject .Value = _oldLoc;
      }

      public void Redo()
      {
         Execute();
      }

      public string GetDescription()
      {
         return $"Modified \"{_locObject.Key[..Math.Min(_locObject.Key.Length, 20)]}\" to \"{_newLoc[..Math.Min(_newLoc.Length, 20)]}\"";
      }
   }

   public class CAddDelLocalisation : ICommand
   {
      private readonly LocObject _locObject;
      private readonly bool _add;

      public CAddDelLocalisation(LocObject obj, bool add, bool executeOnInit = true)
      {
         _locObject = obj;
         _add = add;

         if (executeOnInit)
            if (add)
               Execute();
            else
               Undo();
      }

      public void Execute()
      {
         if (_add)
            Globals.Localisation.Add(_locObject);
         else
            Globals.Localisation.Remove(_locObject);
      }

      public void Undo()
      {
         if (_add)
            Globals.Localisation.Remove(_locObject);
         else
            Globals.Localisation.Add(_locObject);
      }

      public void Redo()
      {
         if (_add)
            Execute();
         else
            Undo();
      }

      public string GetDescription()
      {
         return _add ? $"Added \"{_locObject.Key[..Math.Min(_locObject.Key.Length, 20)]}\"" : $"Deleted \"{_locObject.Key[..Math.Min(_locObject.Key.Length, 20)]}\"";
      }
   }
}