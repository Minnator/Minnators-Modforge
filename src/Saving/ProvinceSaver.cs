using System.Collections;
using System.Globalization;
using System.Text;
using Editor.DataClasses.GameDataClasses;
using static Editor.Helper.ProvinceEnumHelper;

namespace Editor.Saving
{
   public static class ProvinceSaver
   {
      //---------------------------------------- Accessible Methods ----------------------------------------

      

      //---------------------------------------- Helper Methods ----------------------------------------

      // A province will be saved by passing along a reference to a string builder and the province itself.
      // The string builder will be used to write the file.
      // The different portions of the province will be added to the string builder in the following order:
      // - definition Values
      // - anything else
      // - history Entries

      /// <summary>
      /// Generates the string for the province file.
      /// </summary>
      /// <param name="province"></param>
      /// <param name="contentString"></param>
      public static void GetProvinceFileString(Province province, out string contentString)
      {
         contentString = string.Empty;
         var sb = new StringBuilder();

         sb.Append(
            $"# {province.Id} {province.TitleLocalisation} - {DateTime.Now} {Environment.NewLine} {Environment.NewLine}");
         SaveDefinitionValues(ref sb, province);
         sb.AppendLine();
         SaveAnythingElse(ref sb, province);
         sb.AppendLine();
         SaveHistoryEntries(ref sb, province);
         sb.AppendLine();

         contentString = sb.ToString();
      }

      /// <summary>
      /// Adds all definition values of the p to the string builder.
      /// </summary>
      /// <param name="sb"></param>
      /// <param name="p"></param>
      private static void SaveDefinitionValues(ref StringBuilder sb, Province p)
      {
         AddCollection("add_core", p.GetPropertyValue("Cores"), ref sb);
         AddItem("owner", p.GetPropertyValue("Owner"), ref sb);
         AddItem("controller", p.GetPropertyValue("Controller"), ref sb);
         AddItem("base_tax", p.GetPropertyValue("BaseTax"), ref sb);
         AddItem("base_production", p.GetPropertyValue("BaseProduction"), ref sb);
         AddItem("base_manpower", p.GetPropertyValue("BaseManpower"), ref sb);
         sb.AppendLine();
         AddItem("center_of_trade", p.GetPropertyValue("CenterOfTrade"), ref sb);
         AddItem("trade_goods", p.GetPropertyValue("TradeGood"), ref sb);
         AddItem("extra_cost", p.GetPropertyValue("ExtraCost"), ref sb);
         sb.AppendLine();
         AddItem("culture", p.GetPropertyValue("Culture"), ref sb);
         AddItem("religion", p.GetPropertyValue("Religion"), ref sb);
         SavingUtil.AddQuotedString(0, p.GetPropertyValue("Capital").ToString(), "capital", ref sb);
         sb.AppendLine();
         AddItem("hre", p.GetPropertyValue("IsHre"), ref sb);
         AddItem("is_city", p.GetPropertyValue("IsCity"), ref sb);
         AddItem("citysize", p.GetPropertyValue("CitySize"), ref sb);
         AddItem("seat_in_parliament", p.GetPropertyValue("IsSeatInParliament"), ref sb);
         AddItem("add_local_autonomy", p.GetPropertyValue("LocalAutonomy"), ref sb);
         AddItem("add_devastation", p.GetPropertyValue("Devastation"), ref sb);
         AddItem("add_prosperity", p.GetPropertyValue("Prosperity"), ref sb);
         AddItem("add_nationalism", p.GetPropertyValue("Nationalism"), ref sb);
         AddItem("native_size", p.GetPropertyValue("NativeSize"), ref sb);
         AddItem("native_ferocity", p.GetPropertyValue("NativeFerocity"), ref sb);
         AddItem("native_hostileness", p.GetPropertyValue("NativeHostileness"), ref sb);
         AddItem("tribal_owner", p.GetPropertyValue("TribalOwner"), ref sb);
         foreach (string building in p.Buildings)
            AddItem(building, "yes", ref sb);
         AddCollection("add_claim", p.GetPropertyValue("Claims"), ref sb);
         AddCollection("add_permanent_claim", p.GetPropertyValue("PermanentClaims"), ref sb);
         sb.AppendLine();
         AddCollection("discovered_by", p.GetPropertyValue("DiscoveredBy"), ref sb);
         sb.AppendLine();
         AddCollection("add_province_triggered_modifier", (object)p.ProvinceTriggeredModifiers, ref sb);
         sb.AppendLine();
         AddEffects(p, ref sb);
      }

