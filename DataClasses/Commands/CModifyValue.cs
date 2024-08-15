using System.Diagnostics;
using Editor.Commands;
using Editor.DataClasses.GameDataClasses;

namespace Editor.DataClasses.Commands
{
   public class CModifyValue : ICommand
   {
      private readonly bool _increase;
      private readonly int _value;
      private readonly List<Province> _provinces;
      private readonly ProvAttr _attribute;
      private readonly ProvAttrSetr _setter;

      public CModifyValue(List<Province> provinces, ProvAttr attribute, ProvAttrSetr ps, int value, bool increase, bool executeOnInit = true)
      {
         _provinces = provinces;
         _attribute = attribute;
         _setter = ps;
         _value = value;
         _increase = increase;

         if (executeOnInit)
            Execute();
      }

      public void Execute()
      {
         foreach (var province in _provinces)
         {
            var attr = (int)province.GetAttribute(_attribute)!;
            if (_increase)
               attr += _value;
            else
               attr -= _value;
            province.SetAttribute(_setter, attr.ToString());
         }
      }

      public void Undo()
      {
         foreach (var province in _provinces)
         {
            var attr = (int)province.GetAttribute(_attribute)!;
            if (_increase)
               attr -= _value;
            else
               attr += _value;
            province.SetAttribute(_setter, attr.ToString());
         }
      }

      public void Redo()
      {
         Execute();
      }

      public string GetDescription()
      {
         return _provinces.Count == 1
            ? $"{(_increase ? "Increased" : "Decreased")} {_attribute} of {_provinces[0].Id} ({_provinces[0].GetLocalisation()}) by [{_value}]"
            : $"{(_increase ? "Increased" : "Decreased")} {_attribute} of [{_provinces.Count}] provinces by [{_value}]";
      }
   }
}