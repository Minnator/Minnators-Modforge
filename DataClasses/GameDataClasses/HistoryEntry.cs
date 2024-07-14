namespace Editor.DataClasses.GameDataClasses;

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

