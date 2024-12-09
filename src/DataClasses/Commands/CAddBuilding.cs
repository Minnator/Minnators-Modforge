using System.Text;
using Editor.DataClasses.GameDataClasses;

namespace Editor.DataClasses.Commands
{
   public class CAddBuilding : SaveableCommandBasic
   {
      private readonly bool _add;
      private readonly string _building;
      private readonly List<Province> _provinces;
      

      public CAddBuilding(List<Province> provinces, bool add, string building, bool executeOnInit = true)
      {
         _provinces = provinces;
         _add = add;
         _building = building;

         if (executeOnInit)
            Execute();
      }


      public override void Execute()
      {
         base.Execute([.._provinces]);
         InternalExecute(); 
      }

      public override void Undo()
      {
         base.Undo();
         foreach (var province in _provinces)
            province.SetAttribute(_building, _add ? "no" : "yes");
      }

      public override void Redo()
      {
         base.Redo();
         InternalExecute();
      }

      private void InternalExecute()
      {
         foreach (var province in _provinces)
            province.SetAttribute(_building, _add ? "yes" : "no");
      }

      public override string GetDescription()
      {
         return _provinces.Count == 1
            ? $"{(_add ? "Added" : "Removed")} {_building} from {_provinces[0].Id} ({_provinces[0].GetLocalisation()})"
            : $"{(_add ? "Added" : "Removed")} {_building} from [{_provinces.Count}] provinces";
      }

      public override string GetDebugInformation(int indent)
      {
         var sb = new StringBuilder();
         if (_add)
            sb.AppendLine($"Added Building: \"{_building}\" to {_provinces.Count} provinces:");
         else
            sb.AppendLine($"Removed Building: \"{_building}\" from {_provinces.Count} provinces:");
         foreach (var province in _provinces)
            sb.Append($"{province.Id}, ");
         return sb.ToString();
      }
   }
}