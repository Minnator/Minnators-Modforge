namespace Editor.DataClasses
{



   [Flags]
   public enum ModifiedData
   {
      SaveProvinces = 1 << 0,
      SaveAreas = 1 << 1,
      SaveRegions = 1 << 2,
      SaveTradeNodes = 1 << 3,
      SaveTradeCompanies = 1 << 4,
      ColonialRegions = 1 << 5,
      SuperRegions = 1 << 6,
      Continents = 1 << 7,
      ProvinceGroups = 1 << 8,
      EventModifiers = 1 << 9,
      Localisation = 1 << 10,
      All = SaveProvinces | SaveAreas | SaveRegions | SaveTradeNodes | SaveTradeCompanies | ColonialRegions | SuperRegions | Continents | ProvinceGroups | EventModifiers | Localisation,
   }

   public class ModifiedDataClass(bool all)
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
      public bool Localisation { get; set; } = all;

      public ModifiedDataClass() : this(false) { }
   }
}