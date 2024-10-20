namespace Editor.DataClasses.GameDataClasses
{
   public class Unit
   {
      public enum UnitType
      {
         Infantry,
         Cavalry,
         Artillery,
      }

      public string UnitTypeName { get; set; } = string.Empty;
      public UnitType Type { get; set; } = UnitType.Infantry;
      public float Maneuver = 0;
      public float OffensiveMorale = 0;
      public float DefensiveMorale = 0;
      public float OffensiveFire = 0;
      public float DefensiveFire = 0;
      public float OffensiveShock = 0;
      public float DefensiveShock = 0;

      public static Unit Empty => new();

      public override int GetHashCode()
      {
         return Maneuver.GetHashCode() ^ OffensiveMorale.GetHashCode() ^ DefensiveMorale.GetHashCode() ^ OffensiveFire.GetHashCode() ^ DefensiveFire.GetHashCode() ^ OffensiveShock.GetHashCode() ^ DefensiveShock.GetHashCode();
      }

      public override bool Equals(object? obj)
      {
         if (obj is not Unit unit)
            return false;
         return unit.Maneuver == Maneuver && unit.OffensiveMorale == OffensiveMorale && unit.DefensiveMorale == DefensiveMorale && unit.OffensiveFire == OffensiveFire && unit.DefensiveFire == DefensiveFire && unit.OffensiveShock == OffensiveShock && unit.DefensiveShock == DefensiveShock;
      }

      public override string ToString()
      {
         return $"{UnitTypeName} ({Type})";
      }

      public static bool operator ==(Unit a, Unit b)
      {
         return a.Maneuver == b.Maneuver && a.OffensiveMorale == b.OffensiveMorale && a.DefensiveMorale == b.DefensiveMorale && a.OffensiveFire == b.OffensiveFire && a.DefensiveFire == b.DefensiveFire && a.OffensiveShock == b.OffensiveShock && a.DefensiveShock == b.DefensiveShock;
      }

      public static bool operator !=(Unit a, Unit b)
      {
         return a.Maneuver != b.Maneuver || a.OffensiveMorale != b.OffensiveMorale || a.DefensiveMorale != b.DefensiveMorale || a.OffensiveFire != b.OffensiveFire || a.DefensiveFire != b.DefensiveFire || a.OffensiveShock != b.OffensiveShock || a.DefensiveShock != b.DefensiveShock;
      }
   }
}