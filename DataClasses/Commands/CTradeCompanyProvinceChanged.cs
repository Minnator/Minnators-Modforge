using Editor.Commands;
using Editor.DataClasses.GameDataClasses;
using Editor.Helper;

namespace Editor.DataClasses.Commands
{
   public class CTradeCompanyProvinceChanged : ICommand
   {
      private List<Province> _deltaProvinces;
      private string _tradeCompanyKey;
      private bool _add;

      public CTradeCompanyProvinceChanged(string tradeCompanyKey, bool add, bool executeOnInit, List<Province> deltaProvinces)
      {
         _deltaProvinces = deltaProvinces;
         _tradeCompanyKey = tradeCompanyKey;
         _add = add;

         if (executeOnInit)
            Execute();
      }

      public void Execute()
      {
         ProvinceCollectionEditor.ModifyTradeCompany(_tradeCompanyKey, _add, _deltaProvinces);
      }

      public void Undo()
      {
         ProvinceCollectionEditor.ModifyTradeCompany(_tradeCompanyKey, !_add, _deltaProvinces);
      }

      public void Redo()
      {
         Execute();
      }

      public string GetDescription()
      {
         return $"{(_add ? "Added" : "Removed")} ({_deltaProvinces.Count}) provinces to trade company {_tradeCompanyKey}";
      }
   }
}