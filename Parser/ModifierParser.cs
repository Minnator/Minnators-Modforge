using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using Editor.DataClasses.GameDataClasses;
using Editor.Helper;

namespace Editor.Parser
{
   public static class ModifierParser
   {
      private const string PROVINCE_MODIFIER_REGEX = "name\\s*=\\s*(.*)\\s*duration\\s*=\\s*(.*)";
      private static readonly Regex ProvinceModifierRegex = new(PROVINCE_MODIFIER_REGEX, RegexOptions.Compiled);

      public static Dictionary<string, int> CountryModifierToIndex = new(558);
      public static Dictionary<string, int> ProvinceModifierToIndex = new(96);
      public static Dictionary<string, int> EstateModifierToIndex = new(76);
      public static Dictionary<string, int> GovernmentMechanicsToIndex = new(100);
      public static Dictionary<string, int> FactionModifiersToIndex = new(50);

      public static HashSet<string> EstateModifiers = [];
      public static HashSet<string> GovernmentMechanics = [];
      public static HashSet<string> FactionModifiers = [];

      public static void Initialize()
      {
         Globals.ModifierKeys = new string[CountryModifiersDict.Count + ProvinceModifiersDict.Count + EstateModifiers.Count + GovernmentMechanics.Count + FactionModifiers.Count];


         var cnt = 0;
         foreach (var cmod in CountryModifiersDict.Keys)
         {
            Globals.ModifierKeys[cnt] = cmod;
            CountryModifierToIndex[cmod] = cnt;
            Globals.ModifierValueTypes.Add(cnt, CountryModifiersDict[cmod]);
            cnt++;
         }

         foreach (var pmod in ProvinceModifiersDict.Keys)
         {
            Globals.ModifierKeys[cnt] = pmod;
            ProvinceModifierToIndex[pmod] = cnt;
            Globals.ModifierValueTypes.Add(cnt, ProvinceModifiersDict[pmod]);
            cnt++;
         }

         foreach (var emod in EstateModifiers)
         {
            Globals.ModifierKeys[cnt] = emod;
            EstateModifierToIndex[emod] = cnt;
            Globals.ModifierValueTypes.Add(cnt, ModifierValueType.Float);
            cnt++;
         }

         foreach (var gmod in GovernmentMechanics)
         {
            Globals.ModifierKeys[cnt] = gmod;
            GovernmentMechanicsToIndex[gmod] = cnt;
            Globals.ModifierValueTypes.Add(cnt, ModifierValueType.Float);
            cnt++;
         }

         foreach (var fmod in FactionModifiers)
         {
            Globals.ModifierKeys[cnt] = fmod;
            FactionModifiersToIndex[fmod] = cnt;
            Globals.ModifierValueTypes.Add(cnt, ModifierValueType.Float);
            cnt++;
         }

         var sb = new StringBuilder();
         for (var i = 0; i < Globals.ModifierKeys.Length; i++)
         {
            sb.AppendLine($"{i,4} - {Globals.ModifierValueTypes[i],5} - {Globals.ModifierKeys[i]}");
         }
         //downloads folder
         System.IO.File.WriteAllText("C:\\Users\\david\\Downloads\\modifiers.txt", sb.ToString());

      }

      public static void Demilitarize()
      {
         CountryModifierToIndex.Clear();
         ProvinceModifierToIndex.Clear();
         EstateModifierToIndex.Clear();

         RemovedModifiers.Clear();

         CountryModifiersDict.Clear();
         ProvinceModifiersDict.Clear();
      }

      /// <summary>
      /// Parses the application of a modifier to a target following mod_name = { name = x duration = y }
      /// </summary>
      /// <param name="str"></param>
      /// <param name="mod"></param>
      /// <returns></returns>
      public static bool ParseApplicableModifier(string str, out ApplicableModifier mod)
      {
         mod = default!;

         var match = ProvinceModifierRegex.Match(str);
         if (!match.Success)
         {
            Globals.ErrorLog.Write($"Could not parse province modifier from string: {str}");
         }

         var name = match.Groups[1].Value.Trim();
         var duration = int.Parse(match.Groups[2].Value);

         mod = new(name, duration);
         return true;
      }

      public static bool ParseRulerModifier(string str, out RulerModifier rulerMod)
      {
         var kvps = str.Split('=', '\n');
         if (kvps.Length % 2 != 0)
         {
            Globals.ErrorLog.Write($"Could not parse ruler modifier from string: {str}");
            rulerMod = default!;
            return false;
         }

         rulerMod = new();
         for (var i = 0; i < kvps.Length; i += 2)
         {
            if (kvps[i].Trim().Equals("name"))
            {
               rulerMod.Name = kvps[i + 1].Trim();
               break;
            }

            if (kvps[i].Trim().ToLower().Equals("hidden"))
            {
               rulerMod.IsHidden = Parsing.YesNo(kvps[i + 1].Trim());
               break;
            }
         }

         return true;
      }

      public static bool ParseTradeModifier(string str, out TradeModifier tradeModifier)
      {
         tradeModifier = default!;

         var kvps = str.Split('=', '\n');
         if (kvps.Length % 2 != 0)
         {
            Globals.ErrorLog.Write($"Could not parse trade power modifier from string: {str}");
            return false;
         }

         tradeModifier = new();

         for (var i = 0; i < kvps.Length; i += 2)
         {
            switch (kvps[i].Trim())
            {
               case "who":
                  tradeModifier.Who = kvps[i + 1].Trim();
                  break;
               case "duration":
                  if (!int.TryParse(kvps[i + 1].Trim(), out var duration))
                  {
                     Globals.ErrorLog.Write($"Could not parse duration from string: {kvps[i + 1]} in trade_power_modifier");
                     return false;
                  }
                  tradeModifier.Duration = duration;
                  break;
               case "power":
                  if (!int.TryParse(kvps[i + 1].Trim(), out var power))
                  {
                     Globals.ErrorLog.Write($"Could not parse power from string: {kvps[i + 1]} in trade_power_modifier");
                     return false;
                  }
                  tradeModifier.Power = power;
                  break;
               case "key":
                  tradeModifier.Key = kvps[i + 1].Trim();
                  break;
            }
         }

         return true;
      }

