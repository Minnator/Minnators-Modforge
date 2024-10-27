using Editor.DataClasses.GameDataClasses;
using Editor.Helper;

namespace Editor.Parser
{
   public static class EffectParser
   {
      public static bool IsAnyEffect(string input)
      {
         return CountryEffects.Contains(input) || ProvinceEffects.Contains(input);
      }

      public static bool IsAnyEffectCountryHistory(string input)
      {
         return IsAnyEffect(input) || CountryHistoryEffects.Contains(input);
      }

      public static bool ParseEffects(string input, out List<Effect> effects)
      {
         effects = [];
         return ParseEffects(effectsValues: Parsing.GetKeyValueList(ref input), effects:out effects);
      }

      public static bool ParseEffects(List<KeyValuePair<string, string>> effectsValues, out List<Effect> effects)
      {
         effects = [];
         foreach (var kvp in effectsValues)
         {
            if (ParseEffect(kvp.Key, kvp.Value, out var effect))
               effects.Add(effect);
            else
            {
               Globals.ErrorLog.Write($"Could not parse effect: {kvp.Key} = {kvp.Value}");
               return false;
            }
         }
         return true;
      }

      public static bool ParseScriptedEffect(string name, string input, out Effect effect)
      {
         if (input.Equals("yes"))
         {
            effect = new ScriptedEffect(name, input, EffectValueType.Complex, Scope.Country); //TODO check if it is country scope
         }
         else
         {
            var attr = input.Split('=');
            if (attr.Length % 2 != 0)
            {
               effect = default!;
               return false;
            }
            List<KeyValuePair<string, string>> effects = [];
            for (var i = 0; i < attr.Length; i += 2)
            {
               effects.Add(new(attr[i], attr[i + 1]));
            }
            effect = new ScriptedEffect(name, input, EffectValueType.Complex, Scope.Country) { AttributesList = effects };//TODO check if it is country scope
         }
         return true;
      }

      public static bool ParseEffect(string name, string value, out Effect effect)
      {
         if (Globals.ScriptedEffectNames.Contains(name))
            return ParseScriptedEffect(name, value, out effect);
         return ParseSimpleEffect(name, value, out effect);
      }

      public static bool ParseSimpleEffect(string name, string value, out Effect effect)
      {
         effect = Effect.Empty;

         if (CountryEffects.Contains(name))
         {
            effect = new SimpleEffect(name, value, EffectValueType.String, Scope.Country);
            return true;
         }
         if (!ProvinceEffects.Contains(name))
            return false;

         effect = new SimpleEffect(name, value, EffectValueType.String, Scope.Province);
         return true;
      }


      public static bool ParseOpinionEffects(string exactModifierName, string input, out OpinionEffect opinionModifier, Scope scope = Scope.Country)
      {
         opinionModifier = new(exactModifierName, string.Empty, EffectValueType.Complex, scope);

         var attr = input.Split('=', '\n');
         if (attr.Length % 2 != 0 || attr.Length < 4)
            return false;

         for (var i = 0; i < attr.Length; i += 2)
         {
            if (attr[i].Trim().ToLower().Equals("who"))
            {
               if (Tag.TryParse(attr[i + 1].Trim(), out var tag))
               {
                  opinionModifier.Who = tag;
               }
               else
               {
                  Globals.ErrorLog.Write($"Could not parse tag for 'who' attribute in 'opinion' effect: {attr[i + 1]}");
                  return false;
               }
               continue;
            }
            if (attr[i].Trim().ToLower().Equals("modifier"))
            {
               opinionModifier.Modifier = attr[i + 1].Trim();
               continue;
            }
            Globals.ErrorLog.Write($"Unknown attribute in 'opinion' effect: '{attr[i]}'");
         }

         return true;
      }

      public static bool ParseAddEstateLoyaltyEffect(string input, out AddEstateLoyaltyEffect loyaltyEffect)
      {
         loyaltyEffect = new("add_estate_loyalty", string.Empty, EffectValueType.Complex, Scope.Country);

         if (!Parsing.GetSimpleKvpArray(input, out var kvps))
            return false;

         for (var i = 0; i < kvps.Length; i += 2)
         {
            var key = kvps[i].Trim();
            var value = kvps[i + 1].Trim();

            if (key.Equals("estate"))
               loyaltyEffect.Estate = value;
            else if (key.Equals("loyalty"))
               if (!int.TryParse(value, out var loyalty))
                  return false;
               else
                  loyaltyEffect.Loyalty = loyalty;
            else
               Globals.ErrorLog.Write($"Unknown attribute in 'add_estate_loyalty' effect: '{kvps[i]}'");
         }
         return true;
      }

