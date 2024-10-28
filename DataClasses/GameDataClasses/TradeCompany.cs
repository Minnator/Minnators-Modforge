using Editor.DataClasses.Commands;
using Editor.Helper;
using Editor.Interfaces;
using Editor.Parser;

namespace Editor.DataClasses.GameDataClasses
{
   public class TradeCompany : ProvinceCollection<Province>
   {
      public TradeCompany(string name, Color color) : base(name, color)
      {
         GenericName = string.Empty;
         SpecificName = string.Empty;
         Provinces = [];
      }

      public TradeCompany(string codeName, string genericName, string specificName, List<Province> provinces, Color color) : base(codeName, color)
      {
         GenericName = genericName;
         SpecificName = specificName;
         Provinces = [..provinces];
      }

      public static TradeCompany Empty => new ("Empty", Color.Empty);
      public string GenericName { get; set; }
      public string SpecificName { get; set; }
      public HashSet<Province> Provinces { get; set; }

      public string GetLocalisation()
      {
         return Localisation.GetLoc(Name);
      }

      public override string ToString()
      {
         return Name;
      }

      public override ModifiedData WhatAmI()
      {
         return ModifiedData.SaveTradeCompanies;
      }

      public override string SavingComment()
      {
         return Localisation.GetLoc(Name);
      }

      public override PathObj GetDefaultSavePath()
      {
         return new (["common", "trade_companies"]);
      }

      public override string GetSaveString(int tabs)
      {
         return "NOT YET SUPPORTED!";
      }

      public override string GetSavePromptString()
      {
         return $"Save trade companies file";
      }

      public EventHandler<ProvinceComposite> ColorChanged = delegate { };
      public EventHandler<ProvinceComposite> ItemAddedToArea = delegate { };
      public EventHandler<ProvinceComposite> ItemRemovedFromArea = delegate { };
      public EventHandler<ProvinceComposite> ItemModified = delegate { };

      public override void Invoke(ProvinceComposite composite)
      {
         ColorChanged.Invoke(this, composite);
      }

      public override void AddToEvent(EventHandler<ProvinceComposite> handler)
      {
         ColorChanged += handler;
      }

      public override void Invoke(CProvinceCollectionType type, ProvinceComposite composite)
      {
         switch (type)
         {
            case CProvinceCollectionType.Add:
               ItemAddedToArea.Invoke(this, composite);
               break;
            case CProvinceCollectionType.Remove:
               ItemRemovedFromArea.Invoke(this, composite);
               break;
            case CProvinceCollectionType.Modify:
               ItemModified.Invoke(this, composite);
               break;
         }
      }

      public override void AddToEvent(CProvinceCollectionType type, EventHandler<ProvinceComposite> eventHandler)
      {
         switch (type)
         {
            case CProvinceCollectionType.Add:
               ItemAddedToArea += eventHandler;
               break;
            case CProvinceCollectionType.Remove:
               ItemRemovedFromArea += eventHandler;
               break;
            case CProvinceCollectionType.Modify:
               ItemModified += eventHandler;
               break;
         }
      }
   }
}