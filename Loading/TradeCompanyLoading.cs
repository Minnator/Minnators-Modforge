using Editor.Helper;

namespace Editor.Loading
{
   public static class TradeCompanyLoading
   {

      public static void Load()
      {
         if(!FilesHelper.GetModOrVanillaPath(out var path, "common", "trade_companies", "00_trade_companies.txt"))
         {
            Globals.ErrorLog.Write("Error: 00_trade_companies.txt not found!");
            return;
         }

         

      }

   }
}