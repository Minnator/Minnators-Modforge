using System.Text;
using Editor.DataClasses.GameDataClasses;
using Editor.Saving;
using static Editor.Helper.ProvinceEnumHelper;

namespace Editor.DataClasses.Commands
{
   public class CAddRemoveProvinceAttribute : SaveableCommandBasic
   {
      private readonly List<Province> _provinces;
      private readonly bool _add;
      private readonly ProvAttrGet _attribute;
      private readonly ProvAttrSet _setter;
      private readonly ProvAttrSet _remover;
      private readonly string _value;

      public CAddRemoveProvinceAttribute(List<Province> provinces, string value, ProvAttrGet pa, ProvAttrSet ps, ProvAttrSet pr, bool add, bool executeOnInit = true)
      {
         _provinces = provinces;
         _value = value;
         _attribute = pa;
         _setter = ps;
         _remover = pr;
         _add = add;

         if (executeOnInit)
            Execute();
      }

      public override void Execute()
      {
         base.Execute([.. _provinces]);
         InternalExecute();
      }

      public override void Undo()
      {
         base.Undo();
         foreach (var province in _provinces) 
            province.SetAttribute(_remover, _value);
      }

      public override void Redo()
      {
         base.Redo();
         InternalExecute();
      }

      private void InternalExecute()
      {
         foreach (var province in _provinces) 
            province.SetAttribute(_setter, _value);
      }

      public override string GetDescription()
      {
         return _provinces.Count == 1
            ? $"{(_add ? "Added" : "Removed")} {_attribute} {_value} to {_provinces[0].Id} ({_provinces[0].TitleLocalisation})"
            : $"{(_add ? "Added" : "Removed")} {_attribute} {_value} to [{_provinces.Count}] provinces";
      }

      public override string GetDebugInformation(int indent)
      {
         var sb = new StringBuilder();
         SavingUtil.AddTabs(indent, ref sb);
         if (_add)
            sb.AppendLine($"Added {_attribute} \"{_value}\" to {_provinces.Count} provinces:");
         else
            sb.AppendLine($"Removed {_attribute} \"{_value}\" from {_provinces.Count} provinces:");
         SavingUtil.AddTabs(indent, ref sb);
         foreach (var province in _provinces)
            sb.Append($"{province.Id}, ");
         return sb.ToString();
      }
   }
}