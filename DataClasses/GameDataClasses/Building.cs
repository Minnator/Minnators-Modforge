namespace Editor.DataClasses.GameDataClasses
{
   public class Building(string name) 
   {
      public string Name { get; } = name;


      public override string ToString()
      {
         return Name;
      }

      public override bool Equals(object? obj)
      {
         if (obj == null || GetType() != obj.GetType())
            return false;

         return Name == ((Building)obj).Name;
      }

      public override int GetHashCode()
      {
         return Name.GetHashCode();
      }
   }
}