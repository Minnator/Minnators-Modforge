using System.Diagnostics;
using System.Text;
using Editor.DataClasses.GameDataClasses;
using Editor.Saving;
using static Editor.Helper.ProvinceEnumHelper;

namespace Editor.DataClasses.Commands
{
   public sealed class CProvinceAttributeChange : ICommand
   {
      private readonly List<Province> _provinces;
      private readonly SaveablesCommandHelper _provinceSaveables;
      private readonly List<string> _oldValues = [];
      private readonly string _value;
      private readonly ProvAttrGet _attribute;
      private readonly ProvAttrSet _setter;

      public CProvinceAttributeChange(List<Province> provinces, string value, ProvAttrGet pa, ProvAttrSet ps, bool executeOnInit = true) 
      {
         _provinceSaveables = new([..provinces]);
         _provinces = provinces;
         _value = value;
         _attribute = pa;
         _setter = ps;

         foreach (var p in _provinces)
         {
            var attr = p.GetAttribute(_attribute);
            if (attr == null!)
               Debugger.Break();
            else if (attr is bool b)
               _oldValues.Add(b ? "yes" : "no");
            else
               _oldValues.Add(attr.ToString()!);
         }

         if (executeOnInit)
            Execute();
      }


      public void Execute() 
      {
         _provinceSaveables.Execute();
         InternalExecute();
         
      }

      private void InternalExecute()
      {
         foreach (var province in _provinces)
         {
            if (province.GetAttribute(_attribute)!.ToString()! == _value)
               continue;
            province.SetAttribute(_setter, _value);
         }
      }

      public void Undo()
      {
         _provinceSaveables.Undo();
         for (var i = 0; i < _provinces.Count; i++)
            _provinces[i].SetAttribute(_setter, _oldValues[i]);
      }

      public void Redo()
      {
         _provinceSaveables.Redo();
         InternalExecute();
         
      }

      public string GetDescription()
      {
         return _provinces.Count == 1
            ? $"Changed {_attribute} of {_provinces[0].Id} ({_provinces[0].GetLocalisation()}) to [{_value}]"
            : $"Changed {_attribute} of [{_provinces.Count}] provinces to [{_value}]";
      }

      public string GetDebugInformation(int indent)
      {
         var sb = new StringBuilder();
         SavingUtil.AddTabs(indent, ref sb);
         sb.AppendLine($"Changed {_attribute} to [{_value}] in provinces:");
         SavingUtil.AddTabs(indent, ref sb);
         foreach (var province in _provinces)
            sb.Append($"{province.Id}, ");
         return sb.ToString();
      }
   }
}