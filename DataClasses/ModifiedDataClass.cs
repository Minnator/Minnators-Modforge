namespace Editor.DataClasses
{
   [Flags]
   public enum ModifiedData
   {
      SaveProvinces = 1 << 0,
      Areas = 1 << 1,
      Regions = 1 << 2,
      TradeNode = 1 << 3,
      SaveTradeCompanies = 1 << 4,
      ColonialRegions = 1 << 5,
      SuperRegions = 1 << 6,
      Continents = 1 << 7,
      ProvinceGroups = 1 << 8,
      EventModifiers = 1 << 9,
      Localisation = 1 << 10,
      Country = 1 << 11,
      Province = 1 << 12,
      All = SaveProvinces | Areas | Regions | TradeNode | SaveTradeCompanies | ColonialRegions | SuperRegions | Continents | ProvinceGroups | EventModifiers | Localisation | Country | Province,
   }

}