using System.Security.Policy;
using System.Text.RegularExpressions;
using Editor.DataClasses.GameDataClasses;
using Editor.Helper;

namespace Editor.Parser
{
   public static class ModifierParser
   {
      private const string PROVINCE_MODIFIER_REGEX = "name\\s*=\\s*(.*)\\s*duration\\s*=\\s*(.*)";
      private static readonly Regex ProvinceModifierRegex = new(PROVINCE_MODIFIER_REGEX, RegexOptions.Compiled);

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

         var name = match.Groups[1].Value;
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
         if (CountryModifiers.Contains(name))
         {
            modifier = new(name, value);
            return true;
         }
         if (ProvinceModifiers.Contains(name))
         {
            modifier = new(name, value, Scope.Province);
            return true;
         }
         modifier = default!;
         return false;
      }

      public static bool IsValidModifier(string name)
      {
         return CountryModifiers.Contains(name) || ProvinceModifiers.Contains(name);
      }

      public static bool IsRemovedModifier(string name)
      {
         return RemovedModifiers.Contains(name);
      }


      public static readonly HashSet<string> CountryModifiers = ["army_tradition", "army_tradition_decay", "army_tradition_from_battle", "yearly_army_professionalism", "drill_gain_modifier", "drill_decay_modifier", "infantry_cost", "infantry_power", "infantry_fire", "infantry_shock", "cavalry_cost", "cavalry_power", "cavalry_fire", "cavalry_shock", "artillery_cost", "artillery_power", "artillery_fire", "artillery_shock", "cav_to_inf_ratio", "cavalry_flanking", "artillery_levels_available_vs_fort", "artillery_level_modifier", "backrow_artillery_damage", "discipline", "land_morale", "land_morale_constant", "movement_speed", "fire_damage", "fire_damage_received", "shock_damage", "shock_damage_received", "morale_damage", "morale_damage_received", "recover_army_morale_speed", "reserves_organisation", "land_attrition", "reinforce_cost_modifier", "no_cost_for_reinforcing", "reinforce_speed", "manpower_recovery_speed", "global_manpower", "global_manpower_modifier", "global_regiment_cost", "global_regiment_recruit_speed", "global_supply_limit_modifier", "land_forcelimit", "land_forcelimit_modifier", "land_maintenance_modifier", "possible_condottieri", "hostile_attrition", "max_hostile_attrition", "siege_ability", "artillery_barrage_cost", "assault_fort_cost_modifier", "assault_fort_ability", "defensiveness", "garrison_size", "global_garrison_growth", "garrison_damage", "fort_maintenance_modifier", "rival_border_fort_maintenance", "war_exhaustion", "war_exhaustion_cost", "leader_land_fire", "leader_land_manuever", "leader_land_shock", "leader_siege", "max_general_fire", "max_general_maneuver", "max_general_shock", "max_general_siege", "general_cost", "free_leader_pool", "free_land_leader_pool", "free_navy_leader_pool", "raze_power_gain", "loot_amount", "available_province_loot", "prestige_from_land", "war_taxes_cost_modifier", "leader_cost", "may_recruit_female_generals", "manpower_in_true_faith_provinces", "regiment_manpower_usage", "military_tactics", "capped_by_forcelimit", "global_attacker_dice_roll_bonus", "global_defender_dice_roll_bonus", "own_territory_dice_roll_bonus", "manpower_in_accepted_culture_provinces", "manpower_in_culture_group_provinces", "manpower_in_own_culture_provinces", "may_build_supply_depot", "may_refill_garrison", "may_return_manpower_on_disband", "attack_bonus_in_capital_terrain", "can_bypass_forts", "force_march_free", "enable_forced_march", "warscore_from_battles_modifier", "mercenary_cost", "merc_maintenance_modifier", "mercenary_discipline", "mercenary_manpower", "merc_leader_army_tradition", "allow_mercenary_drill", "merc_independent_from_trade_range", "allow_mercenaries_to_split", "special_unit_forcelimit", "special_unit_cost_modifier", "special_unit_manpower_cost_modifier", "has_marines", "allowed_marine_fraction", "has_banners", "amount_of_banners", "can_recruit_cawa", "amount_of_cawa", "cawa_cost_modifier", "has_carolean", "amount_of_carolean", "can_recruit_hussars", "amount_of_hussars", "hussars_cost_modifier", "can_recruit_janissaries", "janissary_cost_modifier", "allow_janissaries_from_own_faith", "can_recruit_cossacks", "allowed_cossack_fraction", "can_recruit_rajputs", "allowed_rajput_fraction", "can_recruit_revolutionary_guards", "allowed_rev_guard_fraction", "has_streltsy", "allowed_streltsy_fraction", "amount_of_streltsy", "has_tercio", "allowed_tercio_fraction", "amount_of_tercio", "has_samurai", "allowed_samurai_fraction", "amount_of_samurai", "has_musketeer", "allowed_musketeer_fraction", "amount_of_musketeers", "has_mamluks", "allowed_mamluks_fraction", "amount_of_mamluks", "has_qizilbash", "allowed_qizilbash_fraction", "amount_of_qizilbash", "navy_tradition", "navy_tradition_decay", "naval_tradition_from_battle", "naval_tradition_from_trade", "heavy_ship_cost", "heavy_ship_power", "light_ship_cost", "light_ship_power", "galley_cost", "galley_power", "transport_cost", "transport_power", "global_ship_cost", "global_ship_recruit_speed", "global_ship_repair", "naval_forcelimit", "naval_forcelimit_modifier", "naval_maintenance_modifier", "global_sailors", "global_sailors_modifier", "sailor_maintenance_modifer", "sailors_recovery_speed", "blockade_efficiency", "siege_blockade_progress", "capture_ship_chance", "global_naval_engagement_modifier", "global_naval_engagement", "naval_attrition", "naval_morale", "naval_morale_constant", "naval_morale_damage", "naval_morale_damage_received", "ship_durability", "sunk_ship_morale_hit_recieved", "recover_navy_morale_speed", "prestige_from_naval", "leader_naval_fire", "leader_naval_manuever", "leader_naval_shock", "max_admiral_fire", "max_admiral_maneuver", "max_admiral_shock", "max_admiral_siege", "own_coast_naval_combat_bonus", "admiral_cost", "global_naval_barrage_cost", "flagship_cost", "disengagement_chance", "transport_attrition", "landing_penalty", "regiment_disembark_speed", "may_perform_slave_raid", "may_perform_slave_raid_on_same_religion", "coast_raid_range", "sea_repair", "movement_speed_in_fleet_modifier", "max_flagships", "number_of_cannons", "number_of_cannons_modifier", "heavy_ship_number_of_cannons_modifier", "light_ship_number_of_cannons_modifier", "galley_number_of_cannons_modifier", "transport_number_of_cannons_modifier", "hull_size", "hull_size_modifier", "heavy_ship_hull_size_modifier", "light_ship_hull_size_modifier", "galley_hull_size_modifier", "transport_hull_size_modifier", "engagement_cost", "engagement_cost_modifier", "ship_trade_power", "ship_trade_power_modifier", "can_transport_units", "admiral_skill_gain_modifier", "flagship_durability", "flagship_morale", "flagship_naval_engagement_modifier", "movement_speed_onto_off_boat_modifier", "flagship_landing_penalty", "number_of_cannons_flagship_modifier", "number_of_cannons_flagship", "naval_maintenance_flagship_modifier", "trade_power_in_fleet_modifier", "morale_in_fleet_modifier", "blockade_impact_on_siege_in_fleet_modifier", "exploration_mission_range_in_fleet_modifier", "barrage_cost_in_fleet_modifier", "naval_attrition_in_fleet_modifier", "privateering_efficiency_in_fleet_modifier", "prestige_from_battles_in_fleet_modifier", "naval_tradition_in_fleet_modifier", "cannons_for_hunting_pirates_in_fleet", "has_geobukseon", "allowed_geobukseon_fraction", "amount_of_geobukseon", "has_galleass", "allowed_galleass_fraction", "amount_of_galleass", "has_voc_indiamen", "allowed_voc_indiamen_fraction", "amount_of_voc_indiamen", "has_man_of_war", "allowed_man_of_war_fraction", "amount_of_man_of_war", "has_galleon", "allowed_galleon_fraction", "amount_of_galleon", "has_caravel", "allowed_caravel_fraction", "amount_of_caravel", "diplomats", "diplomatic_reputation", "diplomatic_upkeep", "envoy_travel_time", "improve_relation_modifier", "ae_impact", "diplomatic_annexation_cost", "province_warscore_cost", "unjustified_demands", "rival_change_cost", "justify_trade_conflict_cost", "stability_cost_to_declare_war", "accept_vassalization_reasons", "transfer_trade_power_reasons", "monthly_federation_favor_growth", "monthly_favor_modifier", "cb_on_overseas", "cb_on_primitives", "idea_claim_colonies", "cb_on_government_enemies", "cb_on_religious_enemies", "reduced_stab_impacts", "can_fabricate_for_vassals", "global_tax_income", "global_tax_modifier", "production_efficiency", "state_maintenance_modifier", "inflation_action_cost", "may_not_reduce_inflation", "inflation_reduction", "monthly_gold_inflation_modifier", "gold_depletion_chance_modifier", "interest", "can_not_build_buildings", "development_cost", "development_cost_in_primary_culture", "development_cost_modifier", "tribal_development_growth", "add_tribal_land_cost", "settle_cost", "global_allowed_num_of_buildings", "global_allowed_num_of_manufactories", "build_cost", "build_time", "great_project_upgrade_cost", "great_project_upgrade_time", "global_monthly_devastation", "global_prosperity_growth", "administrative_efficiency", "free_concentrate_development", "expand_infrastructure_cost_modifier", "centralize_state_cost", "core_creation", "core_decay_on_your_own", "enemy_core_creation", "ignore_coring_distance", "adm_tech_cost_modifier", "technology_cost", "idea_cost", "embracement_cost", "global_institution_spread", "institution_spread_from_true_faith", "native_advancement_cost", "all_power_cost", "innovativeness_gain", "yearly_innovativeness", "free_adm_policy", "free_dip_policy", "free_mil_policy", "possible_adm_policy", "possible_dip_policy", "possible_mil_policy", "possible_policy", "free_policy", "country_admin_power", "country_diplomatic_power", "country_military_power", "prestige", "prestige_decay", "monthly_splendor", "yearly_corruption", "advisor_cost", "same_culture_advisor_cost", "same_religion_advisor_cost", "advisor_pool", "female_advisor_chance", "heir_chance", "monthly_heir_claim_increase", "monthly_heir_claim_increase_modifier", "block_introduce_heir", "monarch_admin_power", "monarch_diplomatic_power", "monarch_military_power", "adm_advisor_cost", "dip_advisor_cost", "mil_advisor_cost", "monthly_support_heir_gain", "power_projection_from_insults", "monarch_lifespan", "local_heir_adm", "local_heir_dip", "local_heir_mil", "national_focus_years", "yearly_absolutism", "max_absolutism", "max_absolutism_effect", "legitimacy", "republican_tradition", "devotion", "horde_unity", "meritocracy", "monthly_militarized_society", "monthly_<government_power_type_id>", "yearly_government_power", "<faction>_influence", "imperial_mandate", "election_cycle", "candidate_random_bonus", "reelection_cost", "max_terms", "governing_capacity", "governing_capacity_modifier", "governing_cost", "state_governing_cost", "territories_governing_cost", "trade_company_governing_cost", "state_governing_cost_increase", "expand_administration_cost", "yearly_revolutionary_zeal", "max_revolutionary_zeal", "reform_progress_growth", "monthly_reform_progress", "monthly_reform_progress_modifier", "move_capital_cost_modifier", "can_revoke_parliament_seats", "parliament_backing_chance", "parliament_effect_duration", "parliament_debate_duration", "parliament_chance_of_decision", "num_of_parliament_issues", "max_possible_parliament_seats", "church_influence_modifier", "church_loyalty_modifier", "nobles_privilege_slots", "all_estate_influence_modifier", "all_estate_loyalty_equilibrium", "all_estate_possible_privileges", "allow_free_estate_privilege_revocation", "loyalty_change_on_revoked", "estate_interaction_cooldown_modifier", "imperial_authority", "imperial_authority_value", "free_city_imperial_authority", "reasons_to_elect", "imperial_mercenary_cost", "max_free_cities", "max_electors", "legitimate_subject_elector", "manpower_against_imperial_enemies", "imperial_reform_catholic_approval", "culture_conversion_cost", "culture_conversion_time", "num_accepted_cultures", "promote_culture_cost", "relation_with_same_culture", "relation_with_same_culture_group", "relation_with_accepted_culture", "relation_with_other_culture", "can_not_declare_war", "global_unrest", "stability_cost_modifier", "global_autonomy", "min_autonomy", "autonomy_change_time", "harsh_treatment_cost", "global_rebel_suppression_efficiency", "years_of_nationalism", "min_autonomy_in_territories", "unrest_catholic_provinces", "no_stability_loss_on_monarch_death", "overextension_impact_modifier", "vassal_forcelimit_bonus", "vassal_naval_forcelimit_bonus", "vassal_manpower_bonus", "vassal_sailors_bonus", "vassal_income", "overlord_naval_forcelimit", "overlord_naval_forcelimit_modifier", "can_transfer_vassal_wargoal", "liberty_desire", "liberty_desire_from_subject_development", "reduced_liberty_desire", "reduced_liberty_desire_on_same_continent", "reduced_liberty_desire_on_other_continent", "allow_client_states", "colonial_type_change_cost_modifier", "colonial_subject_type_upgrade_cost_modifier", "years_to_integrate_personal_union", "chance_to_inherit", "monarch_power_tribute", "tributary_conversion_cost_modifier", "annexation_relations_impact", "spy_offence", "global_spy_defence", "fabricate_claims_cost", "claim_duration", "can_chain_claim", "discovered_relations_impact", "rebel_support_efficiency", "can_claim_states", "no_claim_cost_increasement", "spy_action_cost_modifier", "global_missionary_strength", "global_heretic_missionary_strength", "global_heathen_missionary_strength", "can_not_build_missionaries", "may_not_convert_territories", "missionaries", "missionary_maintenance_cost", "religious_unity", "tolerance_own", "tolerance_heretic", "tolerance_heathen", "tolerance_of_heretics_capacity", "tolerance_of_heathens_capacity", "papal_influence", "papal_influence_from_cardinals", "appoint_cardinal_cost", "curia_treasury_contribution", "curia_powers_cost", "monthly_church_power", "church_power_modifier", "monthly_fervor_increase", "yearly_patriarch_authority", "monthly_piety", "monthly_piety_accelerator", "harmonization_speed", "yearly_harmony", "monthly_karma", "monthly_karma_accelerator", "yearly_karma_decay", "yearly_doom_reduction", "yearly_authority", "enforce_religion_cost", "prestige_per_development_from_conversion", "warscore_cost_vs_other_religion", "establish_order_cost", "global_religious_conversion_resistance", "relation_with_same_religion", "relation_with_heretics", "relation_with_heathens", "reverse_relation_with_same_religion", "reverse_relation_with_heretic_religion", "reverse_relation_with_heathen_religion", "no_religion_penalty", "extra_manpower_at_religious_war", "can_not_build_colonies", "colonists", "colonist_placement_chance", "global_colonial_growth", "colony_cost_modifier", "range", "native_uprising_chance", "native_assimilation", "migration_cost", "global_tariffs", "treasure_fleet_income", "expel_minorities_cost", "may_explore", "auto_explore_adjacent_to_colony", "may_establish_frontier", "free_maintenance_on_expl_conq", "colony_development_boost", "caravan_power", "can_not_send_merchants", "merchants", "placed_merchant_power", "placed_merchant_power_modifier", "global_trade_power", "global_foreign_trade_power", "global_own_trade_power", "global_prov_trade_power_modifier", "global_trade_goods_size_modifier", "global_trade_goods_size", "trade_efficiency", "trade_range_modifier", "trade_steering", "global_ship_trade_power", "privateer_efficiency", "embargo_efficiency", "ship_power_propagation", "center_of_trade_upgrade_cost", "trade_company_investment_cost", "mercantilism_cost", "reduced_trade_penalty_on_non_main_tradenode"];

