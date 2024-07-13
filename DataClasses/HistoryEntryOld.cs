namespace Editor.DataClasses;

public class HistoryEntry(DateTime date)
{
   public DateTime Date { get; set; } = date;
   public string Comment { get; set; } = string.Empty;
   public List<Effect> Effects { get; set; } = [];

   public void Apply(Province province)
   {
      foreach (var effect in Effects)
      {
         effect.ExecuteProvince(province);
      }
   }
}


public class HistoryEntryOld(DateTime date, string content, string? comment = null)
{
   public DateTime Date { get; set; } = date;
   public string Content { get; set; } = content;
   public string? Comment { get; set; } = comment;
   public List<Group> Groups { get; set; } = [];
   public int Start { get; set; }
   public int End { get; set; }

   public HistoryEntryOld(DateTime date, string content, List<Group> groups, string? comment = null) : this(date, content, comment)
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

public struct MultilineAttribute(string name, List<KeyValuePair<string, string>> values)
{
   public string Name { get; set; } = name;
   public List<KeyValuePair<string, string>> Values { get; set; } = values;
}