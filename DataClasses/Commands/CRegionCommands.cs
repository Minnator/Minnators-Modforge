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
               _oldAreasPerRegion.Add(new(areaName, area.Region));
               area.Region = _regionName;
               region.AddArea(areaName);
            }
         }
         else // Remove from region areas
         {
            foreach (var areaName in _deltaAreas)
            {
               if (!Globals.Areas.TryGetValue(areaName, out var area))
                  continue;
               _oldAreasPerRegion.Add(new(areaName, area.Region));
               area.Region = string.Empty;
               region.RemoveArea(areaName);
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

   public class CAddNewRegion : ICommand
   {
      private readonly string _regionName;
      private readonly List<string> _areas;
      private List<KeyValuePair<string, string>> _oldAreasPerRegion = [];

      public CAddNewRegion(string regionName, List<string> areas, bool executeOnInit = true)
      {
         _regionName = regionName;
         _areas = areas;

         if (executeOnInit)
            Execute();
      }

      public void Execute()
      {
         foreach (var area in _areas)
         {
            if (!Globals.Areas.TryGetValue(area, out var value))
               continue;
            _oldAreasPerRegion.Add(new(area, value.Region));
            value.Region = _regionName;
         }
         Globals.Regions.Add(_regionName, new (_regionName, []));

         foreach (var area in _areas)
            Globals.Regions[_regionName].AddArea(area);
      }

      public void Undo()
      {
         if (!Globals.Regions.TryGetValue(_regionName, out var region))
            return;

         // Remove the areas from the region first, as this triggers the MapUpdate if the RegionMapMode is active
         // This would also be the case in the vale.AddArea() method, but this can not update if the region was string.Empty
         for (var i = region.Areas.Count - 1; i >= 0; i--) 
            region.RemoveArea(region.Areas[i]);

         Globals.Regions.Remove(_regionName);

         foreach (var (areaName, oldRegion) in _oldAreasPerRegion)
         {
            if (!Globals.Areas.TryGetValue(areaName, out var area))
               continue;
            area.Region = oldRegion;
            if (Globals.Regions.TryGetValue(oldRegion, out var value))
               value.AddArea(areaName);
         }
      }

      public void Redo()
      {
         Execute();
      }

      public string GetDescription()
      {
         return $"Create new region {_regionName}";
      }
   }

   public class CDeleteRegion : ICommand
   {
      private readonly string _regionName;
      private List<KeyValuePair<string, string>> _oldAreasPerRegion = [];

      public CDeleteRegion(string regionName, bool executeOnInit = true)
      {
         _regionName = regionName;

         if (executeOnInit)
            Execute();
      }

      public void Execute()
      {
         if (!Globals.Regions.TryGetValue(_regionName, out var region))
            return;
         
         foreach (var area in region.Areas)
         {
            if (!Globals.Areas.TryGetValue(area, out var value))
               continue;
            _oldAreasPerRegion.Add(new(area, value.Region));
            value.Region = string.Empty;
         }

         // Trigger the MapUpdate via this
         for (var i = region.Areas.Count - 1; i >= 0; i--)
            region.RemoveArea(region.Areas[i]);
         
         Globals.Regions.Remove(_regionName);
      }

      public void Undo()
      {
         foreach (var (areaName, oldRegion) in _oldAreasPerRegion)
         {
            if (!Globals.Areas.TryGetValue(areaName, out var area))
               continue;
            area.Region = oldRegion;
            if (Globals.Regions.TryGetValue(oldRegion, out var value))
               value.AddArea(areaName);
         }

         Globals.Regions.Add(_regionName, new (_regionName, []));

         // Trigger the MapUpdate via this
         foreach (var (areaName, _) in _oldAreasPerRegion)
            Globals.Regions[_regionName].AddArea(areaName);
      }

      public void Redo()
      {
         Execute();
      }

      public string GetDescription()
      {
         return $"Deleted region {_regionName}";
      }
   }

}