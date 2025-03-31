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
      public IToken[] Tokens { get; init; }
      public List<LineKvp<string, string>> ReplacementValues { get; init; }
      public string Name { get; init; }

      public void Activate(ITarget target)
      {
         foreach (var token in Tokens)
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

   public partial class ScriptedEffectImpl
   {
      public string name;
      public Dictionary<string, int[]> ReplacePos;
      public string effect; // with all references $...$ removed

      // Contains the key and the index to the replacement value in a backwards order
      public List<(string key, int index)> _replaceSources = [];

      public EnhancedBlock Block;
      public PathObj Po;

      public ScriptedEffectImpl(EnhancedBlock block, ref PathObj po)
      {
         name = block.Name;
         Block = block;
         Po = po;
         effect = block.GetContent();
      }

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
            if (ReplaceValues(kvps, out var replaced))
            {
               return new ScriptedEffectUsage
               {
                  Tokens = GeneralFileParser.ParseElementsToTokens(EnhancedParser.LoadBaseOrder(replaced, po), context, po).ToArray(),
                  Name = name,
                  ReplacementValues = kvps
               };
            }
            return null;
         }
         else
         {
            // we have a kvp call which indicates that the value has to be 'yes' as no is not allowed
            if (kvp is null || !kvp.Value.Value.ToLower().Equals("yes"))
            {
               _ = new LoadingError(po, $"Scripted effect '{name}' requires 'yes' as value.");
               return null;
            }

            return new ScriptedEffectUsage
            {
               Tokens = GeneralFileParser.ParseElementsToTokens(EnhancedParser.LoadBaseOrder(effect, po), context, po).ToArray(),
               Name = name,
               ReplacementValues = []
            };
         }

         // in case of block check all replace values
         // in case of kvp check if value == yes and no replace values 
         // Returns list of parsed tokens of scripted Effect as ScriptedEffectUsage
         // Throw error if parsing fails due to wrong context
      }

      public static bool ParseScriptedEffectDefinition(List<string> files, out List<string> result)
      {
         // Get all scripted effects from the files, now that we have the names of them we can resolve them if they contain each other
         HashSet<ScriptedEffectImpl> scriptEffects = [];
         //var files = PathManager.GetAllFilesInFolder(internalPath: ["common", "scripted_effects"]);

         foreach (var file in files)
         {
            var po = PathObj.FromPath(file);
            var (blocks, _) = po.LoadBase(EnhancedParser.FileContentAllowed.BlocksOnly);

            foreach (var block in blocks)
            {
               if (!Scopes.IsEffectKeyUnused(block.Name))
               {
                  _ = new ErrorObject(ErrorType.DuplicateObjectDefinition,
                                      $"'{block.Name}' is not a valid scripted effect name. It is reserved for vanilla effects!", LogType.Critical);
                  continue;
               }
               if (Scopes.ScriptedEffects.ContainsKey(block.Name))
               {
                  _ = new ErrorObject(ErrorType.DuplicateObjectDefinition,
                                      $"'{block.Name}' is already used as a scripted effect name.", LogType.Critical);
                  continue;
               }
               var scrEff = new ScriptedEffectImpl(block, ref po);
               if (!scriptEffects.Add(scrEff))
                  _ = new LoadingError(po, $"Scripted effect '{scrEff.name}' is defined multiple times.");
            }
         }
         if (!TopologicalSorting(scriptEffects, out result))
            return false;

         ReplaceReferences(scriptEffects, result);


         foreach (var scriptEff in scriptEffects)
         {
            scriptEff.SetReplaceSources();
            Scopes.ScriptedEffects.TryAdd(scriptEff.name, scriptEff.CreateEffect);
         }
         Globals.ScriptedEffects = scriptEffects;
         
         return true;
      }

      private static void ReplaceReferences(HashSet<ScriptedEffectImpl> effects, List<string> orderedEffects)
      {
         // TODO optimization
      }


      private static bool TopologicalSorting(ICollection<ScriptedEffectImpl> scriptEffects, out List<string> result)
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

            dependencyDict.TryAdd(scrEff.name, dependencies);
         }

         if (!Sorting.TopologicalSortWithCycle(dependencyDict, out result, true))
         {
            _ = new LoadingError(PathObj.Empty, $"Cyclic dependency detected in scripted effects: {string.Join(", ", result)}\n\nScripted Effects won't work, until the cycle is resolved.", level:LogType.Critical);
            return false;
         }

         return true;
      }


      private void SetReplaceSources()
      {
         var offset = 0;
         StringBuilder sb = new(effect);
         foreach (Match newMatch in ReplaceRegex().Matches(effect))
         {
            if (!newMatch.Groups[1].Success)
               continue;
            var originalIndex = newMatch.Index - offset;
            _replaceSources.Add((newMatch.Groups[1].Value, originalIndex));
            sb.Remove(originalIndex, newMatch.Length);
            offset += newMatch.Length;
         }

         effect = sb.ToString();
      }

      public bool ReplaceValues(List<LineKvp<string, string>> values, out string replaced)
      {
         Debug.Assert(values.Count != 0, "This should never be called for non block scripted effects!");
         
         var sb = new StringBuilder(effect);
         var returnVal = true;
         var wasUsed = new bool[values.Count];
         var offset = 0;
         // The list is backwards so we can go forward and just replace the values
         for (var i = 0; i < _replaceSources.Count; i++)
         {
            var (key, index) = _replaceSources[i];
            var repIndex = values.FindIndex(x => x.Key.Equals(key));
            if (repIndex < 0)
            {
               _ = new LoadingError(Po, $"Missing argument in scripted effect ({name}) call: '{key}'");
               returnVal = false;
               continue;
            }
            wasUsed[repIndex] = true;
            var value = values[repIndex].Value;
            sb.Insert(index + offset, value);
            offset += value.Length;
         }

         for (var i = 0; i < values.Count; i++)
         {
            if (!wasUsed[i])
            {
               _ = new LoadingError(Po, $"Unused argument in scripted effect ({name}) call: '{values[i].Key}' with value '{values[i].Value}'");
               returnVal = false;
            }
         }

         replaced = sb.ToString();
         return returnVal;
      }

      [GeneratedRegex(@"\$(\w+)\$|"".*""")]
      private static partial Regex ReplaceRegex();
   }
}