      public static bool ParseModifiers(string input, out List<Modifier> modifiers)
      {
         var kvps = Parsing.GetKeyValueList(ref input);
         modifiers = [];
         var flawness = true;

         foreach (var kvp in kvps)
         {
            if (!ParseModifierFromName(kvp.Key, kvp.Value, out var modifier))
            {
               Globals.ErrorLog.Write($"Could not parse modifier from string: {kvp.Key} = {kvp.Value}");
               flawness = false;
               continue;
            }
            modifiers.Add(modifier);
         }
         return flawness;
      }

      public static bool ParseModifierFromName(string name, string value, out Modifier modifier)
      {
         if (CountryModifiersDict.ContainsKey(name))
         {
            modifier = new(CountryModifierToIndex[name], value);
            return true;
         }
         if (ProvinceModifiersDict.ContainsKey(name))
         {
            modifier = new(ProvinceModifierToIndex[name], value, Scope.Province);
            return true;
         }
         if (EstateModifiers.Contains(name))
         {
            modifier = new(EstateModifierToIndex[name], value, Scope.Country);
            return true;
         }
         if (GovernmentMechanics.Contains(name))
         {
            modifier = new(GovernmentMechanicsToIndex[name], value, Scope.Country);
            return true;
         }
         if (FactionModifiers.Contains(name))
         {
            modifier = new(FactionModifiersToIndex[name], value, Scope.Country);
            return true;
         }

         modifier = default!;
         return false;
      }

      public static bool IsCustomModifierTrigger(string value)
      {
         return CustomModifierTrigger.ContainsKey(value);
      }

      public static bool IsValidModifier(string name)
      {
         return CountryModifiersDict.ContainsKey(name) || ProvinceModifiersDict.ContainsKey(name);
      }

      public static bool IsRemovedModifier(string name)
      {
         return RemovedModifiers.Contains(name);
      }

