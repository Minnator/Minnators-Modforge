using System.Diagnostics;
using System.Text;
using Editor.DataClasses.GameDataClasses;
using Editor.Saving;
using static Editor.Helper.ProvinceEnumHelper;

namespace Editor.DataClasses.Commands
{
   public sealed class CProvinceAttributeChange : CTextEditingWrapper
   {
      private readonly List<Province> _provinces;
      private readonly List<string> _oldValues = [];
      private string _value;
      private readonly ProvAttrGet _attribute;
      private readonly ProvAttrSet _setter;

      public CProvinceAttributeChange(List<Province> provinces, string value, ProvAttrGet pa, ProvAttrSet ps, bool executeOnInit = true) 
      {
         _provinces = provinces;
         _value = value;
         _attribute = pa;
         _setter = ps;

         foreach (var p in _provinces)
         {
            var attr = p.GetAttribute(_attribute);
            if (attr is bool b)
               _oldValues.Add(b ? "yes" : "no");
            else
               _oldValues.Add(attr.ToString()!);
         }

         if (executeOnInit)
            Execute();
      }


      public override void Execute() 
      {
         base.Execute([.. _provinces]);
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

      public override void Undo()
      {
         base.Undo();
         for (var i = 0; i < _provinces.Count; i++)
            _provinces[i].SetAttribute(_setter, _oldValues[i]);
      }

      public override void Redo()
      {
         base.Redo();
         InternalExecute();
         
      }

      public override string GetDescription()
      {
         return _provinces.Count == 1
            ? $"Changed {_attribute} of {_provinces[0].Id} ({_provinces[0].GetLocalisation()}) to [{_value}]"
            : $"Changed {_attribute} of [{_provinces.Count}] provinces to [{_value}]";
      }

      public override string GetDebugInformation(int indent)
      {
         var sb = new StringBuilder();
         SavingUtil.AddTabs(indent, ref sb);
         sb.AppendLine($"Changed {_attribute} to [{_value}] in provinces:");
         SavingUtil.AddTabs(indent, ref sb);
         foreach (var province in _provinces)
            sb.Append($"{province.Id}, ");
         return sb.ToString();
      }

      public override void SetValue(string value)
      {
         Debug.Assert(_setter == ProvAttrSet.capital);
         _value = value;
      }
   }
}