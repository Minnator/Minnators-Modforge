namespace Editor.DataClasses.GameDataClasses
{
   public class ReligiousGroup(string name)
   {
      public string Name { get; set; } = name;
      public List<Religion> Religions { get; set; } = [];
   }

   public class Religion(string name)
   {
      public string Name { get; set; } = name;
      public Color Color { get; set; } = Color.Empty;
   }
}