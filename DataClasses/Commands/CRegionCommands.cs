using Editor.Commands;
using Region = Editor.DataClasses.GameDataClasses.Region;

namespace Editor.DataClasses.Commands
{

   public class CModifyExistingRegion : ICommand
   {
      private readonly string _regionName;
      private readonly List<string> _deltaAreas;
      private readonly bool _add;
      private readonly List<KeyValuePair<string, string>> _oldAreasPerRegion = [];

      public CModifyExistingRegion(string regionName, List<string> deltaAreas, bool add, bool executeOnInit = true)
      {
         _regionName = regionName;
         _deltaAreas = deltaAreas;
         _add = add;

         if (executeOnInit)
            Execute();
      }

      public void Execute()
      {
         if (!Globals.Regions.TryGetValue(_regionName, out var region))
            return;
         if (_add) // Add to region areas
         {
            foreach (var areaName in _deltaAreas)
            {
               if (!Globals.Areas.TryGetValue(areaName, out var area))
                  continue;
               region.AddArea(areaName);
               _oldAreasPerRegion.Add(new(areaName, area.Region));
               area.Region = _regionName;
            }
         }
         else // Remove from region areas
         {
            foreach (var areaName in _deltaAreas)
            {
               if (!Globals.Areas.TryGetValue(areaName, out var area))
                  continue;
               region.RemoveArea(areaName);
               _oldAreasPerRegion.Add(new(areaName, area.Region));
               area.Region = string.Empty;
            }
         }
      }

      public void Undo()
      {
         if (!Globals.Regions.TryGetValue(_regionName, out _))
            return;
         foreach (var (areaName, oldRegion) in _oldAreasPerRegion)
         {
            if (!Globals.Areas.TryGetValue(areaName, out var area) || !Globals.Regions.TryGetValue(oldRegion, out var value))
               continue;
            if (oldRegion == string.Empty)
               Globals.Regions[_regionName].RemoveArea(areaName);
            else
               value.AddArea(areaName);
            area.Region = oldRegion;
         }
      }

      public void Redo()
      {
         Execute();
      }

      public string GetDescription()
      {
         return _add ? $"Add {_deltaAreas.Count} areas to region {_regionName}" : $"Remove {_deltaAreas.Count} areas from region {_regionName}";
      }
   }
}