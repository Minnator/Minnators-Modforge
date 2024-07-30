using Editor.Commands;
using Editor.DataClasses.GameDataClasses;

namespace Editor.DataClasses.Commands
{
   public class CChangeCapitalName : ICommand
   {
      private readonly List<Province> _provinces = [];
      private readonly List<string> _oldNames = [];
      private readonly string _value;

      public CChangeCapitalName(List<Province> provinces, string value, bool executeOnInit = true)
      {
         _provinces = provinces;
         _value = value;

         foreach (var p in _provinces)
            _oldNames.Add(p.Capital);

         if (executeOnInit)
            Execute();
      }

      public void Execute()
      {
         foreach (var province in _provinces)
         {
            if (province.Capital == _value)
               continue;
            province.Capital = _value;
         }
      }

      public void Undo()
      {
         for (var i = 0; i < _provinces.Count; i++)
            _provinces[i].Capital = _oldNames[i];
      }

      public void Redo()
      {
         Execute();
      }

      public string GetDescription()
      {
         return _provinces.Count == 1
            ? $"Changed capital of {_provinces[0].Id} ({_provinces[0].GetLocalisation()}) to [{_value}]"
            : $"Changed capital of [{_provinces.Count}] provinces to [{_value}]";
      }
   }
}