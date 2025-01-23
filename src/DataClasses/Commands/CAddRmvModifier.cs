using System.Text;
using Editor.DataClasses.GameDataClasses;
using Editor.Saving;

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

      public List<Saveable> GetTargets() => _provinces.Cast<Saveable>().ToList();

      public string GetDescription()
      {
         return $"Add {_modifier.Name} to {_provinces.Count} provinces";
      }

      public string GetDebugInformation(int indent)
      {
         var sb = new StringBuilder();
         SavingUtil.AddTabs(indent, ref sb);
         if (_add)
            sb.AppendLine($"Added {_modifier.Name} to {_provinces.Count} provinces:");
         else
            sb.AppendLine($"Removed {_modifier.Name} from {_provinces.Count} provinces:");
         SavingUtil.AddTabs(indent, ref sb);
         foreach (var province in _provinces)
            sb.Append($"{province.Id}, ");
         return sb.ToString();
      }
   }
}