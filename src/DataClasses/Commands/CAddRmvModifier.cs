using Editor.DataClasses.GameDataClasses;

namespace Editor.DataClasses.Commands
{
   public class CAddRmvModifier : ICommand
   {
      private readonly List<Province> _provinces;
      private readonly ModifierAbstract _modifier;
      private readonly ModifierType _type;
      private readonly bool _add;


      public CAddRmvModifier(List<Province> provinces, ModifierAbstract modifier, ModifierType type, bool add = true, bool executeOnInit = true)
      {
         _provinces = provinces;
         _modifier = modifier;
         _type = type;
         _add = add;

         if (executeOnInit)
            Execute();
      }

      public void Execute()
      {
         if (_add)
            foreach (var province in _provinces)
               province.AddModifier(_type, _modifier);
         else
            foreach (var province in _provinces)
               province.RemoveModifier(_modifier.Name, _type);
      }

      public void Undo()
      {
         if (_add)
            foreach (var province in _provinces)
               province.RemoveModifier(_modifier.Name, _type);
         else
            foreach (var province in _provinces)
               province.AddModifier(_type, _modifier);
      }

      public void Redo()
      {
         Execute();
      }

      public string GetDescription()
      {
         return $"Add {_modifier.Name} to {_provinces.Count} provinces";
      }
   }
}