      public static readonly Dictionary<string, ModifierValueType> CountryModifiersDict = new(){
          { "army_tradition", ModifierValueType.Float },
          { "army_tradition_decay", ModifierValueType.Float },
          { "army_tradition_from_battle", ModifierValueType.Float },
          { "yearly_army_professionalism", ModifierValueType.Percentile },
          { "drill_gain_modifier", ModifierValueType.Float },
          { "drill_decay_modifier", ModifierValueType.Float },
          { "infantry_cost", ModifierValueType.Float },
          { "infantry_power", ModifierValueType.Float },
          { "infantry_fire", ModifierValueType.Float },
          { "infantry_shock", ModifierValueType.Float },
          { "cavalry_cost", ModifierValueType.Float },
          { "cavalry_power", ModifierValueType.Float },
          { "cavalry_fire", ModifierValueType.Float },
          { "cavalry_shock", ModifierValueType.Float },
          { "artillery_cost", ModifierValueType.Float },
          { "artillery_power", ModifierValueType.Float },
          { "artillery_fire", ModifierValueType.Float },
          { "artillery_shock", ModifierValueType.Float },
          { "cav_to_inf_ratio", ModifierValueType.Float },
          { "cavalry_flanking", ModifierValueType.Float },
          { "artillery_levels_available_vs_fort", ModifierValueType.Int },
          { "artillery_level_modifier", ModifierValueType.Float },
          { "backrow_artillery_damage", ModifierValueType.Float },
          { "discipline", ModifierValueType.Float },
          { "land_morale", ModifierValueType.Float },
          { "land_morale_constant", ModifierValueType.Float },
          { "movement_speed", ModifierValueType.Float },
          { "fire_damage", ModifierValueType.Float },
          { "fire_damage_received", ModifierValueType.Float },
          { "shock_damage", ModifierValueType.Float },
          { "shock_damage_received", ModifierValueType.Float },
          { "morale_damage", ModifierValueType.Float },
          { "morale_damage_received", ModifierValueType.Float },
          { "recover_army_morale_speed", ModifierValueType.Float },
          { "reserves_organisation", ModifierValueType.Float },
          { "land_attrition", ModifierValueType.Float },
          { "reinforce_cost_modifier", ModifierValueType.Float },
          { "no_cost_for_reinforcing", ModifierValueType.Bool },
          { "reinforce_speed", ModifierValueType.Float },
          { "manpower_recovery_speed", ModifierValueType.Float },
          { "global_manpower", ModifierValueType.Float },
          { "global_manpower_modifier", ModifierValueType.Float },
          { "global_regiment_cost", ModifierValueType.Float },
          { "global_regiment_recruit_speed", ModifierValueType.Float },
          { "global_supply_limit_modifier", ModifierValueType.Float },
          { "land_forcelimit", ModifierValueType.Float },
          { "land_forcelimit_modifier", ModifierValueType.Float },
          { "land_maintenance_modifier", ModifierValueType.Float },
          { "possible_condottieri", ModifierValueType.Float },
          { "hostile_attrition", ModifierValueType.Float },
          { "max_hostile_attrition", ModifierValueType.Float },
          { "siege_ability", ModifierValueType.Float },
          { "artillery_barrage_cost", ModifierValueType.Float },
          { "assault_fort_cost_modifier", ModifierValueType.Float },
          { "assault_fort_ability", ModifierValueType.Float },
          { "defensiveness", ModifierValueType.Float },
          { "garrison_size", ModifierValueType.Float },
          { "global_garrison_growth", ModifierValueType.Float },
          { "garrison_damage", ModifierValueType.Float },
          { "fort_maintenance_modifier", ModifierValueType.Float },
          { "rival_border_fort_maintenance", ModifierValueType.Float },
          { "war_exhaustion", ModifierValueType.Float },
          { "war_exhaustion_cost", ModifierValueType.Float },
          { "leader_land_fire", ModifierValueType.Int },
          { "leader_land_manuever", ModifierValueType.Int },
          { "leader_land_shock", ModifierValueType.Int },
          { "leader_siege", ModifierValueType.Int },
          { "max_general_fire", ModifierValueType.Int },
          { "max_general_maneuver", ModifierValueType.Int },
          { "max_general_shock", ModifierValueType.Int },
          { "max_general_siege", ModifierValueType.Int },
          { "general_cost", ModifierValueType.Float },
          { "free_leader_pool", ModifierValueType.Int },
          { "free_land_leader_pool", ModifierValueType.Int },
          { "free_navy_leader_pool", ModifierValueType.Int },
          { "raze_power_gain", ModifierValueType.Float },
          { "loot_amount", ModifierValueType.Float },
          { "available_province_loot", ModifierValueType.Float },
          { "prestige_from_land", ModifierValueType.Float },
          { "war_taxes_cost_modifier", ModifierValueType.Float },
          { "leader_cost", ModifierValueType.Float },
          { "may_recruit_female_generals", ModifierValueType.Bool },
          { "manpower_in_true_faith_provinces", ModifierValueType.Float },
          { "regiment_manpower_usage", ModifierValueType.Float },
          { "military_tactics", ModifierValueType.Float },
          { "capped_by_forcelimit", ModifierValueType.Bool },
          { "global_attacker_dice_roll_bonus", ModifierValueType.Float },
          { "global_defender_dice_roll_bonus", ModifierValueType.Float },
          { "own_territory_dice_roll_bonus", ModifierValueType.Float },
          { "manpower_in_accepted_culture_provinces", ModifierValueType.Float },
          { "manpower_in_culture_group_provinces", ModifierValueType.Float },
          { "manpower_in_own_culture_provinces", ModifierValueType.Float },
          { "may_build_supply_depot", ModifierValueType.Bool },
          { "may_refill_garrison", ModifierValueType.Bool },
          { "may_return_manpower_on_disband", ModifierValueType.Bool },
          { "attack_bonus_in_capital_terrain", ModifierValueType.Float },
          { "can_bypass_forts", ModifierValueType.Bool },
          { "force_march_free", ModifierValueType.Bool },
          { "enable_forced_march", ModifierValueType.Bool },
          { "warscore_from_battles_modifier", ModifierValueType.Float },
          { "mercenary_cost", ModifierValueType.Float },
          { "merc_maintenance_modifier", ModifierValueType.Float },
          { "mercenary_discipline", ModifierValueType.Float },
          { "mercenary_manpower", ModifierValueType.Float },
          { "merc_leader_army_tradition", ModifierValueType.Float },
          { "allow_mercenary_drill", ModifierValueType.Bool },
          { "merc_independent_from_trade_range", ModifierValueType.Bool },
          { "allow_mercenaries_to_split", ModifierValueType.Bool },
          { "special_unit_forcelimit", ModifierValueType.Float },
          { "special_unit_cost_modifier", ModifierValueType.Float },
          { "special_unit_manpower_cost_modifier", ModifierValueType.Float },
          { "has_marines", ModifierValueType.Bool },
          { "allowed_marine_fraction", ModifierValueType.Float },
          { "has_banners", ModifierValueType.Bool },
          { "amount_of_banners", ModifierValueType.Float },
          { "can_recruit_cawa", ModifierValueType.Bool },
          { "amount_of_cawa", ModifierValueType.Float },
          { "cawa_cost_modifier", ModifierValueType.Float },
          { "has_carolean", ModifierValueType.Bool },
          { "amount_of_carolean", ModifierValueType.Float },
          { "can_recruit_hussars", ModifierValueType.Bool },
          { "amount_of_hussars", ModifierValueType.Float },
          { "hussars_cost_modifier", ModifierValueType.Float },
          { "can_recruit_janissaries", ModifierValueType.Bool },
          { "janissary_cost_modifier", ModifierValueType.Float },
          { "allow_janissaries_from_own_faith", ModifierValueType.Bool },
          { "can_recruit_cossacks", ModifierValueType.Bool },
          { "allowed_cossack_fraction", ModifierValueType.Float },
          { "can_recruit_rajputs", ModifierValueType.Bool },
          { "allowed_rajput_fraction", ModifierValueType.Float },
          { "can_recruit_revolutionary_guards", ModifierValueType.Bool },
          { "allowed_rev_guard_fraction", ModifierValueType.Float },
          { "has_streltsy", ModifierValueType.Bool },
          { "allowed_streltsy_fraction", ModifierValueType.Float },
          { "amount_of_streltsy", ModifierValueType.Float },
          { "has_tercio", ModifierValueType.Bool },
          { "allowed_tercio_fraction", ModifierValueType.Float },
          { "amount_of_tercio", ModifierValueType.Float },
          { "has_samurai", ModifierValueType.Bool },
          { "allowed_samurai_fraction", ModifierValueType.Float },
          { "amount_of_samurai", ModifierValueType.Float },
          {"has_musketeer", ModifierValueType.Bool},
          {"allowed_musketeer_fraction", ModifierValueType.Float},
          {"amount_of_musketeers", ModifierValueType.Int},
          {"has_mamluks", ModifierValueType.Bool},
          {"allowed_mamluks_fraction", ModifierValueType.Float},
          {"amount_of_mamluks", ModifierValueType.Int},
          {"has_qizilbash", ModifierValueType.Bool},
          {"allowed_qizilbash_fraction", ModifierValueType.Float},
          {"amount_of_qizilbash", ModifierValueType.Int},
          {"navy_tradition", ModifierValueType.Float},
          {"navy_tradition_decay", ModifierValueType.Float},
          {"naval_tradition_from_battle", ModifierValueType.Float},
          {"naval_tradition_from_trade", ModifierValueType.Float},
          {"heavy_ship_cost", ModifierValueType.Float},
          {"heavy_ship_power", ModifierValueType.Float},
          {"light_ship_cost", ModifierValueType.Float},
          {"light_ship_power", ModifierValueType.Float},
          {"galley_cost", ModifierValueType.Float},
          {"galley_power", ModifierValueType.Float},
          {"transport_cost", ModifierValueType.Float},
          {"transport_power", ModifierValueType.Float},
          {"global_ship_cost", ModifierValueType.Float},
          {"global_ship_recruit_speed", ModifierValueType.Float},
          {"global_ship_repair", ModifierValueType.Float},
          {"naval_forcelimit", ModifierValueType.Int},
          {"naval_forcelimit_modifier", ModifierValueType.Float},
          {"naval_maintenance_modifier", ModifierValueType.Float},
          {"global_sailors", ModifierValueType.Int},
          {"global_sailors_modifier", ModifierValueType.Float},
          {"sailor_maintenance_modifer", ModifierValueType.Float},
          {"sailors_recovery_speed", ModifierValueType.Float},
          {"blockade_efficiency", ModifierValueType.Float},
          {"siege_blockade_progress", ModifierValueType.Float},
          {"capture_ship_chance", ModifierValueType.Float},
          {"global_naval_engagement_modifier", ModifierValueType.Float},
          {"global_naval_engagement", ModifierValueType.Float},
          {"naval_attrition", ModifierValueType.Float},
          {"naval_morale", ModifierValueType.Float},
          {"naval_morale_constant", ModifierValueType.Float},
          {"naval_morale_damage", ModifierValueType.Float},
          {"naval_morale_damage_received", ModifierValueType.Float},
          {"ship_durability", ModifierValueType.Float},
          {"sunk_ship_morale_hit_recieved", ModifierValueType.Float},
          {"recover_navy_morale_speed", ModifierValueType.Float},
          {"prestige_from_naval", ModifierValueType.Float},
          {"leader_naval_fire", ModifierValueType.Int},
          {"leader_naval_manuever", ModifierValueType.Int},
          {"leader_naval_shock", ModifierValueType.Int},
          {"max_admiral_fire", ModifierValueType.Int},
          {"max_admiral_maneuver", ModifierValueType.Int},
          {"max_admiral_shock", ModifierValueType.Int},
          {"max_admiral_siege", ModifierValueType.Int},
          {"own_coast_naval_combat_bonus", ModifierValueType.Float},
          {"admiral_cost", ModifierValueType.Float},
          {"global_naval_barrage_cost", ModifierValueType.Float},
          {"flagship_cost", ModifierValueType.Float},
          {"disengagement_chance", ModifierValueType.Float},
          {"transport_attrition", ModifierValueType.Float},
          {"landing_penalty", ModifierValueType.Float},
          {"regiment_disembark_speed", ModifierValueType.Float},
          {"may_perform_slave_raid", ModifierValueType.Bool},
          {"may_perform_slave_raid_on_same_religion", ModifierValueType.Bool},
          {"coast_raid_range", ModifierValueType.Float},
          {"sea_repair", ModifierValueType.Bool},
          {"movement_speed_in_fleet_modifier", ModifierValueType.Float},
          {"max_flagships", ModifierValueType.Int},
          {"number_of_cannons", ModifierValueType.Int},
          {"number_of_cannons_modifier", ModifierValueType.Float},
          {"heavy_ship_number_of_cannons_modifier", ModifierValueType.Float},
          {"light_ship_number_of_cannons_modifier", ModifierValueType.Float},
          {"galley_number_of_cannons_modifier", ModifierValueType.Float},
          {"transport_number_of_cannons_modifier", ModifierValueType.Float},
          {"hull_size", ModifierValueType.Int},
          {"hull_size_modifier", ModifierValueType.Float},
          {"heavy_ship_hull_size_modifier", ModifierValueType.Float},
          {"light_ship_hull_size_modifier", ModifierValueType.Float},
          {"galley_hull_size_modifier", ModifierValueType.Float},
          {"transport_hull_size_modifier", ModifierValueType.Float},
          {"engagement_cost", ModifierValueType.Float},
          {"engagement_cost_modifier", ModifierValueType.Float},
          {"ship_trade_power", ModifierValueType.Float},
          {"ship_trade_power_modifier", ModifierValueType.Float},
          {"can_transport_units", ModifierValueType.Bool},
          {"admiral_skill_gain_modifier", ModifierValueType.Float},
          {"flagship_durability", ModifierValueType.Float},
          {"flagship_morale", ModifierValueType.Float},
          {"flagship_naval_engagement_modifier", ModifierValueType.Float},
          {"movement_speed_onto_off_boat_modifier", ModifierValueType.Float},
          {"flagship_landing_penalty", ModifierValueType.Float},
          {"number_of_cannons_flagship_modifier", ModifierValueType.Float},
          {"number_of_cannons_flagship", ModifierValueType.Int},
          {"naval_maintenance_flagship_modifier", ModifierValueType.Float},
          {"trade_power_in_fleet_modifier", ModifierValueType.Float},
          {"morale_in_fleet_modifier", ModifierValueType.Float},
          {"blockade_impact_on_siege_in_fleet_modifier", ModifierValueType.Float},
          {"exploration_mission_range_in_fleet_modifier", ModifierValueType.Float},
          {"barrage_cost_in_fleet_modifier", ModifierValueType.Float},
          {"naval_attrition_in_fleet_modifier", ModifierValueType.Float},
          {"privateering_efficiency_in_fleet_modifier", ModifierValueType.Float},
          {"prestige_from_battles_in_fleet_modifier", ModifierValueType.Float},
          {"naval_tradition_in_fleet_modifier", ModifierValueType.Float},
          {"cannons_for_hunting_pirates_in_fleet", ModifierValueType.Int},
          {"has_geobukseon", ModifierValueType.Bool},
          {"allowed_geobukseon_fraction", ModifierValueType.Float},
          {"amount_of_geobukseon", ModifierValueType.Int},
          {"has_galleass", ModifierValueType.Bool},
          {"allowed_galleass_fraction", ModifierValueType.Float},
          {"amount_of_galleass", ModifierValueType.Int},
          {"has_voc_indiamen", ModifierValueType.Bool},
          {"allowed_voc_indiamen_fraction", ModifierValueType.Float},
          {"amount_of_voc_indiamen", ModifierValueType.Int},
          {"has_man_of_war", ModifierValueType.Bool},
          {"allowed_man_of_war_fraction", ModifierValueType.Float},
          {"amount_of_man_of_war", ModifierValueType.Int},
          {"has_galleon", ModifierValueType.Bool},
          {"allowed_galleon_fraction", ModifierValueType.Float},
          {"amount_of_galleon", ModifierValueType.Int},
          {"has_caravel", ModifierValueType.Bool},
          {"allowed_caravel_fraction", ModifierValueType.Float},
          {"amount_of_caravel", ModifierValueType.Int},
          { "diplomats", ModifierValueType.Int },
         { "diplomatic_reputation", ModifierValueType.Float },
         { "diplomatic_upkeep", ModifierValueType.Int },
         { "envoy_travel_time", ModifierValueType.Float },
         { "improve_relation_modifier", ModifierValueType.Float },
         { "ae_impact", ModifierValueType.Float },
         { "diplomatic_annexation_cost", ModifierValueType.Float },
         { "province_warscore_cost", ModifierValueType.Float },
         { "unjustified_demands", ModifierValueType.Float },
         { "rival_change_cost", ModifierValueType.Float },
         { "justify_trade_conflict_cost", ModifierValueType.Float },
         { "stability_cost_to_declare_war", ModifierValueType.Float },
         { "accept_vassalization_reasons", ModifierValueType.Int },
         { "transfer_trade_power_reasons", ModifierValueType.Int },
         { "monthly_federation_favor_growth", ModifierValueType.Float },
         { "monthly_favor_modifier", ModifierValueType.Float },
         { "cb_on_overseas", ModifierValueType.Bool },
         { "cb_on_primitives", ModifierValueType.Bool },
         { "idea_claim_colonies", ModifierValueType.Bool },
         { "cb_on_government_enemies", ModifierValueType.Bool },
         { "cb_on_religious_enemies", ModifierValueType.Bool },
         { "reduced_stab_impacts", ModifierValueType.Bool },
         { "can_fabricate_for_vassals", ModifierValueType.Bool },
         { "global_tax_income", ModifierValueType.Float },
         { "global_tax_modifier", ModifierValueType.Float },
         { "production_efficiency", ModifierValueType.Float },
         { "state_maintenance_modifier", ModifierValueType.Float },
         { "inflation_action_cost", ModifierValueType.Float },
         { "may_not_reduce_inflation", ModifierValueType.Bool },
         { "inflation_reduction", ModifierValueType.Float },
         { "monthly_gold_inflation_modifier", ModifierValueType.Float },
         { "gold_depletion_chance_modifier", ModifierValueType.Float },
         { "interest", ModifierValueType.Float },
         { "can_not_build_buildings", ModifierValueType.Bool },
         { "development_cost", ModifierValueType.Float },
         { "development_cost_in_primary_culture", ModifierValueType.Float },
         { "development_cost_modifier", ModifierValueType.Float },
         { "tribal_development_growth", ModifierValueType.Float },
         { "add_tribal_land_cost", ModifierValueType.Float },
         { "settle_cost", ModifierValueType.Float },
         { "global_allowed_num_of_buildings", ModifierValueType.Int },
         { "global_allowed_num_of_manufactories", ModifierValueType.Int },
         { "build_cost", ModifierValueType.Float },
         { "build_time", ModifierValueType.Float },
         { "great_project_upgrade_cost", ModifierValueType.Float },
         { "great_project_upgrade_time", ModifierValueType.Float },
         { "global_monthly_devastation", ModifierValueType.Float },
         { "global_prosperity_growth", ModifierValueType.Float },
         { "administrative_efficiency", ModifierValueType.Float },
         { "free_concentrate_development", ModifierValueType.Bool },
         { "expand_infrastructure_cost_modifier", ModifierValueType.Float },
         { "centralize_state_cost", ModifierValueType.Float },
         { "core_creation", ModifierValueType.Float },
         { "core_decay_on_your_own", ModifierValueType.Float },
         { "enemy_core_creation", ModifierValueType.Float },
         { "ignore_coring_distance", ModifierValueType.Bool },
         { "technology_cost", ModifierValueType.Float },
         { "idea_cost", ModifierValueType.Float },
         { "embracement_cost", ModifierValueType.Float },
         { "global_institution_spread", ModifierValueType.Float },
         { "institution_spread_from_true_faith", ModifierValueType.Float },
         { "native_advancement_cost", ModifierValueType.Float },
         { "all_power_cost", ModifierValueType.Float },
         { "innovativeness_gain", ModifierValueType.Float },
         { "yearly_innovativeness", ModifierValueType.Float },
         { "free_adm_policy", ModifierValueType.Int },
         { "free_dip_policy", ModifierValueType.Int },
         { "free_mil_policy", ModifierValueType.Int },
         { "possible_adm_policy", ModifierValueType.Int },
         { "possible_dip_policy", ModifierValueType.Int },
         { "possible_mil_policy", ModifierValueType.Int },
         { "possible_policy", ModifierValueType.Int },
         { "free_policy", ModifierValueType.Int },
         { "country_admin_power", ModifierValueType.Int },
         { "country_diplomatic_power", ModifierValueType.Int },
         { "country_military_power", ModifierValueType.Int},
         { "prestige", ModifierValueType.Float },
         { "prestige_decay", ModifierValueType.Float },
         { "monthly_splendor", ModifierValueType.Float },
         { "yearly_corruption", ModifierValueType.Float },
         { "advisor_cost", ModifierValueType.Float },
         { "same_culture_advisor_cost", ModifierValueType.Float },
         { "same_religion_advisor_cost", ModifierValueType.Float },
         { "advisor_pool", ModifierValueType.Int },
         { "female_advisor_chance", ModifierValueType.Float },
         { "heir_chance", ModifierValueType.Float },
         { "monthly_heir_claim_increase", ModifierValueType.Float },
         { "monthly_heir_claim_increase_modifier", ModifierValueType.Float },
         { "block_introduce_heir", ModifierValueType.Bool },
         { "monarch_admin_power", ModifierValueType.Float },
         { "monarch_diplomatic_power", ModifierValueType.Float },
         { "monarch_military_power", ModifierValueType.Float },
         { "adm_advisor_cost", ModifierValueType.Float },
         { "dip_advisor_cost", ModifierValueType.Float },
         { "mil_advisor_cost", ModifierValueType.Float },
         { "monthly_support_heir_gain", ModifierValueType.Float },
         { "power_projection_from_insults", ModifierValueType.Float },
         { "monarch_lifespan", ModifierValueType.Float },
         { "local_heir_adm", ModifierValueType.Float },
         { "local_heir_dip", ModifierValueType.Float },
         { "local_heir_mil", ModifierValueType.Float },
         { "national_focus_years", ModifierValueType.Float },
         { "yearly_absolutism", ModifierValueType.Float },
         { "max_absolutism", ModifierValueType.Float },
         { "max_absolutism_effect", ModifierValueType.Float },
         { "legitimacy", ModifierValueType.Float },
         { "republican_tradition", ModifierValueType.Float },
         { "devotion", ModifierValueType.Float },
         { "horde_unity", ModifierValueType.Float },
         { "meritocracy", ModifierValueType.Float },
         { "monthly_militarized_society", ModifierValueType.Float },
         { "monthly_<government_power_type_id>", ModifierValueType.Float },
         { "yearly_government_power", ModifierValueType.Float },
         { "<faction>_influence", ModifierValueType.Float },
         { "imperial_mandate", ModifierValueType.Float },
         { "election_cycle", ModifierValueType.Float },
         { "candidate_random_bonus", ModifierValueType.Float },
         { "reelection_cost", ModifierValueType.Float },
         { "max_terms", ModifierValueType.Int },
         { "governing_capacity", ModifierValueType.Int },
         { "governing_capacity_modifier", ModifierValueType.Float },
         { "governing_cost", ModifierValueType.Float },
         { "state_governing_cost", ModifierValueType.Float },
         { "territories_governing_cost", ModifierValueType.Float },
         { "trade_company_governing_cost", ModifierValueType.Float },
         { "state_governing_cost_increase", ModifierValueType.Float },
         { "expand_administration_cost", ModifierValueType.Float },
         { "yearly_revolutionary_zeal", ModifierValueType.Float },
         { "max_revolutionary_zeal", ModifierValueType.Float },
         { "reform_progress_growth", ModifierValueType.Float },
         { "monthly_reform_progress", ModifierValueType.Float },
         { "monthly_reform_progress_modifier", ModifierValueType.Float },
         { "move_capital_cost_modifier", ModifierValueType.Float },
         { "can_revoke_parliament_seats", ModifierValueType.Bool },
         { "parliament_backing_chance", ModifierValueType.Float },
         { "parliament_effect_duration", ModifierValueType.Float },
         { "parliament_debate_duration", ModifierValueType.Float },
         { "parliament_chance_of_decision", ModifierValueType.Float },
         { "num_of_parliament_issues", ModifierValueType.Int },
         { "max_possible_parliament_seats", ModifierValueType.Int },
         { "church_influence_modifier", ModifierValueType.Float },
         { "church_loyalty_modifier", ModifierValueType.Float },
         { "nobles_privilege_slots", ModifierValueType.Int },
         { "all_estate_influence_modifier", ModifierValueType.Float },
         { "all_estate_loyalty_equilibrium", ModifierValueType.Float },
         { "all_estate_possible_privileges", ModifierValueType.Int },
         { "allow_free_estate_privilege_revocation", ModifierValueType.Bool },
         { "loyalty_change_on_revoked", ModifierValueType.Float },
         { "estate_interaction_cooldown_modifier", ModifierValueType.Float },
         { "imperial_authority", ModifierValueType.Float },
         { "imperial_authority_value", ModifierValueType.Float },
         { "free_city_imperial_authority", ModifierValueType.Float },
         { "reasons_to_elect", ModifierValueType.Int },
         { "imperial_mercenary_cost", ModifierValueType.Float },
         { "max_free_cities", ModifierValueType.Int },
         { "max_electors", ModifierValueType.Int },
         { "legitimate_subject_elector", ModifierValueType.Bool },
         { "manpower_against_imperial_enemies", ModifierValueType.Float },
         { "imperial_reform_catholic_approval", ModifierValueType.Float },
         { "culture_conversion_cost", ModifierValueType.Float },
         { "culture_conversion_time", ModifierValueType.Float },
         { "num_accepted_cultures", ModifierValueType.Int },
         { "promote_culture_cost", ModifierValueType.Float },
         { "relation_with_same_culture", ModifierValueType.Float },
         { "relation_with_same_culture_group", ModifierValueType.Float },
         { "relation_with_accepted_culture", ModifierValueType.Float },
         { "relation_with_other_culture", ModifierValueType.Float },
         { "can_not_declare_war", ModifierValueType.Bool },
         { "global_unrest", ModifierValueType.Float },
         { "stability_cost_modifier", ModifierValueType.Float },
         { "global_autonomy", ModifierValueType.Float },
         { "min_autonomy", ModifierValueType.Float },
         { "autonomy_change_time", ModifierValueType.Float },
         { "harsh_treatment_cost", ModifierValueType.Float },
         { "global_rebel_suppression_efficiency", ModifierValueType.Float },
         { "years_of_nationalism", ModifierValueType.Float },
         { "min_autonomy_in_territories", ModifierValueType.Float },
         { "unrest_catholic_provinces", ModifierValueType.Float },
         { "no_stability_loss_on_monarch_death", ModifierValueType.Bool },
         { "overextension_impact_modifier", ModifierValueType.Float },
         { "vassal_forcelimit_bonus", ModifierValueType.Float },
         { "vassal_naval_forcelimit_bonus", ModifierValueType.Float },
         { "vassal_manpower_bonus", ModifierValueType.Float },
         { "vassal_sailors_bonus", ModifierValueType.Float },
         { "vassal_income", ModifierValueType.Float },
         { "overlord_naval_forcelimit", ModifierValueType.Float },
         { "overlord_naval_forcelimit_modifier", ModifierValueType.Float },
         { "can_transfer_vassal_wargoal", ModifierValueType.Bool },
         { "liberty_desire", ModifierValueType.Float },
         { "liberty_desire_from_subject_development", ModifierValueType.Float },
         { "reduced_liberty_desire", ModifierValueType.Float },
         { "reduced_liberty_desire_on_same_continent", ModifierValueType.Float },
         { "reduced_liberty_desire_on_other_continent", ModifierValueType.Float },
         { "allow_client_states", ModifierValueType.Bool },
         { "colonial_type_change_cost_modifier", ModifierValueType.Float },
         { "colonial_subject_type_upgrade_cost_modifier", ModifierValueType.Float },
         { "years_to_integrate_personal_union", ModifierValueType.Float },
         { "chance_to_inherit", ModifierValueType.Float },
         { "monarch_power_tribute", ModifierValueType.Float },
         { "tributary_conversion_cost_modifier", ModifierValueType.Float },
         { "annexation_relations_impact", ModifierValueType.Float },
         { "spy_offence", ModifierValueType.Float },
         { "global_spy_defence", ModifierValueType.Float },
         { "fabricate_claims_cost", ModifierValueType.Float },
         { "claim_duration", ModifierValueType.Float },
         { "can_chain_claim", ModifierValueType.Bool },
         { "discovered_relations_impact", ModifierValueType.Float },
         { "rebel_support_efficiency", ModifierValueType.Float },
         { "can_claim_states", ModifierValueType.Bool },
         { "no_claim_cost_increasement", ModifierValueType.Bool },
         { "spy_action_cost_modifier", ModifierValueType.Float },
         { "global_missionary_strength", ModifierValueType.Percentile },
         { "global_heretic_missionary_strength", ModifierValueType.Percentile },
         { "global_heathen_missionary_strength", ModifierValueType.Percentile },
         { "can_not_build_missionaries", ModifierValueType.Bool },
         { "may_not_convert_territories", ModifierValueType.Bool },
         { "missionaries", ModifierValueType.Int },
         { "missionary_maintenance_cost", ModifierValueType.Float },
         { "religious_unity", ModifierValueType.Float },
         { "tolerance_own", ModifierValueType.Float },
         { "tolerance_heretic", ModifierValueType.Float },
         { "tolerance_heathen", ModifierValueType.Float },
         { "tolerance_of_heretics_capacity", ModifierValueType.Float },
         { "tolerance_of_heathens_capacity", ModifierValueType.Float },
         { "papal_influence", ModifierValueType.Float },
         { "papal_influence_from_cardinals", ModifierValueType.Float },
         { "appoint_cardinal_cost", ModifierValueType.Float },
         { "curia_treasury_contribution", ModifierValueType.Float },
         { "curia_powers_cost", ModifierValueType.Float },
         { "monthly_church_power", ModifierValueType.Float },
         { "church_power_modifier", ModifierValueType.Float },
         { "monthly_fervor_increase", ModifierValueType.Float },
         { "yearly_patriarch_authority", ModifierValueType.Float },
         { "monthly_piety", ModifierValueType.Float },
         { "monthly_piety_accelerator", ModifierValueType.Float },
         { "harmonization_speed", ModifierValueType.Float },
         { "yearly_harmony", ModifierValueType.Float },
         { "monthly_karma", ModifierValueType.Float },
         { "monthly_karma_accelerator", ModifierValueType.Float },
         { "yearly_karma_decay", ModifierValueType.Float },
         { "yearly_doom_reduction", ModifierValueType.Float },
         { "yearly_authority", ModifierValueType.Float },
         { "enforce_religion_cost", ModifierValueType.Float },
         { "prestige_per_development_from_conversion", ModifierValueType.Float },
         { "warscore_cost_vs_other_religion", ModifierValueType.Float },
         { "establish_order_cost", ModifierValueType.Float },
         { "global_religious_conversion_resistance", ModifierValueType.Float },
         { "relation_with_same_religion", ModifierValueType.Float },
         { "relation_with_heretics", ModifierValueType.Float },
         { "relation_with_heathens", ModifierValueType.Float },
         { "reverse_relation_with_same_religion", ModifierValueType.Float },
         { "reverse_relation_with_heretic_religion", ModifierValueType.Float },
         { "reverse_relation_with_heathen_religion", ModifierValueType.Float },
         { "no_religion_penalty", ModifierValueType.Bool },
         { "extra_manpower_at_religious_war", ModifierValueType.Float },
         { "can_not_build_colonies", ModifierValueType.Bool },
         { "colonists", ModifierValueType.Int },
         { "colonist_placement_chance", ModifierValueType.Float },
         { "global_colonial_growth", ModifierValueType.Float },
         { "colony_cost_modifier", ModifierValueType.Float },
         { "range", ModifierValueType.Float },
         { "native_uprising_chance", ModifierValueType.Float },
         { "native_assimilation", ModifierValueType.Float },
         { "migration_cost", ModifierValueType.Float },
         { "global_tariffs", ModifierValueType.Float },
         { "treasure_fleet_income", ModifierValueType.Float },
         { "expel_minorities_cost", ModifierValueType.Float },
         { "may_explore", ModifierValueType.Bool },
         { "auto_explore_adjacent_to_colony", ModifierValueType.Bool },
         { "may_establish_frontier", ModifierValueType.Bool },
         { "free_maintenance_on_expl_conq", ModifierValueType.Bool },
         { "colony_development_boost", ModifierValueType.Float },
         { "caravan_power", ModifierValueType.Float },
         { "can_not_send_merchants", ModifierValueType.Bool },
         { "merchants", ModifierValueType.Int },
         { "placed_merchant_power", ModifierValueType.Float },
         { "placed_merchant_power_modifier", ModifierValueType.Float },
         { "global_trade_power", ModifierValueType.Float },
         { "global_foreign_trade_power", ModifierValueType.Float },
         { "global_own_trade_power", ModifierValueType.Float },
         { "global_prov_trade_power_modifier", ModifierValueType.Float },
         { "global_trade_goods_size_modifier", ModifierValueType.Float },
         { "global_trade_goods_size", ModifierValueType.Float },
         { "trade_efficiency", ModifierValueType.Float },
         { "trade_range_modifier", ModifierValueType.Float },
         { "trade_steering", ModifierValueType.Float },
         { "global_ship_trade_power", ModifierValueType.Float },
         { "privateer_efficiency", ModifierValueType.Float },
         { "embargo_efficiency", ModifierValueType.Float },
         { "ship_power_propagation", ModifierValueType.Float },
         { "center_of_trade_upgrade_cost", ModifierValueType.Float },
         { "trade_company_investment_cost", ModifierValueType.Float },
         { "mercantilism_cost", ModifierValueType.Float },
         { "reduced_trade_penalty_on_non_main_tradenode", ModifierValueType.Float },
         { "mil_tech_cost_modifier", ModifierValueType.Float },
         { "dip_tech_cost_modifier", ModifierValueType.Float },
         { "adm_tech_cost_modifier", ModifierValueType.Float }
      };

