using System.Collections.Generic;
using System.Text;
using Editor.DataClasses.Misc;
using Editor.Helper;
using Editor.Parser;
using Editor.Saving;
using static Editor.Saving.SavingUtil;

namespace Editor.DataClasses.GameDataClasses
{
   public class ColonialRegion : ProvinceCollection<Province>
   {
      public ColonialRegion(string name, Color color, ObjEditingStatus status = ObjEditingStatus.Modified) : base(name, color, status)
      {
      }

      public ColonialRegion(string name, Color color, ref PathObj path, ICollection<Province> provinces) : base(name, color, ref path, provinces)
      {
      }

      public int TaxIncome { get; set; }
      public int NativeSize { get; set; }
      public int NativeFerocity { get; set; }
      public int NativeHostileness { get; set; }
      public List<KeyValuePair<string, int>> Cultures { get; set; } = [];
      public List<KeyValuePair<string, int>> Religions { get; set; } = [];
      public List<KeyValuePair<string, int>> TradeGoods { get; set; } = [];
      public List<TriggeredName> Names { get; set; } = [];


      public override string ToString()
      {
         return Name;
      }

      public override void OnPropertyChanged(string? propertyName = null) {  }

      public override SaveableType WhatAmI()
      {
         return SaveableType.ColonialRegion;
      }

      public override string[] GetDefaultFolderPath()
      {
         return ["common", "colonial_regions"];
      }

      public override string GetFileEnding()
      {
         return ".txt";
      }

      public override KeyValuePair<string, bool> GetFileName()
      {
         return new("01_colonial_regions", false);
      }

      public override string SavingComment()
      {
         return Localisation.GetLoc(Name);
      }
      
      public override string GetSaveString(int tabs)
      {
         var sb = new StringBuilder();
         OpenBlock(ref tabs, Name, ref sb);
         AddColor(tabs, Color, ref sb);
         AddInt(tabs, TaxIncome, "tax_income", ref sb);
         AddInt(tabs, NativeSize, "native_size", ref sb);
         AddInt(tabs, NativeFerocity, "native_ferocity", ref sb);
         AddInt(tabs, NativeHostileness, "native_hostileness", ref sb);

         OpenBlock(ref tabs, "trade_goods", ref sb);
         foreach (var tradeGood in TradeGoods) 
            AddInt(tabs, tradeGood.Value, tradeGood.Key, ref sb);
         CloseBlock(ref tabs, ref sb);
         OpenBlock(ref tabs, "culture", ref sb);
         foreach (var culture in Cultures) 
            AddInt(tabs, culture.Value, culture.Key, ref sb);
         CloseBlock(ref tabs, ref sb);
         OpenBlock(ref tabs, "religion", ref sb);
         foreach (var religion in Religions) 
            AddInt(tabs, religion.Value, religion.Key, ref sb);
         CloseBlock(ref tabs, ref sb);
         AddFormattedProvinceList(tabs, GetProvinces(), "provinces", ref sb);
         // Names
         AddNames(tabs, Names, ref sb);
         CloseBlock(ref tabs, ref sb);
         return sb.ToString();
      }

      public override string GetSavePromptString()
      {
         return $"colonial_region: {Name}";
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
         Globals.ColonialRegions.Remove(Name);
      }

      public override void AddGlobal()
      {
         if (!Globals.ColonialRegions.TryAdd(Name, this))
            MessageBox.Show($"The ColonialRegion {Name} does already exist and can not be created.", $"ColonialRegion {Name} already exists!", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
   }

   public class TriggeredName(string name, IElement? trigger)
   {
      public string Name { get; init; } = name;
      public IElement? Trigger { get; set; } = trigger;

      public TriggeredName(string name) : this(name, null)
      {
      }

      public static TriggeredName Empty => new (string.Empty);

      public override string ToString()
      {
         return Name;
      }

      public override bool Equals(object? obj)
      {
         if (obj is TriggeredName other)
            return Name == other.Name;
         return false;
      }

      public override int GetHashCode()
      {
         return Name.GetHashCode();
      }

   }
}