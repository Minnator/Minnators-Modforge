using Editor.Helper;

namespace Editor.DataClasses.GameDataClasses
{
   public abstract class Unit
   {
      public string UnitName { get; set; } = string.Empty;
      public UnitType UnitType { get; set; }

      public static Unit Empty => LandUnit.Empty;
   }

   public enum UnitType
   {
      Infantry,
      Cavalry,
      Artillery,
      Light_Ship,
      Heavy_Ship,
      Galley,
      Transport,
   }
   public class LandUnit : Unit
   {
      public TechnologyGroup TechnologyGroup { get; set; } = TechnologyGroup.Empty;
      public UnitType Type { get; set; }
      public float Maneuver = 0;
      public float OffensiveMorale = 0;
      public float DefensiveMorale = 0;
      public float OffensiveFire = 0;
      public float DefensiveFire = 0;
      public float OffensiveShock = 0;
      public float DefensiveShock = 0;

      public LandUnit(string unitTypeName, UnitType type)
      {
         UnitName = unitTypeName;
         Type = type;
      }
      public new static LandUnit Empty => new(string.Empty, UnitType.Infantry);

      public override int GetHashCode()
      {
         return Maneuver.GetHashCode() ^ OffensiveMorale.GetHashCode() ^ DefensiveMorale.GetHashCode() ^ OffensiveFire.GetHashCode() ^ DefensiveFire.GetHashCode() ^ OffensiveShock.GetHashCode() ^ DefensiveShock.GetHashCode();
      }

      public override bool Equals(object? obj)
      {
         if (obj is not LandUnit unit)
            return false;
         return unit.Maneuver == Maneuver && unit.OffensiveMorale == OffensiveMorale && unit.DefensiveMorale == DefensiveMorale && unit.OffensiveFire == OffensiveFire && unit.DefensiveFire == DefensiveFire && unit.OffensiveShock == OffensiveShock && unit.DefensiveShock == DefensiveShock;
      }
      public override string ToString()
      {
         return $"{UnitName} ({Type})";
      }

      public static List<string> AutoSelectFuncUnits(int max)
      {
         if (Selection.SelectedCountry == Country.Empty) 
            return [];

         List<LandUnit> landUnits = [];

         foreach (var unit in Globals.Units)
            if (unit is LandUnit landUnit)
               if (Equals(landUnit.TechnologyGroup, Selection.SelectedCountry.TechnologyGroup))
                  landUnits.Add(landUnit);

         if (max <= landUnits.Count && max != -1)
            return landUnits.Select(x => x.UnitName).ToList();

         List<string> selectedUnits = [];
         if (max == -1) max = Globals.Settings.Misc.AutoPropertiesCountBig;
         for (var index = 0; index < max; index++)
         {
            var randomUnit = landUnits[Globals.Random.Next(landUnits.Count)];
            selectedUnits.Add(randomUnit.UnitName);
            landUnits.Remove(randomUnit);
         }

         return selectedUnits;
      }
   }

   public class ShipUnit : Unit
   {
      public int HullSize { get; set; } = 0;
      public int BaseCannons { get; set; } = 0;
      public int Blockade { get; set; } = 0;
      public float SailSpeed { get; set; } = 0;
      public int Sailors { get; set; } = 0;
      public int SpriteLevel { get; set; } = 0;
      public float TradePower { get; set; } = 0f;

      public ShipUnit(string unitTypeName, UnitType type)
      {
         UnitName = unitTypeName;
         UnitType = type;
      }

      public new static ShipUnit Empty => new(string.Empty, UnitType.Light_Ship);

      public override int GetHashCode()
      {
         return HullSize.GetHashCode() ^ BaseCannons.GetHashCode() ^ Blockade.GetHashCode() ^ SailSpeed.GetHashCode() ^ Sailors.GetHashCode() ^ SpriteLevel.GetHashCode() ^ TradePower.GetHashCode();
      }

      public override bool Equals(object? obj)
      {
         if (obj is not ShipUnit unit)
            return false;
         return unit.HullSize == HullSize && unit.BaseCannons == BaseCannons && unit.Blockade == Blockade && unit.SailSpeed == SailSpeed && unit.Sailors == Sailors && unit.SpriteLevel == SpriteLevel && TradePower == unit.TradePower;
      }

      public override string ToString()
      {
         return $"{UnitName} ({UnitType})";
      }
   }
}