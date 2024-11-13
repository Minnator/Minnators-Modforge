namespace Editor.DataClasses.Misc
{
   [Flags]
   public enum SaveableType
   {
      SaveProvinces = 1 << 0,
      Area = 1 << 1,
      Region = 1 << 2,
      TradeNode = 1 << 3,
      TradeCompany = 1 << 4,
      ColonialRegion = 1 << 5,
      SuperRegion = 1 << 6,
      Continent = 1 << 7,
      ProvinceGroup = 1 << 8,
      EventModifiers = 1 << 9,
      Localisation = 1 << 10,
      Country = 1 << 11,
      Province = 1 << 12,
      All = SaveProvinces | Area | Region | TradeNode | TradeCompany | ColonialRegion | SuperRegion | Continent | ProvinceGroup | EventModifiers | Localisation | Country | Province,
   }

}