      public static readonly HashSet<string> ProvinceModifiers = ["max_attrition", "attrition", "local_hostile_attrition", "local_attacker_dice_roll_bonus", "local_defender_dice_roll_bonus", "fort_level", "garrison_growth", "local_defensiveness", "local_fort_maintenance_modifier", "local_garrison_size", "local_garrison_damage", "local_assault_fort_cost_modifier", "local_assault_fort_ability", "local_friendly_movement_speed", "local_hostile_movement_speed", "local_manpower", "local_manpower_modifier", "local_regiment_cost", "regiment_recruit_speed", "supply_limit", "supply_limit_modifier", "local_own_coast_naval_combat_bonus", "local_has_banners", "local_amount_of_banners", "local_amount_of_cawa", "local_has_carolean", "local_amount_of_carolean", "local_amount_of_hussars", "local_has_streltsy", "local_has_tercio", "local_has_musketeers", "local_has_samurai", "local_has_mamluks", "local_has_qizilbash", "local_naval_engagement_modifier", "local_sailors", "local_sailors_modifier", "local_ship_cost", "local_ship_repair", "ship_recruit_speed", "blockade_force_required", "hostile_disembark_speed", "hostile_fleet_attrition", "block_slave_raid", "local_has_geobukseon", "local_has_man_of_war", "local_has_galleon", "local_has_galleass", "local_has_caravel", "local_has_voc_indiamen", "local_warscore_cost_modifier", "inflation_reduction_local", "local_state_maintenance_modifier", "local_build_cost", "local_build_time", "local_great_project_upgrade_cost", "local_great_project_upgrade_time", "local_monthly_devastation", "local_prosperity_growth", "local_production_efficiency", "local_tax_modifier", "tax_income", "allowed_num_of_buildings", "allowed_num_of_manufactories", "local_development_cost", "local_development_cost_modifier", "local_gold_depletion_chance_modifier", "local_institution_spread", "local_core_creation", "local_governing_cost", "statewide_governing_cost", "local_governing_cost_increase", "local_centralize_state_cost", "institution_growth", "local_culture_conversion_cost", "local_culture_conversion_time", "local_unrest", "local_autonomy", "local_years_of_nationalism", "min_local_autonomy", "local_missionary_strength", "local_religious_unity_contribution", "local_missionary_maintenance_cost", "local_religious_conversion_resistance", "local_tolerance_of_heretics", "local_tolerance_of_heathens", "local_colonial_growth", "local_colonist_placement_chance", "local_colony_cost_modifier", "province_trade_power_modifier", "province_trade_power_value", "trade_goods_size_modifier", "trade_goods_size", "trade_value_modifier", "trade_value", "local_center_of_trade_upgrade_cost"];

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