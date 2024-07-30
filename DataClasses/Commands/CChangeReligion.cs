using Editor.Commands;
using Editor.DataClasses.GameDataClasses;

namespace Editor.DataClasses.Commands
{
   public class CChangeReligion : ICommand
   {
      private readonly List<Province> _provinces = [];
      private readonly List<string> _oldReligions = [];
      private readonly string _value;

      public CChangeReligion(List<Province> provinces, string value, bool executeOnInit = true)
      {
         _provinces = provinces;
         _value = value;

         foreach (var p in _provinces)
            _oldReligions.Add(p.Religion);

         if (executeOnInit)
            Execute();
      }

      public void Execute()
      {
         foreach (var province in _provinces)
         {
            if (province.Religion == _value)
               continue;
            province.Religion = _value;
         }
      }

      public void Undo()
      {
         for (var i = 0; i < _provinces.Count; i++)
            _provinces[i].Religion = _oldReligions[i];
      }

      public void Redo()
      {
         Execute();
      }

      public string GetDescription()
      {
         return _provinces.Count == 1
            ? $"Changed religion of {_provinces[0].Id} ({_provinces[0].GetLocalisation()}) to [{_value}]"
            : $"Changed religion of [{_provinces.Count}] provinces to [{_value}]";
      }
   }
}