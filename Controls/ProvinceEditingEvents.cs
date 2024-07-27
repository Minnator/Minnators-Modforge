using Editor.DataClasses.GameDataClasses;

namespace Editor.Controls
{
   public class ProvinceEditedEventArgs(List<Province> province, object value) : EventArgs
   {
      public List<Province> Province { get; set; } = province;
      public object Value { get; set; } = value;
   }

   public static class ProvinceEditingEvents
   {
      public static void OnOwnerChanged(object? sender, ProvinceEditedEventArgs e)
      {

      }
   }
}