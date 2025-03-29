using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using Editor.DataClasses.GameDataClasses;
using Editor.Helper;
using Editor.Loading.Enhanced.PCFL.Implementation;
using Editor.Saving;
using Windows.Devices.Power;
using Editor.ErrorHandling;
using Editor.Parser;
using Windows.UI.Popups;

namespace Editor.Loading.Enhanced
{
   public class ScriptedEffectUsage : IToken
   {
      private IToken[] _tokens;
      public List<LineKvp<string, string>> ReplacementValues { get; init; }
      public string Name { get; init; }

      public void Activate(ITarget target)
      {
         foreach (var token in _tokens)
         {
            token.Activate(target);
         }
      }

      public void GetTokenString(int tabs, ref StringBuilder sb)
      {
         // Add effect: effect_name = yes
         if (ReplacementValues.Count == 0)
            SavingUtil.AddBool(tabs, true, Name, ref sb);
         else
         {
            // Add effect: effect_name = {
            //    key = value
            //    ...
            // }
            SavingUtil.OpenBlock(ref tabs, Name, ref sb);
            foreach (var kvp in ReplacementValues) 
               SavingUtil.AddLineKvp(tabs, kvp, ref sb);
            SavingUtil.CloseBlock(ref tabs, ref sb);
         }
      }

      public string GetTokenName() => Name;

      public string GetTokenDescription()
      {
         return $"Call of scripted effect \"{Name}\"";
      }

      public string GetTokenExample()
      {
         return "effect = yes \nor\neffect = { \n\t<replaceKey> = value \n\t... \n}";
      }
   }

   public partial class ScriptedEffectImpl(EnhancedBlock block, ref PathObj po)
   {
      public string name = block.Name;
      public Dictionary<string, int[]> replacePos;
      public string effect; // with all references $...$ removed

      public EnhancedBlock Block = block;
      public PathObj Po = po;


      // Save the string
      // With possibility of replacement => easier in string or stringbuilder

      // remove all references $...$
      // name => pos in string
      // insert at pos
      // count length of inserted string
      // insert next at pos + count

      public IToken? CreateEffect(EnhancedBlock? block, LineKvp<string, string>? kvp, ParsingContext context, PathObj po)
      {
         if (block is not null)
         {
            // We have a block call which indicates that there has to be at least one argument in the blocks content elements
            if (block.Count == 0)
            {
               _ = new LoadingError(po, $"Scripted effect '{name}' requires at least one argument in block call.");
               return null;
            }

            var kvps = block.GetContentLines(po);
            if (ReplaceValues(kvps, po, out var replaced))
            {
               return new ScriptedEffectUsage
               {
                  // TODO parse 'replaced' to tokens
                  Name = name,
                  ReplacementValues = kvps
               };
            }
         }
         else
         {
            // we have a kvp call which indicates that the value has to be 'yes' as no is not allowed
            if (kvp is null || kvp.Value.Value.ToLower().Equals("yes"))
            {
               _ = new LoadingError(po, $"Scripted effect '{name}' requires 'yes' as value.");
               return null;
            }

            return new ScriptedEffectUsage
            {
               // TODO parse 'replaced' to tokens
               Name = name,
               ReplacementValues = []
            };
         }

         // in case of block check all replace values
         // in case of kvp check if value == yes and no replace values 
         // Returns list of parsed tokens of scripted Effect as ScriptedEffectUsage
         // Throw error if parsing fails due to wrong context

         return new ScriptedEffectUsage();
      }

      public static bool ParseScriptedEffectDefinition(List<string> files, out List<string> result)
      {
         // Get all scripted effects from the files, now that we have the names of them we can resolve them if they contain each other
         List<ScriptedEffectImpl> scriptEffects = [];
         //var files = PathManager.GetAllFilesInFolder(internalPath: ["common", "scripted_effects"]);

         foreach (var file in files)
         {
            var po = PathObj.FromPath(file);
            var (blocks, _) = po.LoadBase(EnhancedParser.FileContentAllowed.BlocksOnly);

            foreach (var block in blocks)
            {
               var scrEff = new ScriptedEffectImpl(block, ref po);
               scriptEffects.Add(scrEff);
            }
         }
         return ToplologicalSorting(scriptEffects, out result);
         //return scriptEffects;
      }

      private static bool ToplologicalSorting(List<ScriptedEffectImpl> scriptEffects, out List<string> result)
      {
         HashSet<string> effectDict = new(scriptEffects.Select(x => x.name));
         Dictionary<string, List<string>> dependencyDict = new();

         foreach (var scrEff in scriptEffects)
         {
            Stack<IEnhancedElement> stack = new (scrEff.Block.GetElements());
            List<string> dependencies = [];

            while(stack.Count > 0)
            {
               var element = stack.Pop();
               if (element is EnhancedBlock block)
               {
                  if (!effectDict.TryGetValue(block.Name, out var eff))
                     continue;
                  // found scripted effect reference
                  dependencies.Add(block.Name);
                  if (block.Count > 0)
                     foreach (var bE in block.GetElements())
                        stack.Push(bE);
               }
               else
               {
                  foreach (var (key, value, _) in ((EnhancedContent)element).GetLineKvpEnumerator(scrEff.Po, false))
                     if (effectDict.TryGetValue(key, out _))
                     {
                        dependencies.Add(key);
                        if (!value.ToLower().Equals("yes"))
                           _ = new LoadingError(scrEff.Po, $"Scripted effect '{scrEff.name}' requires 'yes' as value.");
                     }
               }
            }

            dependencyDict.Add(scrEff.name, dependencies);
         }

         if (!Sorting.TopologicalSortWithCycle(dependencyDict, out result, true))
         {
            _ = new LoadingError(PathObj.Empty, $"Cyclic dependency detected in scripted effects: {string.Join(", ", result)}");
            return false;
         }

         return true;
      }

