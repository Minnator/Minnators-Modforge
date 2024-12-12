using Editor.DataClasses.GameDataClasses;
using Editor.Helper;
using Editor.Parser;
using Parsing = Editor.Parser.Parsing;

namespace Editor.Loading
{
   
   public static class IdeasLoading
   {
      /// <summary>
      /// Ideas are NOT saveable.
      /// They will lateron be
      /// </summary>
      public static void Load()
      {
         var files = FilesHelper.GetFilesFromModAndVanillaUniquely("*.txt", "common", "ideas");
         if (files.Count == 0)
            return;

         List<Idea> ideas = [];

         foreach (var file in files)
         {
            ParseIdeas(file, ref ideas);
         }

         Globals.Ideas = ideas;
      }

      /// <summary>
      /// Parses all ideas from the given file but does them NOT add to the SAVEABLE pipeline.
      /// </summary>
      /// <param name="filePath"></param>
      /// <param name="ideas"></param>
      private static void ParseIdeas(string filePath, ref List<Idea> ideas)
      {
         Parsing.RemoveCommentFromMultilineString(IO.ReadAllInUTF8(filePath), out var content);
         var elements = Parsing.GetElements(0, ref content);

         foreach (var element in elements)
         {
            if (element is not Block ideaBlock)
            {
               Globals.ErrorLog.Write($"Illegal content found in \"{filePath}\": {element}");
               continue;
            }

            var name = ideaBlock.Name;
            List<KeyValuePair<string, List<Modifier>>> ideaModifiers = [];
            List<Modifier> bonus = [];
            List<Modifier> start = [];
            var triggerString = string.Empty;
            var free = false;
            var important = false;
            var category = Mana.NONE;

            foreach (var block in ideaBlock.GetBlockElements)
            {
               switch (block.Name)
               {
                  case "trigger":
                     triggerString = block.GetContent;
                     break;
                  case "ai_will_do":
                     break;
                  case "bonus":
                     if (!ModifierParser.ParseModifiers(block.GetContent, out var bonusModifiers))
                     {
                        Globals.ErrorLog.Write($"Failed to parse bonus modifiers in idea \"{name}\"");
                        continue;
                     }
                     bonus.AddRange(bonusModifiers);
                     break;
                  case "start":
                     if (!ModifierParser.ParseModifiers(block.GetContent, out var startModifiers))
                     {
                        Globals.ErrorLog.Write($"Failed to parse start modifiers in idea \"{name}\"");
                        continue;
                     }
                     start.AddRange(startModifiers);
                     break;
                  default:
                     if (!ModifierParser.ParseModifiers(block.GetContent, out var ideaMod))
                     {
                        Globals.ErrorLog.Write($"Failed to parse modifiers in idea \"{block.Name}\"");
                        continue;
                     }
                     ideaModifiers.Add(new (block.Name, ideaMod));
                     break;
               }
            }

            var ideaKvps = Parsing.GetKeyValueList(ideaBlock.GetContent);
            foreach (var (key, value) in ideaKvps)
            {
               switch (key)
               {
                  case "free":
                     free = value.Equals("yes", StringComparison.CurrentCultureIgnoreCase);
                     break;
                  case "category":
                     if (!Enum.TryParse<Mana>(value, true, out var manaType))
                     {
                        Globals.ErrorLog.Write($"Unknown mana type \"{value}\" in file \"{filePath}\"");
                        continue;
                     }
                     category = manaType;
                     break;
                  case "important":
                     important = value.Equals("yes", StringComparison.CurrentCultureIgnoreCase);
                     break;
                  default:
                     Globals.ErrorLog.Write($"Unknown key \"{key}\" in file \"{filePath}\"");
                     break;
               }
            }

            if (free)
            {
               ideas.Add(new NationalIdea
               {
                  Name = name,
                  Ideas = ideaModifiers,
                  Bonus = bonus,
                  TriggerString = triggerString,
                  Free = free,
                  Start = start,
                  Type = IdeaType.NationalIdea
               });
            }
            else
            {
               ideas.Add(new IdeaGroup
               {
                  Name = name,
                  Ideas = ideaModifiers,
                  Bonus = bonus,
                  TriggerString = triggerString,
                  Category = category,
                  Important = important,
                  Type = IdeaType.IdeaGroup
               });
            }

         }
      }

   }
}