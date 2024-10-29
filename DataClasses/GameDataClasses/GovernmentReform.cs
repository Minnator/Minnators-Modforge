using Editor.ParadoxLanguage.Trigger;

namespace Editor.DataClasses.GameDataClasses
{
   public class GovernmentReform(string name)
   {
      public string Name { get; } = name;
      public string Icon { get; set; } = string.Empty;
      public static GovernmentReform Empty => new (string.Empty);

      public List<Trigger> Potential = [];
      public List<Trigger> Trigger = [];
      public List<KeyValuePair<string, string>> Attributes = [];
      public List<KeyValuePair<string, string>> CustomAttributes = [];
      public List<Modifier> Modifiers = [];
      public List<Effect> Effects = [];
      public List<Effect> RemoveEffects = [];
      public List<Conditional> Conditionals = [];

      public override string ToString()
      {
         return $"{Name}: ({Modifiers.Count} modifiers)";
      }

      public override bool Equals(object? obj)
      {
         if (obj is GovernmentReform other)
            return Name == other.Name;
         return false;
      }

      public override int GetHashCode()
      {
         return Name.GetHashCode();
      }
   }

   public struct Conditional
   {
      public List<Trigger> Condition = [];
      public List<KeyValuePair<string, string>> Attributes = [];
      public List<string> Abilities = [];

      public Conditional()
      {

      }
   }
}