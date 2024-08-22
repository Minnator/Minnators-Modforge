using System.Text;
using Editor.DataClasses.GameDataClasses;

namespace Editor.Formatters
{
   public static class FormatterController
   {
      public static Dictionary<ProvAttr, IFormatter> SubFormatters { get; set; } = new()
      {
         { ProvAttr.latent_trade_good, new LatentTradeGoodFormatter() },
         { ProvAttr.history, new HistoryFormatter() }
      };

      /// <summary>
      /// If there is a subformatter for the given key, it will be called to format the value and append it to the stringbuilder
      /// </summary>
      /// <param name="sb"></param>
      /// <param name="toFormat"></param>
      public static void Format(StringBuilder sb, List<KeyValuePair<string, object>> toFormat)
      {
         foreach (var kvp in toFormat)
         {
            if (SubFormatters.TryGetValue((ProvAttr)Enum.Parse(typeof(ProvAttr), kvp.Key), out var formatter))
               formatter.Format(sb, kvp.Value);
            else
               Globals.ErrorLog.Write($"Can not find formatter for: [{kvp.Key}] in [{nameof(FormatterController)}]");
         }
      }
   }
}