      public static bool ParseSpawnRebels(string input, out SpawnRebelsEffect rebels)
      {
         rebels = default!;

         var attr = input.Split('=', '\n');
         if (attr.Length % 2 != 0)
            return false;

         // type and size are required for constructing the effect so they are checked first
         var type = string.Empty;
         var size = -1;
         for (var i = 0; i < attr.Length; i += 2)
         {
            attr[i] = attr[i].Trim();
            attr[i + 1] = attr[i + 1].Trim();
            if (attr[i].Equals("type"))
               type = attr[i + 1];
            else if (attr[i].Equals("size"))
               if (!int.TryParse(attr[i + 1], out size))
                  return false;
         }

         if (string.IsNullOrEmpty(type) || size == -1)
         {
            Globals.ErrorLog.Write($"Could not parse 'spawn_rebels' effect: {input}");
            return false;
         }

         rebels = new("rebels", string.Empty, EffectValueType.Complex, Scope.Province) { RebelType = type, RebelSize = size};

         for (var i = 0; i < attr.Length; i += 2)
         {
            switch (attr[i])
            {
               case "type":
               case "size":
               break;
               case "culture":
                  rebels.Culture = attr[i + 1];
                  break;
               case "religion":
                  rebels.Religion = attr[i + 1];
                  break;
               case "leader":
                  rebels.Leader = attr[i + 1];
                  break;
               case "leader_dynasty":
                  rebels.LeaderDynasty = attr[i + 1];
                  break;
               case "estate":
                  rebels.Estate = attr[i + 1];
                  break;
               case "unrest":
                  if (!int.TryParse(attr[i + 1], out var unrest))
                     return false;
                  rebels.Unrest = unrest;
                  break;
               case "win":
                  if (Parsing.YesNo(attr[i + 1]))
                     rebels.Win = true;
                  break;
               case "female":
                  if (Parsing.YesNo(attr[i + 1]))
                     rebels.Female = false;
                  break;
               case "use_heir_as_leader":
                  if (Parsing.YesNo(attr[i + 1]))
                     rebels.UseHeirAsLeader = true;
                  break;
               case "use_consort_as_leader":
                  if (Parsing.YesNo(attr[i + 1]))
                     rebels.UseConsortAsLeader = true;
                  break;
               case "as_if_faction":
                  if (Parsing.YesNo(attr[i + 1]))
                     rebels.AsIfFaction = true;
                  break;
               case "should_take_capital":
                  if (Parsing.YesNo(attr[i + 1]))
                     rebels.ShouldTakeCapital = true;
                  break;
               case "separatist_target":
                  if (Tag.TryParse(attr[i + 1], out var tag))
                     rebels.SeparatistTarget = tag;
                  break;
               case "friend":
                  if (Tag.TryParse(attr[i + 1], out var tag2))
                     rebels.Friend = tag2;
                  break;
               default:
                  Globals.ErrorLog.Write($"Unknown attribute in 'spawn_rebels' effect: '{attr[i]}'");
                  break;
            }
         }

         return true;
      }

      private static readonly HashSet<string> CountryHistoryEffects =
      [
         "religion", "government", "religious_school", "capital", "technology_group", "primary_culture", "decision", "government_rank", "secondary_religion", "unit_type", "share", "mercantilism", "revolution_target", "changed_tag_from", "culture", "set_government"
      ];

