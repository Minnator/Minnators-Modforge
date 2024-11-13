using System.Text;
using Editor.DataClasses.GameDataClasses;

namespace Editor.Formatters
{
   public class HistoryFormatter : IFormatter
   {
      public void Format(StringBuilder sb, object obj)
      {
         if (obj is not IList<HistoryEntry> entries)
            return;

         List<KeyValuePair<string, object>> entryList = [];
         foreach (var entry in entries)
         {
            entryList.Add(new(entry.Date.ToString("yyyy.MM.dd"), entry));
         }

         Format(sb, entryList);
      }
      /*   Following Format:         
            date = {
               key = value
               key = {
                  key = value
               }
            }
            _
      */
      public void Format(StringBuilder sb, List<KeyValuePair<string, object>> toFormat)
      {
         foreach (var pair in toFormat)
         {
            sb.Append($"{pair.Key} = {{\n");
            FormatterController.Format(sb, [pair]);
            sb.Append("\n}\n");
         }
      }


   }
}