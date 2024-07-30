using Editor.Commands;
using Editor.DataClasses.GameDataClasses;

namespace Editor.DataClasses.Commands
{
   public class CSetDevelopment : ICommand
   {
      private readonly List<Province> _provinces = [];
      private readonly List<int> _oldTaxDevelopments = [];
      private readonly int _value;
      private readonly int _type;

      /// <summary>
      /// 0 = Tax, 1 = Production, 2 = Manpower
      /// </summary>
      /// <param name="provinces"></param>
      /// <param name="value"></param>
      /// <param name="type"></param>
      /// <param name="executeOnInit"></param>
      public CSetDevelopment(List<Province> provinces, int value, int type, bool executeOnInit = true)
      {
         _provinces = provinces;
         _value = value;
         _type = type;

         switch (type)
         {
            case 0:
               foreach (var p in _provinces)
                  _oldTaxDevelopments.Add(p.BaseTax);
               break;
            case 1:
               foreach (var p in _provinces)
                  _oldTaxDevelopments.Add(p.BaseProduction);
               break;
            case 2:
               foreach (var p in _provinces)
                  _oldTaxDevelopments.Add(p.BaseManpower);
               break;
         }

         if (executeOnInit)
            Execute();
      }

      public void Execute()
      {
         foreach (var province in _provinces)
         {
            switch (_type)
            {
               case 0:
                  if (province.BaseTax == _value)
                     continue;
                  province.BaseTax = _value;
                  break;
               case 1:
                  if (province.BaseProduction == _value)
                     continue;
                  province.BaseProduction = _value;
                  break;
               case 2:
                  if (province.BaseManpower == _value)
                     continue;
                  province.BaseManpower = _value;
                  break;
            }
         }
      }

      public void Undo()
      {
         for (var i = 0; i < _provinces.Count; i++)
         {
            switch (_type)
            {
               case 0:
                  _provinces[i].BaseTax = _oldTaxDevelopments[i];
                  break;
               case 1:
                  _provinces[i].BaseProduction = _oldTaxDevelopments[i];
                  break;
               case 2:
                  _provinces[i].BaseManpower = _oldTaxDevelopments[i];
                  break;
            }
         }
      }

      public void Redo()
      {
         Execute();
      }

      public string GetDescription()
      {
         return _provinces.Count == 1
            ? $"Changed {GetTypeName} development of {_provinces[0].Id} ({_provinces[0].GetLocalisation()}) to [{_value}]"
            : $"Changed {GetTypeName} development of [{_provinces.Count}] provinces to [{_value}]";
      }

      private string GetTypeName
      {
         get
         {
            return _type switch
            {
               0 => "Tax",
               1 => "Production",
               2 => "Manpower",
               _ => "Unknown"
            };
         }
      }
   }
}