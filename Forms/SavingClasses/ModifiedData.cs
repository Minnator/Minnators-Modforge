namespace Editor.Forms.SavingClasses
{
   public class ModifiedData(bool all)
   {
      public bool SaveProvinces { get; set; } = all;
      public bool SaveAreas { get; set; } = all;
      public bool SaveRegions { get; set; } = all;
      public bool SaveTradeNodes { get; set; } = all;
      public bool SaveTradeCompanies { get; set; } = all;
      public bool ColonialRegions { get; set; } = all;
      public bool SuperRegions { get; set; } = all;
      public bool Continents { get; set; } = all;
      public bool ProvinceGroups { get; set; } = all;
      public bool EventModifiers { get; set; } = all;

      public ModifiedData() : this(false) {  }
   }
}