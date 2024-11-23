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
      EventModifier = 1 << 9,
      Localisation = 1 << 10,
      Country = 1 << 11,
      Province = 1 << 12,
      Terrain = 1 << 13,
      All = SaveProvinces | Area | Region | TradeNode | TradeCompany | ColonialRegion | SuperRegion | Continent | ProvinceGroup | EventModifier | Localisation | Country | Province | Terrain,
   }

}