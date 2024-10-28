using Editor.DataClasses.Commands;
using Editor.Helper;
using Editor.Interfaces;

namespace Editor.DataClasses.GameDataClasses
{
   public class ColonialRegion : ProvinceCollection<Province>
   {
      public ColonialRegion(string name, Color color) : base(name, color)
      {
      }

      public int TaxIncome { get; set; }
      public int NativeSize { get; set; }
      public int NativeFerocity { get; set; }
      public int NativeHostileness { get; set; }
      public List<KeyValuePair<string, int>> Cultures { get; set; } = [];
      public List<KeyValuePair<string, int>> Religions { get; set; } = [];
      public List<KeyValuePair<string, int>> TradeGoods { get; set; } = [];
      public HashSet<Province> Provinces { get; set; } = [];
      public List<TriggeredName> Names { get; set; } = [];


      public override string ToString()
      {
         return Name;
      }

      public override ModifiedData WhatAmI()
      {
         return ModifiedData.ColonialRegions;
      }

      public override string SavingComment()
      {
         return Localisation.GetLoc(Name);
      }

      public override PathObj GetDefaultSavePath()
      {
         return new (["common", "colonial_regions"]);
      }

      public override string GetSaveString(int tabs)
      {
         //TODO
         return "NOT YET SUPPORTED!";
      }

      public override string GetSavePromptString()
      {
         return $"Save colonial regions file";
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

   public class TriggeredName(string name, string trigger)
   {
      public string Name { get; init; } = name;
      public string Trigger { get; set; } = trigger;

      public TriggeredName(string name) : this(name, string.Empty)
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