namespace Editor.DataClasses.GameDataClasses
{
   public class Building(string name) //TODO expand if needed
   {
      public string Name { get; set; } = name;


      public override string ToString()
      {
         return Name;
      }
   }
}