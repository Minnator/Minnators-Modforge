namespace Editor.DataClasses;

public class Area(string name, int[] provinces)
{
   public string Name { get; } = name;
   // Contains the provinces in the area will be editable as the array is only a few elements long
   public int[] Provinces { get; set; } = provinces;
   public string Edict { get; set; } = string.Empty;
   public float Prosperity { get; set; } = 0;
   public bool IsStated { get; set; } = false;
   public string Region { get; set; } = string.Empty;

   public override bool Equals(object? obj)
   {
      if (obj is Area other)
         return Name == other.Name;
      return false;
   }

   public override int GetHashCode()
   {
      return Name.GetHashCode();
   }
}