      public static readonly Dictionary<string, ModifierValueType> CustomModifierTrigger = new()
      {
         { "religion", ModifierValueType.Bool },
         { "secondary_religion", ModifierValueType.Bool },
         { "is_janissary_modifier", ModifierValueType.Bool },
         { "is_cossack_modifier", ModifierValueType.Bool },
         { "is_revolutionary_guard_modifier", ModifierValueType.Bool },
         { "is_rajput_modifier", ModifierValueType.Bool },
         { "is_mercenary_modifier", ModifierValueType.Bool },
         { "is_cawa_modifier", ModifierValueType.Bool },
         { "is_carolean_modifier", ModifierValueType.Bool },
         { "is_marine_modifier", ModifierValueType.Bool },
         { "is_banner_modifier", ModifierValueType.Bool },
         { "is_qizilbash_modifier", ModifierValueType.Bool },
         { "is_streltsy_modifier", ModifierValueType.Bool },
         { "is_hussars_modifier", ModifierValueType.Bool },
         { "is_samurai_modifier", ModifierValueType.Bool },
         { "is_tercio_modifier", ModifierValueType.Bool },
         { "is_mamluks_modifier", ModifierValueType.Bool },
         { "is_galleass_modifier", ModifierValueType.Bool },
         { "is_voc_indiamen_modifier", ModifierValueType.Bool }
      };

