using Editor.Commands;
using Editor.DataClasses.GameDataClasses;

namespace Editor.DataClasses.Commands
{
   public class CChangeController : ICommand
   {
      private readonly List<Tag> _oldTags = [];
      private readonly List<Province> _provinces;
      private readonly Tag _value;

      public CChangeController(List<Province> provinces, string value, bool executeOnInit = true)
      {
         _provinces = provinces;
         _value = new (value);

         foreach (var p in _provinces)
            _oldTags.Add(p.Controller);

         if (executeOnInit) 
            Execute();
      }
      public void Execute()
      {
         foreach (var province in _provinces)
         {
            if (province.Controller == _value)
               continue;
            province.Controller = _value;
         }
      }

      public void Undo()
      {
         for (var i = 0; i < _provinces.Count; i++)
            _provinces[i].Controller = _oldTags[i];
      }

      public void Redo()
      {
         Execute();
      }

      public string GetDescription()
      {
         return _provinces.Count == 1 
            ? $"Changed controller of {_provinces[0].Id} ({_provinces[0].GetLocalisation()}) to [{_value}]" 
            : $"Changed controller of [{_provinces.Count}] provinces to [{_value}]";
      }
   }
}