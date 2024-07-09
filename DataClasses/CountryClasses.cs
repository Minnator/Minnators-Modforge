namespace Editor.DataClasses
{
   public struct MonarchName (string name, int ordinalNumber, int chance)
   {
      public string Name { get; } = name;
      public int OrdinalNumber { get; } = ordinalNumber;
      public int Chance { get; } = chance;

      // Chances for female names are negative
      public readonly bool IsFemale => Chance < 0;
   }

}