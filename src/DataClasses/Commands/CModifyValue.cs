﻿using System.Text;
using Editor.DataClasses.GameDataClasses;
using Editor.Saving;
using static Editor.Helper.ProvinceEnumHelper;

namespace Editor.DataClasses.Commands
{
   public class CModifyValue : ICommand
   {
      private readonly bool _increase;
      private readonly int _value;
      private readonly List<Province> _provinces = [];
      private readonly SaveablesCommandHelper _provinceSaveables;
      private readonly ProvAttrGet _attribute;
      private readonly ProvAttrSet _setter;

      public CModifyValue(List<Province> provinces, ProvAttrGet attribute, ProvAttrSet ps, int value, bool increase, bool executeOnInit = true)
      {
         if (provinces.Count == 0)
            return;
         _provinces = provinces;
         _provinceSaveables = new([.. provinces]);
         _attribute = attribute;
         _setter = ps;
         _value = value;
         _increase = increase;

         if (executeOnInit)
            Execute();
      }

      public void Execute()
      {
         _provinceSaveables.Execute();
         InternalExecute();
      }

      public void Undo()
      {
         _provinceSaveables.Undo();
         foreach (var province in _provinces)
         {
            object? attributeValue = province.GetAttribute(_attribute);
            int attr;

            if (attributeValue is int intValue)
            {
               attr = intValue;
            }
            else if (attributeValue is float floatValue)
            {
               attr = (int)floatValue;
            }
            else
            {
               throw new InvalidOperationException($"Cannot convert attribute {_attribute} to int or float.");
            }

            if (_increase)
               attr -= _value;
            else
               attr += _value;

            province.SetAttribute(_setter, attr.ToString());
         }
      }

      public void Redo()
      {
         _provinceSaveables.Redo();
         InternalExecute();
      }

      private void InternalExecute()
      {
         foreach (var province in _provinces)
         {
            var attributeValue = province.GetAttribute(_attribute);

            var attr = attributeValue switch
            {
               int intValue => intValue,
               float floatValue => (int)floatValue,
               _ => throw new InvalidOperationException($"Cannot convert attribute {_attribute} to int or float.")
            };

            if (_increase)
               attr += _value;
            else
               attr -= _value;

            province.SetAttribute(_setter, attr.ToString());
         }
      }

      public string GetDescription()
      {
         return _provinces.Count == 1
            ? $"{(_increase ? "Increased" : "Decreased")} {_attribute} of {_provinces[0].Id} ({_provinces[0].GetLocalisation()}) by [{_value}]"
            : $"{(_increase ? "Increased" : "Decreased")} {_attribute} of [{_provinces.Count}] provinces by [{_value}]";
      }

      public string GetDebugInformation(int indent)
      {
         var sb = new StringBuilder();
         SavingUtil.AddTabs(indent, ref sb);
         if (_increase)
            sb.AppendLine($"Increased {_attribute} of [{_provinces.Count}] provinces by [{_value}]:");
         else
            sb.AppendLine($"Decreased {_attribute} of [{_provinces.Count}] provinces by [{_value}]:");
         SavingUtil.AddTabs(indent, ref sb);
         foreach (var province in _provinces)
            sb.Append($"{province.Id}, ");
         return sb.ToString();
      }
   }
}