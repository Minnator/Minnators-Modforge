namespace Editor.DataClasses.GameDataClasses
{
   public enum TCICategory
   {
      company_garrison,
      harbor,
      local_venture,
      foreign_influence,
      governance
   }

   public class TradeCompanyInvestment(string name)
   {
      public string Name { get; init; } = name;
      public string Triggers = string.Empty; // TODO: implement triggers just the string for now until triggers are implemented
      public string UpgradesTo { get; set; } = string.Empty;
      public TCICategory Category { get; set; } = TCICategory.company_garrison;
      public int Cost { get; set; } = 0;
      public string Sprite { get; set; } = string.Empty;
      public List<Modifier> ProvinceModifiers = [];
      public List<Modifier> AreaModifiers = [];
      public List<Modifier> RegionModifiers = [];
      public List<Modifier> OwnerRegionModifiers = [];
      public List<Modifier> OwnerModifiers = [];
      
      public override bool Equals(object? obj)
      {
         if (obj == null || GetType() != obj.GetType())
            return false;

         var other = (TradeCompanyInvestment)obj;
         return Name == other.Name;
      }

      public override int GetHashCode()
      {
         return Name.GetHashCode();
      }

      public override string ToString()
      {
         return Name;
      }
   }
}