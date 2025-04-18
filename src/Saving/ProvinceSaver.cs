using System.Collections;
using System.Globalization;
using System.Text;
using Editor.DataClasses.GameDataClasses;
using Editor.DataClasses.Saveables;
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
         AddCollection("add_core", p.GetPropertyValue(nameof(Province.Cores)), ref sb);
         AddItem("owner", p.GetPropertyValue(nameof(Province.ScenarioOwner)), ref sb);
         AddItem("controller", p.GetPropertyValue(nameof(Province.ScenarioController)), ref sb);
         AddItem("base_tax", p.GetPropertyValue(nameof(Province.ScenarioBaseTax)), ref sb);
         AddItem("base_production", p.GetPropertyValue(nameof(Province.ScenarioBaseProduction)), ref sb);
         AddItem("base_manpower", p.GetPropertyValue(nameof(Province.ScenarioBaseManpower)), ref sb);
         sb.AppendLine();
         AddItem("center_of_trade", p.GetPropertyValue(nameof(Province.ScenarioCenterOfTrade)), ref sb);
         AddItem("trade_goods", p.GetPropertyValue(nameof(Province.ScenarioTradeGood)), ref sb);
         AddItem("extra_cost", p.GetPropertyValue(nameof(Province.ScenarioExtraCost)), ref sb);
         sb.AppendLine();
         AddItem("culture", p.GetPropertyValue(nameof(Province.ScenarioCulture)), ref sb);
         AddItem("religion", p.GetPropertyValue(nameof(Province.ScenarioReligion)), ref sb);
         SavingUtil.AddQuotedString(0, p.GetPropertyValue(nameof(Province.ScenarioCapital)).ToString(), "capital", ref sb);
         sb.AppendLine();
         AddItem("hre", p.GetPropertyValue(nameof(Province.ScenarioIsHre)), ref sb);
         AddItem("is_city", p.GetPropertyValue(nameof(Province.ScenarioIsCity)), ref sb);
         AddItem("citysize", p.GetPropertyValue(nameof(Province.ScenarioCitySize)), ref sb);
         AddItem("seat_in_parliament", p.GetPropertyValue(nameof(Province.ScenarioIsSeatInParliament)), ref sb);
         AddItem("add_local_autonomy", p.GetPropertyValue(nameof(Province.ScenarioLocalAutonomy)), ref sb);
         AddItem("add_devastation", p.GetPropertyValue(nameof(Province.ScenarioDevastation)), ref sb);
         AddItem("add_prosperity", p.GetPropertyValue(nameof(Province.ScenarioProsperity)), ref sb);
         AddItem("add_nationalism", p.GetPropertyValue(nameof(Province.ScenarioNationalism)), ref sb);
         AddItem("native_size", p.GetPropertyValue(nameof(Province.ScenarioNativeSize)), ref sb);
         AddItem("native_ferocity", p.GetPropertyValue(nameof(Province.ScenarioNativeFerocity)), ref sb);
         AddItem("native_hostileness", p.GetPropertyValue(nameof(Province.ScenarioNativeHostileness)), ref sb);
         AddItem("tribal_owner", p.GetPropertyValue(nameof(Province.ScenarioTribalOwner)), ref sb);
         foreach (var building in p.Buildings)
            AddItem(building.Name, "yes", ref sb);
         AddCollection("add_claim", p.GetPropertyValue(nameof(Province.Claims)), ref sb);
         AddCollection("add_permanent_claim", p.GetPropertyValue(nameof(Province.PermanentClaims)), ref sb);
         sb.AppendLine();
         AddCollection("discovered_by", p.GetPropertyValue(nameof(Province.DiscoveredBy)), ref sb);
         sb.AppendLine();
         AddCollection("add_province_triggered_modifier", p.ProvinceTriggeredModifiers, ref sb);
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
            effect.GetTokenString(1, ref sb);
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
            case Country c:
               if (c.Tag.IsEmpty())
                  break;
               sb.AppendLine($"{name} = {c.Tag}");
               break;
            case Religion r:
               sb.AppendLine($"{name} = {r.Name}");
               break;
            case Culture c:
               sb.AppendLine($"{name} = {c.Name}");
               break;
            case TradeGood tg:
               sb.AppendLine($"{name} = {tg.Name}");
               break;
         }
      }

   }
}