using Editor.DataClasses.GameDataClasses;

namespace Editor.Commands
{
   public class CChangeOwner : ICommand
   {
      private readonly List<Province> _provinces;
      private readonly List<Tag> _oldTags = [];
      private readonly Tag _value;
      public CChangeOwner(List<Province> province, Tag value, bool executeOnInit = true)
      {
         _provinces = province;
         _value = value;

         foreach (var p in _provinces)
            _oldTags.Add(p.Owner);

         if (executeOnInit)
            Execute();
      }

      public void Execute()
      {
         foreach (var province in _provinces) 
            province.Owner = _value;
      }

      public void Undo()
      {
         for (var i = 0; i < _provinces.Count; i++)
            _provinces[i].Owner = _oldTags[i];
      }

      public void Redo()
      {
         Execute();
      }

      public string GetDescription()
      {
         return _provinces.Count == 1 
            ? $"Changed owner of {_provinces[0].Id} ({_provinces[0].GetLocalisation()}) to [{_value}]" 
            : $"Changed owner of [{_provinces.Count}] provinces to [{_value}]";
      }
   }
}