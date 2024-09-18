using Editor.Commands;
using Editor.Helper;

namespace Editor.DataClasses.Commands
{
   public class CTradeCompanyProvinceChanged : ICommand
   {
      private List<int> _deltaProvinces;
      private string _tradeCompanyKey;
      private bool _add;

      public CTradeCompanyProvinceChanged(List<int> deltaProvinces, string tradeCompanyKey, bool add, bool executeOnInit)
      {
         _deltaProvinces = deltaProvinces;
         _tradeCompanyKey = tradeCompanyKey;
         _add = add;

         if (executeOnInit)
            Execute();
      }

      public void Execute()
      {
         ProvinceCollectionEditor.ModifyTradeCompany(_tradeCompanyKey, _deltaProvinces, _add);
      }

      public void Undo()
      {
         ProvinceCollectionEditor.ModifyTradeCompany(_tradeCompanyKey, _deltaProvinces, !_add);
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