using Editor.Commands;
using Editor.DataClasses.GameDataClasses;

namespace Editor.DataClasses.Commands
{
   public class CModifyExistingSuperRegion : ICommand
   {
      private readonly string _superRegionName;
      private readonly List<string> _deltaRegions;
      private readonly List<KeyValuePair<string, string>> _regionsPerSuperRegion = [];
      private readonly bool _add;

      public CModifyExistingSuperRegion(string superRegionName, List<string> deltaRegions, bool add, bool executeOnInit = true)
      {
         _superRegionName = superRegionName;
         _deltaRegions = deltaRegions;
         _add = add;

         if (executeOnInit) 
            Execute();
      }

      public void Execute()
      {
         if (!Globals.SuperRegions.TryGetValue(_superRegionName, out var sRegion))
            return;

         if (_add)
         {
            foreach (var regionName in _deltaRegions)
            {
               if (!Globals.Regions.TryGetValue(regionName, out var region))
                  continue;
               _regionsPerSuperRegion.Add(new(regionName, region.SuperRegion));
               // set name field of region to superRegionName first to ensure mapMode updates as desired
               region.SuperRegion = _superRegionName;
               sRegion.AddRegion(regionName);
            }
         }
         else
         {
            foreach (var regionName in _deltaRegions)
            {
               if (!Globals.Regions.TryGetValue(regionName, out var region))
                  continue;
               _regionsPerSuperRegion.Add(new(regionName, region.SuperRegion));
               // set name field of region to superRegionName first to ensure mapMode updates as desired
               region.SuperRegion = string.Empty;
               sRegion.RemoveRegion(regionName);
            }
         }
      }

      public void Undo()
      {
         if (!Globals.SuperRegions.TryGetValue(_superRegionName, out _))
            return;

         foreach (var (regionName, oldSuperRegion) in _regionsPerSuperRegion)
         {
            if (!Globals.Regions.TryGetValue(regionName, out var region) || !Globals.SuperRegions.TryGetValue(oldSuperRegion, out var value))
               continue;
            if (oldSuperRegion == string.Empty)
               continue;
            region.SuperRegion = oldSuperRegion;
            value.AddRegion(regionName);
         }
      }

      public void Redo()
      {
         Execute();
      }

      public string GetDescription()
      {
         return _add ? $"Add {_deltaRegions.Count} regions to super region {_superRegionName}" : $"Remove {_deltaRegions.Count} regions from super region {_superRegionName}";
      }
   }

   public class CAddNewSuperRegion : ICommand
   {
      private readonly string _superRegionName;
      private readonly List<string> _deltaRegions;
      private readonly List<KeyValuePair<string, string>> _regionsPerSuperRegion = [];
      private readonly ComboBox _comboBox;

      public CAddNewSuperRegion(string superRegionName, List<string> deltaRegions, ComboBox comboBox, bool executeOnInit = true)
      {
         _superRegionName = superRegionName;
         _deltaRegions = deltaRegions;
         _comboBox = comboBox;

         if (executeOnInit) 
            Execute();
      }

      public void Execute()
      {
         if (Globals.SuperRegions.ContainsKey(_superRegionName))
         {
            MessageBox.Show($"Super Region {_superRegionName} already exists!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
         }

         var sRegion = new SuperRegion(_superRegionName, [])
         {
            Color = Globals.ColorProvider.GetRandomColor()
         };

         foreach (var regionName in _deltaRegions)
         {
            if (!Globals.Regions.TryGetValue(regionName, out var region))
               continue;
            _regionsPerSuperRegion.Add(new(regionName, region.SuperRegion));
            region.SuperRegion = _superRegionName;
            sRegion.AddRegion(regionName);
         }

         Globals.SuperRegions.Add(_superRegionName, sRegion);
         _comboBox.Items.Add(_superRegionName);
      }

      public void Undo()
      {
         if (!Globals.SuperRegions.TryGetValue(_superRegionName, out _))
            return;

         foreach (var (regionName, oldSuperRegion) in _regionsPerSuperRegion)
         {
            if (!Globals.Regions.TryGetValue(regionName, out var region) || !Globals.SuperRegions.TryGetValue(oldSuperRegion, out var value))
               continue;
            region.SuperRegion = oldSuperRegion;
            value.AddRegion(regionName);
         }

         Globals.SuperRegions.Remove(_superRegionName);
         _comboBox.Items.Remove(_superRegionName);
      }

      public void Redo()
      {
         Execute();
      }

      public string GetDescription()
      {
         return $"Add super region {_superRegionName} with {_deltaRegions.Count} regions";
      }
   }

   public class CDeleteSuperRegion : ICommand
   {
      private readonly string _superRegionName;
      private readonly List<KeyValuePair<string, string>> _regionsPerSuperRegion = [];
      private readonly Color _regionColor;
      private readonly ComboBox _comboBox;

      public CDeleteSuperRegion(string superRegionName, ComboBox comboBox, bool executeOnInit = true)
      {
         _superRegionName = superRegionName;
         _comboBox = comboBox;

         if (Globals.SuperRegions.TryGetValue(_superRegionName, out var sRegion))
            _regionColor = sRegion.Color;

         if (executeOnInit) 
            Execute();
      }

      public void Execute()
      {
         if (!Globals.SuperRegions.TryGetValue(_superRegionName, out var sRegion))
            return;

         for (var i = sRegion.Regions.Count - 1; i >= 0; i--)
         {
            var regionName = sRegion.Regions[i];
            if (!Globals.Regions.TryGetValue(regionName, out var region))
               continue;
            _regionsPerSuperRegion.Add(new(regionName, region.SuperRegion));
            region.SuperRegion = string.Empty;
            sRegion.RemoveRegion(regionName);
         }

         Globals.SuperRegions.Remove(_superRegionName);
         _comboBox.Items.Remove(_superRegionName);
      }

      public void Undo()
      {
         if (Globals.SuperRegions.TryGetValue(_superRegionName, out _))
         {
            MessageBox.Show($"Super Region {_superRegionName} already exists! \nUnable to reverse Deleting", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
         }

         Globals.SuperRegions.Add(_superRegionName, new(_superRegionName, []) { Color = _regionColor });

         foreach (var kvp in _regionsPerSuperRegion)
         {
            if (!Globals.Regions.TryGetValue(kvp.Key, out var region))
               continue;
            region.SuperRegion = kvp.Value;
            Globals.SuperRegions[_superRegionName].AddRegion(kvp.Key);
         }

         _comboBox.Items.Add(_superRegionName);
      }

      public void Redo()
      {
         Execute();
      }

      public string GetDescription()
      {
         return $"Delete super region {_superRegionName}";
      }
   }
}