
using System.Text;
using Editor.Helper;
using Editor.Loading.Enhanced.PCFL.Implementation;
using Editor.Saving;

namespace Editor.Loading.Enhanced
{
   public class ScriptedEffect(string name, List<IEnhancedElement> elements, ref PathObj po) 
   {
      public PathObj Po { get; } = po;
      public string Name { get; } = name;
      public List<IToken> Tokens { get; } = [];

      // Temporary
      public List<IEnhancedElement> EnhancedElements { get; set; } = elements;
      public List<string> References = [];
      
      public string DebugString()
      {
         var sb = new StringBuilder();
         sb.Append($"ScriptedEffect: {Name}\n");
         foreach (var token in Tokens) 
            sb.Append($"\t{token.GetTokenName()} = {token.GetTokenDescription()}");
         return sb.ToString();
      }
   }

   public static class ScriptedEffectLoading
   {

      public static List<ScriptedEffect> LoadAndParse()
      {
         // Get all scripted effects from the files, now that we have the names of them we can resolve them if they contain each other
         List<ScriptedEffect> scriptEffects = [];
         var files = PathManager.GetAllFilesInFolder(internalPath:["common", "scripted_effects"]);

         foreach (var file in files)
         {
            var po = PathObj.FromPath(file);
            var (blocks, _) = po.LoadBase(EnhancedParser.FileContentAllowed.BlocksOnly);

            foreach (var block in blocks)
            {
               var scrEff = new ScriptedEffect(block.Name, block.GetElements().ToList(),ref po);
               scriptEffects.Add(scrEff);
            }
         }
         ResolveEffectReferences(scriptEffects);
         return scriptEffects;
      }

      public static List<ScriptedEffect> LoadParseFile(string file)
      {
         List<ScriptedEffect> scriptEffects = [];

         var po = PathObj.FromExternalPath(file);
         var elements = po.LoadBaseOrder();

         foreach (var element in elements)
         {
            if (element is not EnhancedBlock block)
               continue;
            var scrEff = new ScriptedEffect(block.Name, block.GetElements().ToList(), ref po);
            scriptEffects.Add(scrEff);
         }

         ResolveEffectReferences(scriptEffects);
         return scriptEffects;
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

      private static void ResolveReferences(Dictionary<string, ScriptedEffect> validScriptedEffects, PathObj po)
      {
         // Topological sort
         // 

         Dictionary<string, List<string>> objects = [];


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
                  throw new ($"Cyclic dependency detected in scripted effect {block.Name}");
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
                     throw new ($"Cyclic dependency detected in scripted effect {line.Key}");
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

   }
}