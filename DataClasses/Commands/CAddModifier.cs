using Editor.Commands;
using Editor.DataClasses.GameDataClasses;

namespace Editor.DataClasses.Commands
{
   public class CAddModifier : ICommand
   {
      private readonly List<Province> _provinces;
      private readonly string _scope;
      private readonly ModifierAbstract _modifier;
      private readonly ModifierType _type;


      public CAddModifier(List<Province> provinces, string scope, ModifierAbstract modifier, ModifierType type, bool executeOnInit = true)
      {
         _provinces = provinces;
         // TODO improve Scope Support to allow for multiple and or complex scopes
         _scope = scope;
         _modifier = modifier;
         _type = type;

         if (executeOnInit)
            Execute();
      }

      public void Execute()
      {
         foreach (var province in _provinces) 
            province.AddModifier(_type, _modifier, true);
      }

      public void Undo()
      {
         foreach (var province in _provinces) 
            province.AddModifier(_type, _modifier, false);
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