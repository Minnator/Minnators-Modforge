using Editor.DataClasses.Commands;
using Editor.Helper;
using Editor.Parser;

namespace Editor.DataClasses.GameDataClasses
{
   public class TradeCompany : ProvinceCollection<Province>
   {
      public TradeCompany(string name, Color color) : base(name, color)
      {
         GenericName = string.Empty;
         SpecificName = string.Empty;
      }

      public TradeCompany(string codeName, string genericName, string specificName, List<Province> provinces, Color color) : base(codeName, color)
      {
         GenericName = genericName;
         SpecificName = specificName;
         SubCollection = provinces;
      }

      public new static TradeCompany Empty => new ("Empty", Color.Empty);
      public string GenericName { get; set; }
      public string SpecificName { get; set; }

      public string GetLocalisation()
      {
         return Localisation.GetLoc(Name);
      }

      public override string ToString()
      {
         return Name;
      }

      public override SaveableType WhatAmI()
      {
         return SaveableType.TradeCompany;
      }

      public override string SavingComment()
      {
         return Localisation.GetLoc(Name);
      }

      public override PathObj GetDefaultSavePath()
      {
         return new (["common", "trade_companies", "00_trade_companies.txt"]);
      }

      public override string GetSaveString(int tabs)
      {
         return "NOT YET SUPPORTED!";
      }

      public override string GetSavePromptString()
      {
         return $"Save trade companies file";
      }

      public static EventHandler<ProvinceComposite> ColorChanged = delegate { };

      public override void ColorInvoke(ProvinceComposite composite)
      {
         ColorChanged.Invoke(this, composite);
      }

      public override void AddToColorEvent(EventHandler<ProvinceComposite> handler)
      {
         ColorChanged += handler;
      }

      public static EventHandler<ProvinceCollectionEventArguments<Province>> ItemsModified = delegate { };

      public override void Invoke(ProvinceCollectionEventArguments<Province> eventArgs)
      {
         ItemsModified.Invoke(this, eventArgs);
      }

      public override void AddToEvent(EventHandler<ProvinceCollectionEventArguments<Province>> eventHandler)
      {
         ItemsModified += eventHandler;
      }

      public override void RemoveGlobal()
      {
         Globals.TradeCompanies.Remove(Name);
      }

      public override void AddGlobal()
      {
         Globals.TradeCompanies.Add(Name, this);
      }
   }
}