      private static void ResolveEffectReferences(List<ScriptedEffect> scriptedEffects)
      {
         // lookup table for the scripted effects
         var effectDict = scriptedEffects.ToDictionary(x => x.Name, x => x);
         HashSet<string> visited = [];

         foreach (var scriptedEffect in scriptedEffects)
         {
            visited.Add(scriptedEffect.Name);
            var effElements = new List<IEnhancedElement>();
            foreach (var element in scriptedEffect.EnhancedElements)
               effElements.AddRange(ResolveRecursive(element, visited, effectDict, scriptedEffect.Po));
            scriptedEffect.EnhancedElements = effElements;
            visited.Remove(scriptedEffect.Name);
         }
      }

      private static List<IEnhancedElement> ResolveRecursive(IEnhancedElement element, HashSet<string> visited, Dictionary<string, ScriptedEffect> validScriptedEffects, PathObj po)
      {
         if (element is EnhancedBlock block)
         {
            // no scripted Effect but a block we need to resolve
            if (!validScriptedEffects.TryGetValue(block.Name, out var scriptedEffect))
            {
               List<IEnhancedElement> blockContent = [];
               foreach (var bE in block.GetElements())
                  blockContent.AddRange(ResolveRecursive(bE, visited, validScriptedEffects, po));
               return blockContent;
            }
            else
            {
               if (!visited.Add(block.Name))
                  throw new($"Cyclic dependency detected in scripted effect {block.Name}");
               var effs = scriptedEffect.EnhancedElements;
               return effs;
            }
         }
         else
         {
            var sb = new StringBuilder();
            var lineNum = -1;
            List<IEnhancedElement> effs = [];
            foreach (var line in ((EnhancedContent)element).GetLineKvpEnumerator(po, false))
            {
               lineNum = line.Line;

               if (!validScriptedEffects.TryGetValue(line.Key, out var scriptedEffect))
               {
                  sb.Append($"{line.Key} = {line.Value}");
               }
               else
               {
                  if (!visited.Add(line.Key))
                     throw new($"Cyclic dependency detected in scripted effect {line.Key}");
                  visited.Add(line.Key);
                  effs.Add(new EnhancedContent(sb.ToString(), lineNum, -1));
                  sb.Clear();
                  foreach (var eff in scriptedEffect.EnhancedElements)
                     effs.AddRange(ResolveRecursive(eff, visited, validScriptedEffects, scriptedEffect.Po));
                  visited.Remove(line.Key);
               }
            }

            if (sb.Length > 0)
               effs.Add(new EnhancedContent(sb.ToString(), lineNum, -1));
            return effs;
         }
      }


      // Contains the key and the index to the replacement value in a backwards order
      private List<(string key, int index)> _replaceSources = [];

      // TODO fix
      private void SetReplaceSources(List<LineKvp<string, string>> values, PathObj po)
      {
         var expectedValues = new string[values.Count];

         for (var i = 0; i < values.Count; i++)
            expectedValues[i] = values[i].Key;

         var matches = ReplaceRegex().Matches(effect);

         for (var i = matches.Count - 1; i >= 0; i--)
         {
            var match = matches[i];
            if (expectedValues.Contains(match.Value))
            {
               // the value is one of the expected values
               // we trim the $ to be able to use .Equals() in later usage
               _replaceSources.Add((match.Value[1..^1], match.Index));
            }
            else
            {
               _ = new LoadingError(po, $"Unknown argument '{match.Value}' detected in scripted effect '{name}' call.");
            }
         }
      }

      public bool ReplaceValues(List<LineKvp<string, string>> values, PathObj po, out string replaced)
      {
         Debug.Assert(values.Count != 0, "This should never be called for non block scripted effects!");
         
         var sb = new StringBuilder(effect);
         var returnVal = true;

         // The list is backwards so we can go forward and just replace the values
         for (var i = 0; i < _replaceSources.Count; i++)
         {
            var (key, index) = _replaceSources[i];
            var replacement = values.FirstOrDefault(x => x.Key.Equals(key));
            if (replacement.Key == null)
            {
               _ = new LoadingError(po, $"Missing argument in scripted effect ({name}) call: '{key}'");
               returnVal = false;
               continue;
            }
            sb.Remove(index, key.Length).Insert(index, replacement.Value);
         }

         replaced = sb.ToString();
         return returnVal;
      }

      [GeneratedRegex(@"\$\w+\$")]
      private static partial Regex ReplaceRegex();
   }
}