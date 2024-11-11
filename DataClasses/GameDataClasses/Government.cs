namespace Editor.DataClasses.GameDataClasses;

public class Government(string name)
{
   public string Name { get; } = name;
   public List<ReformLevel> ReformLevels = [];
   public GovernmentReform BasicReform = GovernmentReform.Empty;
   public List<GovernmentReform> LegacyReforms = [];
   public List<HashSet<GovernmentReform>> ExclusiveReforms = [];
   public Color Color = Color.Empty;

   public static Government Empty => new (string.Empty);

   public List<GovernmentReform> AllReforms
   {
      get
      {
         List<GovernmentReform> allReforms = [];
         foreach (var level in ReformLevels)
            allReforms.AddRange(level.Reforms);
         return allReforms;
      }
   }

   public List<string> AllReformNames
   {
      get
      {
         var allReforms = AllReforms;
         return allReforms.Select(reform => reform.Name).ToList();
      }
   }

   public override string ToString()
   {
      return Name;
   }

   public override bool Equals(object? obj)
   {
      if (obj is Government other)
         return Name == other.Name;
      return false;
   }

   public override int GetHashCode()
   {
      return Name.GetHashCode();
   }
}

public class ReformLevel(string name)
{
   public string Name { get; } = name;
   public List<GovernmentReform> Reforms = [];

   public static ReformLevel Empty => new (string.Empty);

   public override string ToString()
   {
      return Name;
   }

   public override bool Equals(object? obj)
   {
      if (obj is ReformLevel other)
         return Name == other.Name;
      return false;
   }

   public override int GetHashCode()
   {
      return Name.GetHashCode();
   }
}