      /// <summary>
      /// Writes all effects to the string builder.
      /// </summary>
      private static void AddEffects(Province p, ref StringBuilder sb)
      {
         //SavingUtil.AddElements(0, p.Effects, ref sb);
      }

      /// <summary>
      /// Adds all values to the string builder which are no history entries or definitions
      /// </summary>
      /// <param name="sb"></param>
      /// <param name="province"></param>
      private static void SaveAnythingElse(ref StringBuilder sb, Province province)
      {
         // latent_trade_goods
         var latentTradeGoods = province.GetPropertyValue("LatentTradeGood") as string;
         if (!string.IsNullOrEmpty(latentTradeGoods))
         {
            sb.AppendLine("latent_trade_goods = {")
               .AppendLine($"   {latentTradeGoods}")
               .AppendLine("}");
         }

         // province modifiers
         foreach (var mod in province.PermanentProvinceModifiers)
         {
            sb.AppendLine("add_permanent_province_modifier = {")
               .AppendLine($"\tname = {mod.Name}\n\tduration = {mod.Duration}")
               .AppendLine("}");
         }

         // province modifiers
         foreach (var mod in province.ProvinceModifiers)
         {
            sb.AppendLine("add_province_modifier = {")
               .AppendLine($"\tname = {mod.Name}\n\tduration = {mod.Duration}")
               .AppendLine("}");
         }

         // trade modifiers
         foreach (var mod in province.TradeModifiers)
         {
            sb.AppendLine("add_trade_modifier = {")
               .AppendLine($"\twho = {mod.Who}\n\tpower = {mod.Power}\n\tkey = {mod.Key}\n\tduration = {mod.Duration}")
               .AppendLine("}");
         }

      }

      /// <summary>
      /// Adds all history entries of the province to the string builder.
      /// </summary>
      /// <param name="sb"></param>
      /// <param name="province"></param>
      private static void SaveHistoryEntries(ref StringBuilder sb, Province province)
      {
         foreach (var entry in province.History)
            SaveHistoryEntry(entry, ref sb);
      }

      private static void SaveHistoryEntry(ProvinceHistoryEntry entry, ref StringBuilder sb)
      {
         sb.AppendLine($"{entry.Date} = {{");
         foreach (var effect in entry.Effects)
            sb.AppendLine($"{effect.GetEffectString(1)}");
         sb.AppendLine("}");
      }

      private static void AddCollection(string name, object list, ref StringBuilder sb)
      {
         if (list is not IEnumerable collection)
            return;

         foreach (var item in collection)
            sb.AppendLine($"{name} = {item.ToString()}");
      }

      private static void AddItem(string name, object value, ref StringBuilder sb)
      {
         switch (value)
         {
            case bool b:
               if (!b)
                  break;
               sb.AppendLine($"{name} = {SavingUtil.GetYesNo(b)}");
               return;
            case float f:
               if (f == 0)
                  break;
               sb.AppendLine($"{name} = {f.ToString(CultureInfo.InvariantCulture),2}");
               break;
            case int i:
               if (i == 0)
                  break;
               sb.AppendLine($"{name} = {i.ToString()}");
               break;
            case Tag t:
               if (t.IsEmpty())
                  break;
               sb.AppendLine($"{name} = {t.ToString()}");
               break;
            case string s:
               if (string.IsNullOrEmpty(s))
                  break;
               sb.AppendLine($"{name} = {value.ToString()}");
               break;
         }
      }

   }
}