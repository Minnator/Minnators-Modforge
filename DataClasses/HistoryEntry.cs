using System;
using System.Collections.Generic;

namespace Editor.DataClasses;
#nullable enable

public class HistoryEntry(DateTime date, string content, string? comment = null)
{
   public DateTime Date { get; set; } = date;
   public string Content { get; set; } = content;
   public string? Comment { get; set; } = comment;
   public List<Group> Groups { get; set; } = [];
   public int Start { get; set; } = 0;
   public int End { get; set; } = 0;

   public HistoryEntry(DateTime date, string content, List<Group> groups, string? comment = null) : this(date, content, comment)
   {
      Groups = groups;
   }

   public override string ToString()
   {
      return $"{Date} \n\t {Content} \n\t {Comment}\n";
   }


}

public struct Group(int start, int end, string content)
{
   public string Content { get; set; } = content;
   public int Start { get; set; } = start;
   public int End { get; set; } = end;
}

