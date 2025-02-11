using Editor.ErrorHandling;

namespace Editor.DataClasses.GameDataClasses
{
   public class Building(string name) 
   {
      public string Name { get; set; } = name;


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

      public static bool TryParse(string str, out Building building)
      {
         building = Globals.Buildings.Find(x => x.Name.Equals(str))!;
         if (building == null)
            building = Building.Empty;
         return building != null;
      }

      public static IErrorHandle GeneralParse(string str, out object result)
      {
         if (TryParse(str, out var res))
         {
            result = res;
            return ErrorHandle.Success;
         }

         result = Empty;
         return new ErrorObject(ErrorType.TempParsingError, "Building not found: " + str);
      }

      public static Building Empty { get; } = new ("UNDEFINED");
   }
}