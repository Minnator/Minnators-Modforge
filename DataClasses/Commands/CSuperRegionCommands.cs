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
      private string _superRegionName;
      private List<string> _deltaRegions;
      private readonly List<KeyValuePair<string, string>> _regionsPerSuperRegion = [];

      public CAddNewSuperRegion(string superRegionName, List<string> deltaRegions, bool executeOnInit = true)
      {
         _superRegionName = superRegionName;
         _deltaRegions = deltaRegions;

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
      private SuperRegion _sRegion = null!;

      public CDeleteSuperRegion(string superRegionName, bool executeOnInit = true)
      {
         _superRegionName = superRegionName;

         if (executeOnInit) 
            Execute();
      }

      public void Execute()
      {
         if (!Globals.SuperRegions.TryGetValue(_superRegionName, out _sRegion!))
            return;

         foreach (var regionName in _sRegion.Regions)
         {
            if (!Globals.Regions.TryGetValue(regionName, out var region))
               continue;
            _regionsPerSuperRegion.Add(new(regionName, region.SuperRegion));
            region.SuperRegion = string.Empty;
            _sRegion.RemoveRegion(regionName);
         }

         Globals.SuperRegions.Remove(_superRegionName);
      }

      public void Undo()
      {
         if (Globals.SuperRegions.ContainsKey(_superRegionName))
         {
            MessageBox.Show($"Super Region {_superRegionName} already exists! \nUnable to reverse Deleting", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
         }

         foreach (var kvp in _regionsPerSuperRegion)
         {
            if (!Globals.Regions.TryGetValue(kvp.Key, out var region))
               continue;
            region.SuperRegion = kvp.Value;
            _sRegion.AddRegion(kvp.Key);
         }

         Globals.SuperRegions.Add(_superRegionName, _sRegion);
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