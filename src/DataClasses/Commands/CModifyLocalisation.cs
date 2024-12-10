using Editor.Helper;

namespace Editor.DataClasses.Commands
{
   public class CModifyLocalisation : CTextEditingWrapper
   {
      private readonly List<LocObject> _locObjects;
      private string _newLoc;
      private readonly List<string> _oldLoc;

      public CModifyLocalisation(ICollection<LocObject> obj, string newLoc, bool executeOnInit = true)
      {
         _newLoc = newLoc;
         _locObjects = [..obj];
         _oldLoc = obj.Select(x => x.Value).ToList();

         if (executeOnInit)
            Execute();
      }


      public override void Execute()
      {
         base.Execute([.. _locObjects]);
         InternalExecute();
      }

      public override void Undo()
      {
         base.Undo();
         var cnt = 0;
         foreach (var obj in _locObjects)
         {
            obj.SilentSet(_oldLoc[cnt]);
            cnt++;
         }
      }

      public override void Redo()
      {
         base.Redo();
         InternalExecute();
      }

      private void InternalExecute()
      {
         _locObjects.ForEach((x) => x.SilentSet(_newLoc));
      }

      public override string GetDescription()
      {
         if (_locObjects.Count == 1)
         {
            var locObject = _locObjects.First();
            return $"Modified \"{locObject.Key[..Math.Min(locObject.Key.Length, 20)]}\" to \"{_newLoc[..Math.Min(_newLoc.Length, 20)]}\"";
         }
         return $"Modified \"{_locObjects.Count}\" localisations to \"{_newLoc}\"";
      }

      public override string GetDebugInformation(int indent)
      {
         return _locObjects.Count == 1 ? $"Modified \"{_locObjects[^1].Value}\" to \"{_newLoc}\"" : $"Modified \"{_locObjects.Count}\" localisations to \"{_newLoc}\"";
      }

      public override void SetValue(string value)
      {
         _newLoc = value;
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