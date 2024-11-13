using System.Diagnostics;
using Editor.DataClasses.GameDataClasses;
using static Editor.Helper.ProvinceEnumHelper;

namespace Editor.DataClasses.Commands
{
   public class CProvinceAttributeChange : ICommand
   {
      private readonly List<Province> _provinces;
      private readonly List<string> _oldValues = [];
      private readonly string _value;
      private readonly ProvAttrGet _attribute;
      private readonly ProvAttrSet _setter;

      public CProvinceAttributeChange(List<Province> provinces, string value, ProvAttrGet pa, ProvAttrSet ps, bool executeOnInit = true)
      {
         _provinces = provinces;
         _value = value;
         _attribute = pa;
         _setter = ps;

         foreach (var p in _provinces)
         {
            var attr = p.GetAttribute(_attribute);
            if (attr == null)
               Debugger.Break();
            else
               _oldValues.Add(attr.ToString()!);
         }

         if (executeOnInit)
            Execute();
      }


      public void Execute()
      {
         foreach (var province in _provinces)
         {
            if (province.GetAttribute(_attribute)!.ToString()! == _value)
               continue;
            province.SetAttribute(_setter, _value);
         }
      }

      public void Undo()
      {
         for (var i = 0; i < _provinces.Count; i++)
            _provinces[i].SetAttribute(_setter, _oldValues[i]);
      }

      public void Redo()
      {
         Execute();
      }

      public string GetDescription()
      {
         return _provinces.Count == 1
            ? $"Changed {_attribute} of {_provinces[0].Id} ({_provinces[0].GetLocalisation()}) to [{_value}]"
            : $"Changed {_attribute} of [{_provinces.Count}] provinces to [{_value}]";
      }
   }
}