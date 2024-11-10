namespace Editor.DataClasses.GameDataClasses
{
   public enum IdeaType
   {
      NationalIdea,
      IdeaGroup
   }

   public abstract class Idea
   {
      public string Name { get; set; } = string.Empty;
      public abstract IdeaType Type { get; set; } 
      public List<KeyValuePair<string, List<Modifier>>> Ideas { get; set; } = [];
      public List<Modifier> Bonus { get; set; } = [];
      public string TriggerString { get; set; } = string.Empty;
   }

   public class IdeaGroup : Idea
   {
      public Mana Category { get; set; } = Mana.NONE;
      public override IdeaType Type { get; set; } = IdeaType.IdeaGroup;
      public bool Important { get; set; }
   }

   public class NationalIdea : Idea
   {
      public override IdeaType Type { get; set; } = IdeaType.NationalIdea;
      public bool Free { get; set; }
      public List<Modifier> Start { get; set; } = [];
   }
}