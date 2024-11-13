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

      public static List<string> GetAutoAssignment(int num)
      {
         List<IdeaGroup> ideaGroups = [];
         List<string> result = [];
         foreach (var idea in Globals.Ideas)
            if (idea.Type == IdeaType.IdeaGroup)
               ideaGroups.Add((IdeaGroup)idea);

         if (ideaGroups.Count == 0)
            return result;
         if (num > ideaGroups.Count)
            return ideaGroups.Select(x => x.Name).ToList();

         var numPerCategory = num / 3;
         var admin = ideaGroups.Where(x => x.Category == Mana.ADM).ToList();
         var diplo = ideaGroups.Where(x => x.Category == Mana.DIP).ToList();
         var military = ideaGroups.Where(x => x.Category == Mana.MIL).ToList();

         // assign numPerCategory per category
         for (var i = 0; i < numPerCategory; i++)
         {
            var index = Globals.Random.Next(military.Count);
            result.Add(military[index].Name);
            military.RemoveAt(index);
            ideaGroups.Remove(military[index]);

            index = Globals.Random.Next(admin.Count);
            result.Add(admin[index].Name);
            admin.RemoveAt(index);
            ideaGroups.Remove(admin[index]);

            index = Globals.Random.Next(diplo.Count);
            result.Add(diplo[index].Name);
            diplo.RemoveAt(index);
            ideaGroups.Remove(diplo[index]);
         }

         for (var i = numPerCategory * 3; i < num; i++)
         {
            var index = Globals.Random.Next(ideaGroups.Count);
            result.Add(ideaGroups[index].Name);
            ideaGroups.RemoveAt(index);
         }

         return result;
      }
   }

   public class NationalIdea : Idea
   {
      public override IdeaType Type { get; set; } = IdeaType.NationalIdea;
      public bool Free { get; set; }
      public List<Modifier> Start { get; set; } = [];
   }
}