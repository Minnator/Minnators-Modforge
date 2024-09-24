using Editor.Commands;
using Editor.DataClasses.GameDataClasses;

namespace Editor.DataClasses.Commands
{
   public class CModifyExitingArea : ICommand
   {
      private readonly string _areaName;
      private readonly List<int> _deltaProvinces;
      private readonly bool _add;
      private readonly List<KeyValuePair<int, string>> _oldAreasPerId = [];

      public CModifyExitingArea(string areaName, List<int> deltaProvinces, bool add, bool executeOnInit = true)
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
            var newProvIds = new int[_deltaProvinces.Count + area.Provinces.Length];
            area.Provinces.CopyTo(newProvIds, 0);
            _deltaProvinces.CopyTo(newProvIds, area.Provinces.Length);
            area.Provinces = newProvIds;
            foreach (var prov in _deltaProvinces)
            {
               if (!Globals.Provinces.TryGetValue(prov, out var province))
                  continue;
               _oldAreasPerId.Add(new (prov, province.Area));
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
               province.Area = string.Empty;
            }
            area.Provinces = area.Provinces.Except(_deltaProvinces).ToArray();
         }
      }

      public void Undo()
      {
         if (!Globals.Areas.TryGetValue(_areaName, out var area))
            return;

         foreach (var kvp in _oldAreasPerId)
         {
            if (!Globals.Provinces.TryGetValue(kvp.Key, out var province))
               continue;
            province.Area = kvp.Value;
         }
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
}