using System.Text;
using Editor.ErrorHandling;
using Editor.Helper;
using Editor.Loading.Enhanced;
using Editor.Parser;
using Editor.Saving;

namespace Editor.DataClasses.GameDataClasses;

public class Price(float price) : Saveable, IStringify
{
   private TradeGood _tradeGood = TradeGood.Empty;

   public TradeGood TradeGood
   {
      get => _tradeGood;
      set
      {
         _tradeGood = value;
         _tradeGood.Price = this;
      }
   }

   public float Value { get; set; } = price;

   public static Price Empty { get; } = new (-1f);

   public override void OnPropertyChanged(string? propertyName = null) { }

   public override SaveableType WhatAmI() => SaveableType.Price;

   public override string[] GetDefaultFolderPath() => ["common", "prices"];

   public override string GetFileEnding() => ".txt";
   public override KeyValuePair<string, bool> GetFileName() => new("00_prices", false);

   public override string SavingComment() => Localisation.GetLoc(_tradeGood.Name);

   public override string GetSaveString(int tabs)
   {
      var sb = new StringBuilder();
      SavingUtil.OpenBlock(ref tabs, _tradeGood.Name, ref sb);
      SavingUtil.AddFloat(tabs, Value, "base_price", ref sb);
      SavingUtil.CloseBlock(ref tabs, ref sb);
      return sb.ToString();
   }

   public override string GetSavePromptString()
   {
      return $"Save price for: {_tradeGood.Name}";
   }

   public string Stringify() => _tradeGood.Name;
}

public class TradeGood(string name) : Saveable
{
   private Price _price = Price.Empty;
   public string Name { get; } = name;

   public Price Price
   {
      get => _price;
      set
      {
         _price = value;
      }
   }

   public Color Color { get; set; }
   public bool IsLatent { get; set; } = false;
   public bool IsValuable { get; set; } = false;
   public int RNWLatentChance { get; set; } = -1;

   public List<KeyValuePair<string, string>> Modifier { get; set; } = [];
   public List<KeyValuePair<string, string>> ProvinceModifier { get; set; } = [];
   public EnhancedBlock? Trigger { get; set; } = null;
   public EnhancedBlock? Chance { get; set; } = null;

   public static TradeGood Empty { get; } = new ("UNDEFINED");


   public override bool Equals(object? obj)
   {
      if (obj is TradeGood other)
         return Name == other.Name;
      return false;
   }

   public override int GetHashCode()
   {
      return Name.GetHashCode();
   }

   public static bool operator ==(TradeGood left, TradeGood right)
   {
      if (left is null)
         return right is null;
      return left.Equals(right);
   }

   public static bool operator !=(TradeGood left, TradeGood right)
   {
      if (left is null)
         return !(right is null);
      return !left.Equals(right);
   }

   public override string ToString()
   {
      return Name;
   }

   public static IErrorHandle GeneralParse(string? str, out object result)
   {
      var handle = TryParse(str, out var tradeGood);
      result = tradeGood;
      return handle;
   }

   public static IErrorHandle TryParse(string input, out TradeGood tradeGood)
   {
      if (string.IsNullOrEmpty(input))
      {
         tradeGood = Empty;
         return new ErrorObject(ErrorType.TypeConversionError, "Could not parse tradegood!", addToManager: false);
      }

      if (!Globals.TradeGoods.TryGetValue(input, out tradeGood))
      {
         tradeGood = Empty;
         return new ErrorObject(ErrorType.TODO_ERROR, $"Tradegood \"{input}\" is already defined!",
            addToManager: false);
      }

      return ErrorHandle.Success;
   }

   public override void OnPropertyChanged(string? propertyName = null) { }

   public override SaveableType WhatAmI() => SaveableType.TradeGood;

   public override string[] GetDefaultFolderPath() => ["common", "tradegoods"];

   public override string GetFileEnding() => ".txt";

   public override KeyValuePair<string, bool> GetFileName() => new("00_tradegoods", false);

   public override string SavingComment() => Localisation.GetLoc(Name);

   public override string GetSaveString(int tabs)
   {
      var sb = new StringBuilder();
      SavingUtil.OpenBlock(ref tabs, Name, ref sb);
      SavingUtil.AddColor(tabs, Color, ref sb);
      SavingUtil.AddBoolIfYes(tabs, IsLatent, "is_latent", ref sb);
      SavingUtil.AddBoolIfYes(tabs, IsValuable, "is_valuable", ref sb);
      if (RNWLatentChance > 0)
         SavingUtil.AddInt(tabs, RNWLatentChance, "rnw_latent_chance", ref sb);
      SavingUtil.AddFormattedStringList("modifier", Modifier.Select(x => $"{x.Key} = {x.Value}").ToList(), tabs + 1, ref sb);
      SavingUtil.AddFormattedStringList("province", ProvinceModifier.Select(x => $"{x.Key} = {x.Value}").ToList(), tabs + 1, ref sb);
      if (Trigger != null)
         sb.AppendLine(Trigger.GetFormattedString(tabs + 1, ref sb));
      if (Chance != null)
         sb.AppendLine(Chance.GetFormattedString(tabs + 1, ref sb));
      SavingUtil.CloseBlock(ref tabs, ref sb);
      return sb.ToString();
   }

   public override string GetSavePromptString()
   {
      return $"Save tradegood: {Name}";
   }
}

public static class TradeGoodHelper
{
   public static TradeGood StringToTradeGood(string str)
   {
      if (Globals.TradeGoods.TryGetValue(str, out var tradeGood))
         return tradeGood;
      if (str == string.Empty)
         return TradeGood.Empty;
      Globals.ErrorLog.Write($"Can not find tradegood {str}");
      return TradeGood.Empty;
   }
   public static void DumpTradeGoods(string folderPath)
   {
      var sb = new StringBuilder();
      foreach (var tradeGood in Globals.TradeGoods.Values)
      {
         sb.AppendLine(tradeGood.ToString());
      }

      File.WriteAllText(Path.Combine(folderPath, "tradegoods_dump.txt"), sb.ToString());
   }
}
