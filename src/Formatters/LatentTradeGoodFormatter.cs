using System.Text;
using Editor.DataClasses.GameDataClasses;

namespace Editor.Formatters
{
   public class LatentTradeGoodFormatter : IFormatter
   {
      public void Format(StringBuilder sb, Province province)
      {
         //we don't need to format this as it's a subformatter
      }

      public void Format(StringBuilder sb, object obj)
      {
         // this can only be called if the key is latent_trade_good
         sb.Append("latent_trade_goods = { \n\t");
         sb.Append($"{obj.ToString() ?? ""} ");
         sb.Append("\n}");
      }

      /*    Following Format:         
            latent_trade_goods = { 
               val val
            }
            _
       */
      public void Format(StringBuilder sb, List<KeyValuePair<string, object>> toFormat)
      {
         // this can only be called if the key is latent_trade_good
         sb.Append("latent_trade_goods = { \n\t");
         foreach (var pair in toFormat)
         {
            sb.Append($"{pair.Value} ");
         }
         sb.Append("\n}");
      }
   }
}