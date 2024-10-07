using Editor.Commands;
using Editor.Controls;
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
      private readonly CollectionEditor _collectionEditor;
      private readonly Color _color;

      /// <summary>
      /// Adds a new super region by specifying the name and the regions it should contain.
      /// </summary>
      /// <param name="superRegionName">The name of the new super region to be created.</param>
      /// <param name="deltaRegions">The list of regions to be included in the new super region.</param>
      /// <param name="collectionEditor">The editor responsible for modifying collections of regions and super regions.</param>
      /// <param name="executeOnInit">Determines if the execution should occur immediately upon initialization (default is true).</param>
      public CAddNewSuperRegion(string superRegionName, List<string> deltaRegions, CollectionEditor collectionEditor, bool executeOnInit = true)
      {
         _superRegionName = superRegionName;
         _deltaRegions = deltaRegions;
         _collectionEditor = collectionEditor;
         _color = Globals.ColorProvider.GetRandomColor();

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

         SuperRegion sRegion = new (_superRegionName, [])
         {
            Color = _color
         };

         Globals.SuperRegions.Add(_superRegionName, sRegion);

         foreach (var regionName in _deltaRegions)
         {
            if (!Globals.Regions.TryGetValue(regionName, out var region))
               continue;
            _regionsPerSuperRegion.Add(new(regionName, region.SuperRegion));
            region.SuperRegion = _superRegionName;
            sRegion.AddRegion(regionName);
         }

         _collectionEditor.ExtendedComboBox.Items.Add(_superRegionName);
         Globals.MapWindow.RegionEditingGui.ExtendedComboBox.AutoCompleteCustomSource.Add(_superRegionName);
         Globals.MapWindow.RegionEditingGui.ExtendedComboBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
         Globals.MapWindow.RegionEditingGui.ExtendedComboBox.AutoCompleteSource = AutoCompleteSource.ListItems;
         _collectionEditor.ExtendedComboBox.SelectedIndex = _collectionEditor.ExtendedComboBox.Items.IndexOf(_superRegionName);
      }

      public void Undo()
      {
         if (!Globals.SuperRegions.TryGetValue(_superRegionName, out _))
         {
            MessageBox.Show($"Super Region {_superRegionName} already exists! \nUnable to reverse Adding", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
         }

         foreach (var (regionName, oldSuperRegion) in _regionsPerSuperRegion)
         {
            if (!Globals.Regions.TryGetValue(regionName, out var region))
               continue;
            region.SuperRegion = oldSuperRegion;
            Globals.SuperRegions[_superRegionName].AddRegion(regionName);
         }

         Globals.SuperRegions.Remove(_superRegionName);
         _collectionEditor.ExtendedComboBox.Items.Remove(_superRegionName);
         _collectionEditor.ExtendedComboBox.Text = string.Empty;
         Globals.MapWindow.RegionEditingGui.ExtendedComboBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
         Globals.MapWindow.RegionEditingGui.ExtendedComboBox.AutoCompleteSource = AutoCompleteSource.ListItems;
         Globals.MapWindow.RegionEditingGui.ExtendedComboBox.AutoCompleteCustomSource.Remove(_superRegionName);
         _collectionEditor.Clear();
      }

      public void Redo()
      {
         Execute();
         _collectionEditor.ExtendedComboBox.Items.Add(_superRegionName);
         Globals.MapWindow.RegionEditingGui.ExtendedComboBox.AutoCompleteCustomSource.Add(_superRegionName);
         Globals.MapWindow.RegionEditingGui.ExtendedComboBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
         Globals.MapWindow.RegionEditingGui.ExtendedComboBox.AutoCompleteSource = AutoCompleteSource.ListItems;
         _collectionEditor.ExtendedComboBox.SelectedIndex = _collectionEditor.ExtendedComboBox.Items.IndexOf(_superRegionName);
      }

      public string GetDescription()
      {
         return $"Add super region {_superRegionName} with {_deltaRegions.Count} regions";
      }
   }

   // Finished
   public class CDeleteSuperRegion : ICommand
   {
      private readonly string _superRegionName;
      private SuperRegion _superRegion = null!;
      private readonly List<string> _oldRegions = [];
      private readonly CollectionEditor _collectionEditor;

      public CDeleteSuperRegion(string superRegionName, CollectionEditor collectionEditor, bool executeOnInit = true)
      {
         _superRegionName = superRegionName;
         _collectionEditor = collectionEditor;

         if (executeOnInit) 
            Execute();
      }

      public void Execute()
      {
         if (!Globals.SuperRegions.TryGetValue(_superRegionName, out _superRegion!))
            return;

         for (var i = _superRegion.Regions.Count - 1; i >= 0; i--)
         {
            var regionName = _superRegion.Regions[i];
            if (!Globals.Regions.TryGetValue(regionName, out var region))
               continue;
            _oldRegions.Add(regionName);
            region.SuperRegion = string.Empty;
            _superRegion.RemoveRegion(regionName);
         }

         Globals.SuperRegions.Remove(_superRegionName);
         _collectionEditor.ExtendedComboBox.Items.Remove(_superRegionName);
         _collectionEditor.ExtendedComboBox.Text = string.Empty;
         Globals.MapWindow.RegionEditingGui.ExtendedComboBox.AutoCompleteCustomSource.Remove(_superRegionName);
         Globals.MapWindow.RegionEditingGui.ExtendedComboBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
         Globals.MapWindow.RegionEditingGui.ExtendedComboBox.AutoCompleteSource = AutoCompleteSource.ListItems;
         _collectionEditor.Clear();
      }

      public void Undo()
      {
         if (Globals.SuperRegions.TryGetValue(_superRegionName, out _))
         {
            MessageBox.Show($"Super Region {_superRegionName} already exists! \nUnable to reverse Deleting", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
         }

         Globals.SuperRegions.Add(_superRegionName, _superRegion);

         foreach (var regionName in _oldRegions)
         {
            if (!Globals.Regions.TryGetValue(regionName, out var region))
               continue;
            region.SuperRegion = _superRegionName;
            Globals.SuperRegions[_superRegionName].AddRegion(regionName);
         }

         _collectionEditor.ExtendedComboBox.Items.Add(_superRegionName);
         _collectionEditor.ExtendedComboBox.SelectedIndex = _collectionEditor.ExtendedComboBox.Items.IndexOf(_superRegionName);
         _collectionEditor.ExtendedComboBox.SelectionStart = _superRegionName.Length;
         Globals.MapWindow.RegionEditingGui.ExtendedComboBox.AutoCompleteCustomSource.Add(_superRegionName);
         Globals.MapWindow.RegionEditingGui.ExtendedComboBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
         Globals.MapWindow.RegionEditingGui.ExtendedComboBox.AutoCompleteSource = AutoCompleteSource.ListItems;
      }

      public void Redo()
      {
         Execute();
         _collectionEditor.ExtendedComboBox.Items.Remove(_superRegionName);
         _collectionEditor.ExtendedComboBox.Text = string.Empty;
         Globals.MapWindow.RegionEditingGui.ExtendedComboBox.AutoCompleteCustomSource.Remove(_superRegionName);
         Globals.MapWindow.RegionEditingGui.ExtendedComboBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
         Globals.MapWindow.RegionEditingGui.ExtendedComboBox.AutoCompleteSource = AutoCompleteSource.ListItems;
         _collectionEditor.Clear();
      }

      public string GetDescription()
      {
         return $"Delete super region {_superRegionName}";
      }
   }
}