using Editor.Commands;
using Editor.DataClasses.GameDataClasses;

namespace Editor.DataClasses.Commands
{
   public class CProvinceAttributeChange : ICommand
   {
      private readonly List<Province> _provinces;
      private readonly List<string> _oldValues = [];
      private readonly string _value;
      private readonly string _attribute;

      public CProvinceAttributeChange(List<Province> provinces, string value, string attribute, bool executeOnInit = true)
      {
         _provinces = provinces;
         _value = value;
         _attribute = attribute;

         foreach (var p in _provinces)
            _oldValues.Add(p.GetAttribute(_attribute)!.ToString()!);

         if (executeOnInit)
            Execute();
      }


      public void Execute()
      {
         foreach (var province in _provinces)
         {
            if (province.GetAttribute(_attribute)!.ToString()! == _value)
               continue;
            province.SetAttribute(_attribute, _value);
         }
      }

      public void Undo()
      {
         for (var i = 0; i < _provinces.Count; i++)
            _provinces[i].SetAttribute(_attribute, _oldValues[i]);
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