﻿using Editor.Commands;
using Editor.Controls;
using Editor.DataClasses.GameDataClasses;

namespace Editor.DataClasses.Commands
{
   public class CModifyExitingArea : ICommand
   {
      private readonly string _areaName;
      private readonly List<Province> _deltaProvinces;
      private readonly bool _add;
      private readonly List<KeyValuePair<Province, string>> _oldAreasPerId = [];


      public CModifyExitingArea(string areaName, List<Province> deltaProvinces, bool add, bool executeOnInit = true)
      {
         _areaName = areaName;
         _deltaProvinces = deltaProvinces;
         _add = add;

         if (executeOnInit)
            Execute();
      }

      public void Execute()
      {
         if (!Globals.Areas.TryGetValue(_areaName, out var area))
            return;
         if (_add) // Add to area provinces
         {
            var newProvIds = area.Provinces.Union(_deltaProvinces).ToArray();
            area.Provinces = newProvIds;
            foreach (var prov in _deltaProvinces)
            {
               if (!Globals.Provinces.TryGetValue(prov, out var province))
                  continue;
               _oldAreasPerId.Add(new (prov, province.Area));
               if (Globals.Areas.TryGetValue(province.Area, out var oldArea)) 
                  oldArea.Provinces = oldArea.Provinces.Except([prov]).ToArray();
               province.Area = _areaName;
            }
         }
         else // Remove from area provinces
         {
            foreach (var prov in _deltaProvinces)
            {
               if (!Globals.Provinces.TryGetValue(prov, out var province))
                  continue;
               _oldAreasPerId.Add(new (prov, province.Area));
               if (Globals.Areas.TryGetValue(province.Area, out var oldArea))
                  oldArea.Provinces = oldArea.Provinces.Except([prov]).ToArray();
               province.Area = string.Empty;

            }
            area.Provinces = area.Provinces.Except(_deltaProvinces).ToArray();
         }

         area.CalculateBounds();
      }

      public void Undo()
      {
         if (!Globals.Areas.TryGetValue(_areaName, out var area))
            return;

         foreach (var kvp in _oldAreasPerId)
         {
            if (!Globals.ProvinceIdToProvince.TryGetValue(kvp.Key, out var province))
               continue;
            province.Area = kvp.Value;
            if (Globals.Areas.TryGetValue(kvp.Value, out var oldArea))
               oldArea.Provinces = oldArea.Provinces.Union([kvp.Key]).ToArray();
         }
         area.CalculateBounds();
      }

      public void Redo()
      {
         Execute();
      }

      public string GetDescription()
      {
         return _add ? $"Add provinces to area {_areaName}" : $"Remove provinces from area {_areaName}";
      }
   }

   public class CCreateNewArea : ICommand
   {
      private readonly string _areaName;
      private readonly List<Province> _provinces;
      private readonly List<KeyValuePair<Province, string>> _oldAreasPerId = [];
      private ComboBox _comboBox;

      public CCreateNewArea(string areaName, List<Province> provinces, ComboBox comboBox, bool executeOnInit = true)
      {
         _areaName = areaName;
         _provinces = provinces;
         _comboBox = comboBox;

         if (executeOnInit)
            Execute();
      }

      public void Execute()
      {
         if (Globals.Areas.ContainsKey(_areaName))
            return;

         var area = new Area(_areaName, _provinces.ToArray(), Globals.ColorProvider.GetRandomColor());
         Globals.Areas.Add(_areaName, area);

         foreach (var prov in _provinces)
         {
            if (!Globals.Provinces.TryGetValue(prov, out var province))
               continue;
            _oldAreasPerId.Add(new (prov, province.Area));
            if (Globals.Areas.TryGetValue(province.Area, out var oldArea))
               oldArea.Provinces = oldArea.Provinces.Except([prov]).ToArray();
            province.Area = _areaName;
         }

         _comboBox.Items.Add(_areaName);
         area.CalculateBounds();
      }

      public void Undo()
      {
         if (!Globals.Areas.Remove(_areaName, out _))
            return;

         foreach (var kvp in _oldAreasPerId)
         {
            if (!Globals.Provinces.TryGetValue(kvp.Key, out var province))
               continue;
            province.Area = kvp.Value;
            if (Globals.Areas.TryGetValue(kvp.Value, out var oldArea))
               oldArea.Provinces = oldArea.Provinces.Union([kvp.Key]).ToArray();
         }

         _comboBox.AutoCompleteCustomSource.Remove(_areaName);
         _comboBox.Items.Remove(_areaName);
         _comboBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
         _comboBox.AutoCompleteSource = AutoCompleteSource.ListItems;
      }

      public void Redo()
      {
         Execute();
         // Do it down here as it is covered in the add_button event of the CollectionEditor
         _comboBox.AutoCompleteCustomSource.Add(_areaName);
         _comboBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
         _comboBox.AutoCompleteSource = AutoCompleteSource.ListItems;
      }

      public string GetDescription()
      {
         return $"Create new area {_areaName}";
      }
   }

   public class CRemoveArea : ICommand
   {
      private readonly string _areaName;
      private Area _area = null!;
      private readonly ComboBox _comboBox;

      public CRemoveArea(string area, ComboBox comboBox, bool executeOnInit = true)
      {
         _areaName = area;
         _comboBox = comboBox;

         if (executeOnInit)
            Execute();
      }

      public void Execute()
      {
         if (!Globals.Areas.TryGetValue(_areaName, out _area!))
            return;
         
         foreach (var prov in _area.Provinces)
            if (Globals.Provinces.TryGetValue(prov, out var province))
               province.Area = string.Empty;

         Globals.Areas.Remove(_areaName);
         _comboBox.Items.Remove(_areaName);
      }

      public void Undo()
      {
         Globals.Areas.Add(_areaName, _area);

         foreach (var prov in _area.Provinces)
            if (Globals.Provinces.TryGetValue(prov, out var province))
               province.Area = _areaName;

         _comboBox.Items.Add(_areaName);
         _comboBox.AutoCompleteCustomSource.Add(_areaName);
         _comboBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
         _comboBox.AutoCompleteSource = AutoCompleteSource.ListItems;


      }

      public void Redo()
      {
         Execute();
         _comboBox.AutoCompleteCustomSource.Remove(_areaName);
         _comboBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
         _comboBox.AutoCompleteSource = AutoCompleteSource.ListItems;
      }

      public string GetDescription()
      {
         return $"Remove area {_areaName}";
      }
   }
}