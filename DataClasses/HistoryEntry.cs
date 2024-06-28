using System;

namespace Editor.DataClasses;

public class HistoryEntry(DateTime date, string content, string? comment = null)
{
   public DateTime Date { get; set; } = date;
   public string Content { get; set; } = content;
   public string? Comment { get; set; } = comment;

   public override string ToString()
   {
      return $"{Date} \n\t {Content} \n\t {Comment}\n";
   }
}