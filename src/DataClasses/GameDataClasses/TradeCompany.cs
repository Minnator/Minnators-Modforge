using Editor.DataClasses.Misc;
using Editor.Helper;
using Editor.Saving;

namespace Editor.DataClasses.GameDataClasses
{
   public class TradeCompany : ProvinceCollection<Province>
   {
      public TradeCompany(string genericName, string specificName, string name, Color color, ObjEditingStatus status = ObjEditingStatus.Modified) : base(name, color, status)
      {
         GenericName = genericName;
         SpecificName = specificName;
      }

      public TradeCompany(string name, Color color, ref PathObj path, ICollection<Province> provinces, string genericName, string specificName) : base(name, color, ref path, provinces)
      {
         GenericName = genericName;
         SpecificName = specificName;
      }

      public override void OnPropertyChanged(string? propertyName = null) { }
      

      public new static TradeCompany Empty => new (string.Empty, string.Empty, string.Empty, Color.Empty, ObjEditingStatus.Immutable);
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

      public override string[] GetDefaultFolderPath()
      {
         return ["common", "trade_companies"];
      }

      public override string GetFileEnding()
      {
         return ".txt";
      }

      public override string SavingComment()
      {
         return Localisation.GetLoc(Name);
      }

      public override KeyValuePair<string, bool> GetFileName()
      {
         return new("00_trade_companies", true);
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