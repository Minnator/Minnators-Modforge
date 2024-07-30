using Editor.Commands;
using Editor.DataClasses.GameDataClasses;

namespace Editor.DataClasses.Commands
{
   public class CChangeCulture : ICommand
   {
      private readonly List<Province> _provinces = [];
      private readonly List<string> _oldCultures = [];
      private readonly string _value;

      public CChangeCulture(List<Province> provinces, string value, bool executeOnInit = true)
      {
         _provinces = provinces;
         _value = value;

         foreach (var p in _provinces)
            _oldCultures.Add(p.Culture);

         if (executeOnInit)
            Execute();
      }

      public void Execute()
      {
         foreach (var province in _provinces)
         {
            if (province.Culture == _value)
               continue;
            province.Culture = _value;
         }
      }

      public void Undo()
      {
         for (var i = 0; i < _provinces.Count; i++)
            _provinces[i].Culture = _oldCultures[i];
      }

      public void Redo()
      {
         Execute();
      }

      public string GetDescription()
      {
         return _provinces.Count == 1
            ? $"Changed culture of {_provinces[0].Id} ({_provinces[0].GetLocalisation()}) to [{_value}]"
            : $"Changed culture of [{_provinces.Count}] provinces to [{_value}]";
      }
   }
}