using System.Diagnostics;
using System.Text;
using Editor.DataClasses.GameDataClasses;
using Editor.Helper;
using Parsing = Editor.Parser.Parsing;

namespace Editor.Loading
{
   [Loading]
   public static class UnitTypeLoading
   {
      public static void Load()
      {
         var sw = Stopwatch.StartNew();
         var files = FilesHelper.GetFilesFromModAndVanillaUniquely("*.txt", "common", "units");

         foreach (var file in files)
         {
            Parsing.RemoveCommentFromMultilineString(IO.ReadAllInUTF8(file), out var content);
            LoadUnitType(file);
         }

         sw.Stop();
         Globals.LoadingLog.WriteTimeStamp("Unit types", sw.ElapsedMilliseconds);
         PrintAllUnitsToFile();
      }

      private static void LoadUnitType(string path)
      {
         Parsing.RemoveCommentFromMultilineString(IO.ReadAllInUTF8(path), out var content);
         var kvps = Parsing.GetKeyValueList(ref content);

         var fileName = Path.GetFileNameWithoutExtension(path);
         var unit = Unit.Empty;
         
         foreach (var kvp in kvps)
         {
            if (kvp.Key != "type")
               continue;

            if (!Enum.TryParse<UnitType>(kvp.Value, true, out var type))
            {
               Globals.ErrorLog.Write($"Error parsing unit type in {fileName}.");
               return;
            }

            switch (type)
            {
               case UnitType.Infantry:
               case UnitType.Cavalry:
               case UnitType.Artillery:
                  unit = new LandUnit(fileName, type);
                  break;
               case UnitType.Light_Ship:
               case UnitType.Heavy_Ship:
               case UnitType.Galley:
               case UnitType.Transport:
                  unit = new ShipUnit(fileName, type);
                  break;
            }
            break;
         }

         if (unit is ShipUnit)
            AddParamsToShipUnit((ShipUnit)unit, kvps);
         else
            AddParamsToLandUnit((LandUnit)unit, kvps);

         Globals.Units.Add(unit);
      }

      private static void AddParamsToShipUnit(ShipUnit unit, List<KeyValuePair<string, string>> param)
      {
         foreach (var kvp in param)
         {
            switch (kvp.Key)
            {
               case "hull_size":
                  if (!int.TryParse(kvp.Value, out var hullSize))
                  {
                     Globals.ErrorLog.Write($"Error parsing hull size in {unit.UnitName}.");
                     return;
                  }
                  unit.HullSize = hullSize;
                  break;
               case "base_cannons":
                  if (!int.TryParse(kvp.Value, out var baseCannons))
                  {
                     Globals.ErrorLog.Write($"Error parsing base cannons in {unit.UnitName}.");
                     return;
                  }
                  unit.BaseCannons = baseCannons;
                  break;
               case "blockade":
                  if (!int.TryParse(kvp.Value, out var blockade))
                  {
                     Globals.ErrorLog.Write($"Error parsing blockade in {unit.UnitName}.");
                     return;
                  }
                  unit.Blockade = blockade;
                  break;
               case "sail_speed":
                  if (!float.TryParse(kvp.Value, out var sailSpeed))
                  {
                     Globals.ErrorLog.Write($"Error parsing sail speed in {unit.UnitName}.");
                     return;
                  }
                  unit.SailSpeed = sailSpeed;
                  break;
               case "sailors":
                  if (!int.TryParse(kvp.Value, out var sailors))
                  {
                     Globals.ErrorLog.Write($"Error parsing sailors in {unit.UnitName}.");
                     return;
                  }
                  unit.Sailors = sailors;
                  break;
               case "sprite_level":
                  if (!int.TryParse(kvp.Value, out var spriteLevel))
                  {
                     Globals.ErrorLog.Write($"Error parsing sprite level in {unit.UnitName}.");
                     return;
                  }
                  unit.SpriteLevel = spriteLevel;
                  break;
               case "trade_power":
                  if (!float.TryParse(kvp.Value, out var tradePower))
                  {
                     Globals.ErrorLog.Write($"Error parsing trade power in {unit.UnitName}.");
                     return;
                  }
                  unit.TradePower = tradePower;
                  break;
               case "type":
                  break;
               default:
                  Globals.ErrorLog.Write($"Error parsing {kvp.Key} in {unit.UnitName}.");
                  break;
            }
         }

      }
      
      private static void AddParamsToLandUnit(LandUnit unit, List<KeyValuePair<string, string>> param)
      {
         foreach (var kvp in param)
         {
            switch (kvp.Key)
            {
               case "type":
                  break;
               case "maneuver":
                  if (!int.TryParse(kvp.Value, out var maneuver))
                  {
                     Globals.ErrorLog.Write($"Error parsing maneuver in {unit.UnitName}.");
                     return;
                  }
                  unit.Maneuver = maneuver;
                  break;
               case "offensive_morale":
                  if (!int.TryParse(kvp.Value, out var offensiveMorale))
                  {
                     Globals.ErrorLog.Write($"Error parsing offensive morale in {unit.UnitName}.");
                     return;
                  }
                  unit.OffensiveMorale = offensiveMorale;
                  break;
               case "defensive_morale":
                  if (!int.TryParse(kvp.Value, out var defensiveMorale))
                  {
                     Globals.ErrorLog.Write($"Error parsing defensive morale in {unit.UnitName}.");
                     return;
                  }
                  unit.DefensiveMorale = defensiveMorale;
                  break;
               case "offensive_fire":
                  if (!int.TryParse(kvp.Value, out var offensiveFire))
                  {
                     Globals.ErrorLog.Write($"Error parsing offensive fire in {unit.UnitName}.");
                     return;
                  }
                  unit.OffensiveFire = offensiveFire;
                  break;
               case "defensive_fire":
                  if (!int.TryParse(kvp.Value, out var defensiveFire))
                  {
                     Globals.ErrorLog.Write($"Error parsing defensive fire in {unit.UnitName}.");
                     return;
                  }
                  unit.DefensiveFire = defensiveFire;
                  break;
               case "offensive_shock":
                  if (!int.TryParse(kvp.Value, out var offensiveShock))
                  {
                     Globals.ErrorLog.Write($"Error parsing offensive shock in {unit.UnitName}.");
                     return;
                  }
                  unit.OffensiveShock = offensiveShock;
                  break;
               case "defensive_shock":
                  if (!int.TryParse(kvp.Value, out var defensiveShock))
                  {
                     Globals.ErrorLog.Write($"Error parsing defensive shock in {unit.UnitName}.");
                     return;
                  }
                  unit.DefensiveShock = defensiveShock;
                  break;
               case "unit_type":
                  if (!Globals.TechnologyGroups.TryGetValue(kvp.Value, out var technologyGroup))
                  {
                     Globals.ErrorLog.Write($"Error parsing technology group in {unit.UnitName}.");
                     return;
                  }
                  unit.TechnologyGroup = technologyGroup;
                  break;
               default:
                  Globals.ErrorLog.Write($"Error parsing {kvp.Key} in {unit.UnitName}.");
                  break;
            }
         }
      }

      private static void PrintAllUnitsToFile()
      {
         var sb = new StringBuilder();
         foreach (var unit in Globals.Units)
         {
            sb.AppendLine(unit.ToString());
         }

         //get the download folder of the current user
         var downloadFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Downloads";
         var path = Path.Combine(downloadFolder, "AllUnits.txt");
         File.WriteAllText(path, sb.ToString());
      }
   }
}