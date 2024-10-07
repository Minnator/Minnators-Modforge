using Editor.Commands;
using Editor.Controls;
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
      private readonly List<KeyValuePair<string, string>> _oldAreasPerRegion = [];
      private readonly ComboBox _comboBox;

      public CAddNewRegion(string regionName, List<string> areas, ComboBox comboBox, bool executeOnInit = true)
      {
         _regionName = regionName;
         _areas = areas;
         _comboBox = comboBox;

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

         _comboBox.Items.Add(_regionName);
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

         _comboBox.Items.Remove(_regionName);
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
      private Region _region = null!;
      private List<string> _areas = [];
      private readonly CollectionEditor _collectionEditor;

      public CDeleteRegion(string regionName, CollectionEditor collectionEditor, bool executeOnInit = true)
      {
         _regionName = regionName;
         _collectionEditor = collectionEditor;

         if (executeOnInit)
            Execute();
      }

      public void Execute()
      {
         if (!Globals.Regions.TryGetValue(_regionName, out _region!))
            return;
         
         // Trigger the MapUpdate via this
         for (var i = _region.Areas.Count - 1; i >= 0; i--)
         {
            _areas.Add(_region.Areas[i]);
            if (!Globals.Areas.TryGetValue(_region.Areas[i], out var area))
               continue;
            area.Region = string.Empty;
            _region.RemoveArea(_region.Areas[i]);
         }
         
         Globals.Regions.Remove(_regionName);
         Globals.MapWindow.RegionEditingGui.ExtendedComboBox.Items.Remove(_regionName);
      }

      public void Undo()
      {
         if (!Globals.Regions.TryAdd(_regionName, _region))
            return;
         foreach (var areaName in _areas)
         {
            if (!Globals.Areas.TryGetValue(areaName, out var area))
               continue;
            area.Region = _regionName;
            Globals.Regions[_regionName].AddArea(areaName);
         }
         
         Globals.MapWindow.RegionEditingGui.ExtendedComboBox.Items.Add(_regionName);
         _collectionEditor.ExtendedComboBox.Items.Add(_regionName);
         _collectionEditor.ExtendedComboBox.SelectedIndex = _collectionEditor.ExtendedComboBox.Items.IndexOf(_regionName);
         _collectionEditor.ExtendedComboBox.SelectionStart = _collectionEditor.Text.Length;
         Globals.MapWindow.RegionEditingGui.ExtendedComboBox.AutoCompleteCustomSource.Add(_regionName);
         Globals.MapWindow.RegionEditingGui.ExtendedComboBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
         Globals.MapWindow.RegionEditingGui.ExtendedComboBox.AutoCompleteSource = AutoCompleteSource.ListItems;
      }

      public void Redo()
      {
         Execute();
         Globals.MapWindow.RegionEditingGui.ExtendedComboBox.AutoCompleteCustomSource.Remove(_regionName);
         Globals.MapWindow.RegionEditingGui.ExtendedComboBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
         Globals.MapWindow.RegionEditingGui.ExtendedComboBox.AutoCompleteSource = AutoCompleteSource.ListItems;
         _collectionEditor.Clear();
      }

      public string GetDescription()
      {
         return $"Deleted region {_regionName}";
      }
   }

}