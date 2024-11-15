using Editor.DataClasses.Misc;
using Editor.Helper;

namespace Editor.DataClasses.GameDataClasses
{
   public class ColonialRegion(string name, Color color) : ProvinceCollection<Province>(name, color)
   {
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
         throw new NotImplementedException();
      }

      public override string SavingComment()
      {
         return Localisation.GetLoc(Name);
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
         Globals.ColonialRegions.Add(Name, this);
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