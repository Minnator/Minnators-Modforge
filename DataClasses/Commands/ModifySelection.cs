using Editor.Commands;
using Editor.DataClasses.GameDataClasses;
using Editor.Helper;

namespace Editor.DataClasses.Commands
{
   public class ModifySelection : ICommand
   {
      private List<Province> _deltaProvinces;
      private bool _isAddition;

      public ModifySelection(List<Province> deltaProvinces, bool isAddition, bool executeOnInit = true)
      {
         _deltaProvinces = deltaProvinces;
         _isAddition = isAddition;

         if (executeOnInit)
            Execute();
      }

      public void Execute()
      {
         if (_isAddition)
            Selection.AddProvincesToSelection(_deltaProvinces);
         else
            Selection.RemoveProvincesFromSelection(_deltaProvinces);
      }

      public void Undo()
      {
         if (!_isAddition)
            Selection.RemoveProvincesFromSelection(_deltaProvinces);
         else
            Selection.AddProvincesToSelection(_deltaProvinces);
      }

      public void Redo()
      {
         Execute();
      }

      public string GetDescription()
      {
         return _isAddition ? $"Add {_deltaProvinces.Count} provinces to selection" : $"Remove {_deltaProvinces.Count} provinces from selection";
      }
   }
}