      public static readonly Dictionary<string, ModifierValueType> ProvinceModifiersDict = new Dictionary<string, ModifierValueType>
      {
          { "max_attrition", ModifierValueType.Float },
          { "attrition", ModifierValueType.Float },
          { "local_hostile_attrition", ModifierValueType.Float },
          { "local_attacker_dice_roll_bonus", ModifierValueType.Int },
          { "local_defender_dice_roll_bonus", ModifierValueType.Int },
          { "fort_level", ModifierValueType.Int },
          { "garrison_growth", ModifierValueType.Float },
          { "local_defensiveness", ModifierValueType.Float },
          { "local_fort_maintenance_modifier", ModifierValueType.Float },
          { "local_garrison_size", ModifierValueType.Int },
          { "local_garrison_damage", ModifierValueType.Float },
          { "local_assault_fort_cost_modifier", ModifierValueType.Float },
          { "local_assault_fort_ability", ModifierValueType.Float },
          { "local_friendly_movement_speed", ModifierValueType.Float },
          { "local_hostile_movement_speed", ModifierValueType.Float },
          { "local_manpower", ModifierValueType.Int },
          { "local_manpower_modifier", ModifierValueType.Float },
          { "local_regiment_cost", ModifierValueType.Float },
          { "regiment_recruit_speed", ModifierValueType.Float },
          { "supply_limit", ModifierValueType.Int },
          { "supply_limit_modifier", ModifierValueType.Float },
          { "local_own_coast_naval_combat_bonus", ModifierValueType.Float },
          { "local_has_banners", ModifierValueType.Bool },
          { "local_amount_of_banners", ModifierValueType.Int },
          { "local_amount_of_cawa", ModifierValueType.Int },
          { "local_has_carolean", ModifierValueType.Bool },
          { "local_amount_of_carolean", ModifierValueType.Int },
          { "local_amount_of_hussars", ModifierValueType.Int },
          { "local_has_streltsy", ModifierValueType.Bool },
          { "local_has_tercio", ModifierValueType.Bool },
          { "local_has_musketeers", ModifierValueType.Bool },
          { "local_has_samurai", ModifierValueType.Bool },
          { "local_has_mamluks", ModifierValueType.Bool },
          { "local_has_qizilbash", ModifierValueType.Bool },
          { "local_naval_engagement_modifier", ModifierValueType.Float },
          { "local_sailors", ModifierValueType.Int },
          { "local_sailors_modifier", ModifierValueType.Float },
          { "local_ship_cost", ModifierValueType.Float },
          { "local_ship_repair", ModifierValueType.Float },
          { "ship_recruit_speed", ModifierValueType.Float },
          { "blockade_force_required", ModifierValueType.Int },
          { "hostile_disembark_speed", ModifierValueType.Float },
          { "hostile_fleet_attrition", ModifierValueType.Float },
          { "block_slave_raid", ModifierValueType.Bool },
          { "local_has_geobukseon", ModifierValueType.Bool },
          { "local_has_man_of_war", ModifierValueType.Bool },
          { "local_has_galleon", ModifierValueType.Bool },
          { "local_has_galleass", ModifierValueType.Bool },
          { "local_has_caravel", ModifierValueType.Bool },
          { "local_has_voc_indiamen", ModifierValueType.Bool },
          { "local_warscore_cost_modifier", ModifierValueType.Float },
          { "inflation_reduction_local", ModifierValueType.Float },
          { "local_state_maintenance_modifier", ModifierValueType.Float },
          { "local_build_cost", ModifierValueType.Float },
          { "local_build_time", ModifierValueType.Float },
          { "local_great_project_upgrade_cost", ModifierValueType.Float },
          { "local_great_project_upgrade_time", ModifierValueType.Float },
          { "local_monthly_devastation", ModifierValueType.Float },
          { "local_prosperity_growth", ModifierValueType.Float },
          { "local_production_efficiency", ModifierValueType.Float },
          { "local_tax_modifier", ModifierValueType.Float },
          { "tax_income", ModifierValueType.Int },
          { "allowed_num_of_buildings", ModifierValueType.Int },
          { "allowed_num_of_manufactories", ModifierValueType.Int },
          { "local_development_cost", ModifierValueType.Float },
          { "local_development_cost_modifier", ModifierValueType.Float },
          { "local_gold_depletion_chance_modifier", ModifierValueType.Float },
          { "local_institution_spread", ModifierValueType.Float },
          { "local_core_creation", ModifierValueType.Float },
          { "local_governing_cost", ModifierValueType.Float },
          { "statewide_governing_cost", ModifierValueType.Float },
          { "local_governing_cost_increase", ModifierValueType.Float },
          { "local_centralize_state_cost", ModifierValueType.Float },
          { "institution_growth", ModifierValueType.Float },
          { "local_culture_conversion_cost", ModifierValueType.Float },
          { "local_culture_conversion_time", ModifierValueType.Float },
          { "local_unrest", ModifierValueType.Float },
          { "local_autonomy", ModifierValueType.Float },
          { "local_years_of_nationalism", ModifierValueType.Int },
          { "min_local_autonomy", ModifierValueType.Float },
          { "local_missionary_strength", ModifierValueType.Percentile },
          { "local_religious_unity_contribution", ModifierValueType.Float },
          { "local_missionary_maintenance_cost", ModifierValueType.Float },
          { "local_religious_conversion_resistance", ModifierValueType.Float },
          { "local_tolerance_of_heretics", ModifierValueType.Float },
          { "local_tolerance_of_heathens", ModifierValueType.Float },
          { "local_colonial_growth", ModifierValueType.Float },
          { "local_colonist_placement_chance", ModifierValueType.Float },
          { "local_colony_cost_modifier", ModifierValueType.Float },
          { "province_trade_power_modifier", ModifierValueType.Float },
          { "province_trade_power_value", ModifierValueType.Float },
          { "trade_goods_size_modifier", ModifierValueType.Float },
          { "trade_goods_size", ModifierValueType.Float },
          { "trade_value_modifier", ModifierValueType.Float },
          { "trade_value", ModifierValueType.Float },
          { "local_center_of_trade_upgrade_cost", ModifierValueType.Float }
      };

      public static readonly HashSet<string> RemovedModifiers =
      [
         "overseas_income", "local_tariffs", "colonist_time", "leader_fire", "leader_shock", "minimum_revolt_risk",
         "local_revolt_risk", "global_revolt_risk", "colonial_liberty_desire", "merchant_steering_to_inland",
         "may_force_march", "merchant_present_inland", "build_power_cost", "global_trade_income_modifier",
         "reduced_native_attacks", "local_spy_defence", "may_study_technology", "may_sow_discontent",
         "may_sabotage_reputation", "may_infiltrate_administration", "may_agitate_for_liberty",
         "accepted_culture_threshold", "relations_decay_of_me", "local_movement_speed",
         "cs_only_local_development_cost", "max_states", "possible_mercenaries", "religious_conversion_resistance",
         "migration_cooldown", "artillery_bonus_vs_fort", "yearly_tribal_allegiance", "can_colony_boost_development"
      ];
   }
}