using Editor.Commands;
using Editor.Helper;

namespace Editor.DataClasses.Commands
{
   public class CTradeCompanyProvinceChanged : ICommand
   {
      private int[] _deltaProvinces;
      private string _tradeCompanyKey;
      private bool _add;

      public CTradeCompanyProvinceChanged(string tradeCompanyKey, bool add, bool executeOnInit, params int[] deltaProvinces)
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
         return $"{(_add ? "Added" : "Removed")} ({_deltaProvinces.Length}) provinces to trade company {_tradeCompanyKey}";
      }
   }
}