      private static readonly HashSet<string> CountryEffects = 
      [
         "set_global_flag", "clr_global_flag", "custom_tooltip", "log", "save_event_target_as", "save_global_event_target_as", "clear_global_event_target", "clear_global_event_targets", "show_ambient_object", "hide_ambient_object", "enable_council", "finish_council", "country_event", "add_country_modifier", "extend_country_modifier", "remove_country_modifier", "set_country_flag", "clr_country_flag", "change_tag", "switch_tag", "change_graphical_culture", "override_country_name", "restore_country_name", "change_country_color", "restore_country_color", "add_adm_power", "add_dip_power", "add_mil_power", "set_saved_name", "clear_saved_name", "change_innovativeness", "complete_mission", "swap_non_generic_missions", "adm_power_cost", "dip_power_cost", "mil_power_cost", "remove_country", "play_sound", "add_years_of_income", "add_treasury", "add_inflation", "add_mercantilism", "add_owned_provinces_development_ducats", "add_owned_provinces_development_manpower", "add_tariff_value", "loan_size", "change_price", "add_loan", "raise_war_taxes", "add_years_of_owned_provinces_production_income", "add_years_of_owned_provinces_manpower", "add_years_of_owned_provinces_sailors", "set_bankruptcy", "steer_trade", "add_absolutism", "add_government_reform", "remove_government_reform", "change_government_reform_progress", "add_legitimacy", "add_republican_tradition", "add_scaled_republican_tradition", "add_devotion", "add_horde_unity", "add_meritocracy", "set_meritocracy", "add_militarised_society", "add_revolutionary_zeal", "add_tribal_allegiance", "adopt_reform_progress", "change_statists_vs_orangists", "change_statists_vs_monarchists", "change_government", "set_government_rank", "add_government_power", "add_government_power_scaled_to_seats", "set_government_power", "freeze_government_power", "unfreeze_government_power", "regenerate_government_mechanics", "set_government_and_rank", "set_legacy_government", "dissolve_parliament", "reinstate_parliament", "start_debate", "end_current_debate", "cancel_current_debate", "remove_enacted_issue", "enact_issue", "add_accepted_culture", "change_primary_culture", "remove_accepted_culture", "change_religion", "enable_religion", "force_converted", "add_authority", "add_doom", "remove_religious_reforms", "add_fervor", "add_karma", "set_karma", "add_church_power", "add_church_aspect", "remove_church_aspect", "set_defender_of_the_faith", "remove_defender_of_the_faith", "set_papal_controller", "transfer_papal_controller", "add_papal_influence", "add_reform_desire", "excommunicate", "set_papacy_active", "add_piety", "add_curia_treasury", "set_school_opinion", "set_religious_school", "add_patriarch_authority", "change_personal_deity", "add_harmony", "add_harmonized_religion", "add_harmonization_progress", "unlock_cult", "change_cult", "add_isolationism", "set_isolationism", "add_incident_variable_value", "set_incident_variable_value", "add_idea", "add_idea_group", "add_active_policy", "change_technology_group", "remove_idea", "remove_idea_group", "set_primitive", "swap_free_idea_group", "add_next_institution_embracement", "add_next_institution_embracement_scaled", "add_adm_tech", "add_dip_tech", "add_mil_tech", "add_prestige", "set_capital", "add_corruption", "add_splendor", "create_advisor", "kill_advisor", "remove_advisor", "remove_advisor_by_category", "define_advisor", "hire_advisor", "extend_regency", "regency", "change_national_focus", "add_power_projection", "remove_power_projection", "add_stability", "add_war_exhaustion", "add_liberty_desire", "disband_rebels", "collapse_nation", "add_disaster_modifier", "add_disaster_progress", "end_disaster", "add_rebel_progress", "add_army_tradition", "add_navy_tradition", "add_army_professionalism", "add_manpower", "add_sailors", "add_yearly_manpower", "add_yearly_sailors", "create_admiral", "create_explorer", "create_conquistador", "create_general", "change_unit_type", "kill_leader", "define_admiral", "define_explorer", "define_conquistador", "define_general", "add_company_manpower", "disband_mercenary_company", "disband_all_units", "disband_all_regiments", "disband_all_ships", "create_unit_forcelimit_percentage", "set_naval_doctrine", "add_historical_friend", "add_historical_rival", "remove_historical_friend", "remove_historical_rival", "add_rival", "remove_rival", "force_remove_rival", "add_trust", "add_favors", "add_opinion", "reverse_add_opinion", "remove_opinion", "reverse_remove_opinion", "create_alliance", "remove_alliance", "create_guarantee", "remove_guarantee", "create_marriage", "create_union", "create_vassal", "create_march", "vassalize", "create_subject", "break_marriage", "break_alliance", "break_union", "inherit", "release", "release_all_subjects", "release_all_possible_countries", "grant_independence", "free_vassal", "declare_war", "declare_war_with_cb", "add_truce_with", "white_peace", "form_coalition_against", "add_casus_belli", "reverse_add_casus_belli", "remove_casus_belli", "reverse_remove_casus_belli", "remove_fow", "remove_hegemon", "add_spy_network_from", "add_spy_network_in", "join_all_offensive_wars_of", "join_all_defensive_wars_of", "add_aggressive_expansion", "reverse_add_aggressive_expansion", "add_subjects_development_ducats", "add_subjects_development_manpower", "create_independent_estate", "create_independent_estate_from_religion", "add_estate_influence_modifier", "add_estate_loyalty", "add_estate_loyalty_modifier", "add_faction", "remove_faction", "add_faction_influence", "set_estate_privilege", "remove_estate_privilege", "set_estate_led_regency_privilege", "change_estate_land_share", "start_estate_agenda", "change_adm", "change_dip", "change_mil", "add_ruler_personality", "remove_ruler_personality", "clear_scripted_personalities", "set_ruler_flag", "clr_ruler_flag", "set_dynasty", "set_ruler_culture", "set_ruler_religion", "exile_ruler_as", "set_ruler", "kill_ruler", "convert_female_ruler_to_general", "convert_ruler_to_general", "convert_ruler_to_admiral", "add_ruler_modifier", "define_ruler_to_general", "define_leader_to_ruler", "define_ruler", "add_queen_personality", "remove_queen_personality", "set_consort_flag", "clr_consort_flag", "set_consort_culture", "set_consort_religion", "change_consort_regent_to_ruler", "remove_consort", "define_consort", "exile_consort_as", "set_consort", "change_heir_adm", "change_heir_dip", "change_heir_mil", "add_heir_personality", "remove_heir_personality", "set_heir_flag", "clr_heir_flag", "add_heir_claim", "add_heir_support", "set_heir_culture", "set_heir_religion", "exile_heir_as", "set_heir", "kill_heir", "remove_heir", "convert_female_heir_to_general", "convert_heir_to_general", "define_heir_to_general", "define_heir", "add_imperial_influence", "add_scaled_imperial_influence", "elector", "revoke_reform", "set_in_empire", "hre_inheritable", "imperial_ban_allowed", "internal_hre_cb", "set_allow_female_emperor", "dismantle_hre", "enable_hre_leagues", "set_hre_religion", "set_hre_heretic_religion", "set_hre_religion_locked", "set_hre_religion_treaty", "join_league", "leave_league", "dismantle_empire_of_china", "set_emperor_of_china", "add_mandate", "set_mandate", "set_revolution_target", "set_emperor", "set_ai_personality", "set_ai_attitude", "manpower", "estate", "discover_province", "change_government", "lose_reforms"
      ];

      private static readonly HashSet<string> ProvinceEffects = 
      [
         "province_event", "add_province_modifier", "add_permanent_province_modifier", "extend_province_modifier", "remove_province_modifier", "add_province_triggered_modifier", "remove_province_triggered_modifier", "set_province_flag", "clr_province_flag", "change_province_name", "rename_capital", "remove_revolution_from_province_effect", "change_tribal_land", "add_base_tax", "add_base_production", "add_base_manpower", "add_prosperity", "add_devastation", "add_local_autonomy", "set_local_autonomy", "change_trade_goods", "add_scaled_local_adm_power", "add_scaled_local_dip_power", "add_scaled_local_mil_power", "cancel_construction", "add_great_project", "move_great_project", "destroy_great_project", "add_great_project_tier", "add_construction_progress", "add_building", "remove_building", "add_building_construction", "set_base_tax", "set_base_production", "set_base_manpower", "add_random_development", "create_advisor", "set_seat_in_parliament", "back_current_issue", "change_culture", "change_original_culture", "add_culture_construction", "change_religion", "change_to_secondary_religion", "send_missionary", "add_cardinal", "remove_cardinal", "add_reform_center", "remove_reform_center", "set_in_empire", "add_institution_embracement", "reset_golden_age", "extend_golden_age", "add_nationalism", "add_unrest", "create_native", "create_pirate", "create_revolt", "spawn_rebels", "create_colony", "add_colonysize", "multiply_colonysize", "add_siberian_construction", "change_native_ferocity", "change_native_hostileness", "change_native_size", "discover_country", "undiscover_country", "native_policy", "add_claim", "add_core", "add_core_construction", "add_permanent_claim", "add_territorial_core", "cede_province", "change_controller", "decolonize", "remove_claim", "remove_core", "remove_territorial_core", "artillery", "revolutionary_guard_artillery", "cavalry", "mercenary_cavalry", "cossack_cavalry", "hussars_cavalry", "revolutionary_guard_cavalry", "banner_cavalry", "qizilbash_cavalry", "mamluks_cavalry", "infantry", "mercenary_infantry", "streltsy_infantry", "rajput_infantry", "cawa_infantry", "carolean_infantry", "marine_infantry", "janissary_infantry", "revolutionary_guard_infantry", "tercio_infantry", "musketeer_infantry", "samurai_infantry", "cossack_infantry", "banner_infantry", "qizilbash_infantry", "mamluks_infantry", "heavy_ship", "man_of_war_heavy", "galleon_heavy", "create_flagship", "light_ship", "man_of_war_light", "caravel_light", "voc_indiamen_light", "galley", "geobukseon_galley", "galleass_galley", "transport", "kill_leader", "kill_units", "add_unit_construction", "build_to_forcelimit", "remove_loot", "change_siege", "clear_rebels", "recall_merchant", "add_trade_node_income", "add_trade_modifier", "remove_trade_modifier", "add_to_trade_company", "add_trade_company_investment", "center_of_trade", "add_center_of_trade_level"
      ];
   }
}