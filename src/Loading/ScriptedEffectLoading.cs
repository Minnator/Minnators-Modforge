using System.Diagnostics;
using Editor.DataClasses;
using Editor.Helper;
using Editor.Parser;
using Parsing = Editor.Parser.Parsing;

namespace Editor.Loading
{
   public static class ScriptedEffectLoading
   {
      /// <summary>
      /// Gathers all names of the scripted Effects in common/scripted_effects/*.txt files
      /// </summary>
      public static void LoadScriptedEffects()
      {
         var sw = Stopwatch.StartNew();
         var files = FilesHelper.GetFilesFromModAndVanillaUniquely("*.txt", "common", "scripted_effects");
         HashSet<string> scriptedEffects = [];

         Parallel.ForEach(files, file =>
         {
            IO.ReadAllInANSI(file, out var str);
            Parsing.RemoveCommentFromMultilineString(ref str, out var content);
            HashSet<string> effects = [];

            var blocks = Parsing.GetElements(0, ref content);
            
            foreach (var element in blocks)
            {
               if (element is not Block block)
               {
                  Globals.ErrorLog.Write($"Error in scripted_effects file {file}: Invalid content: {element}");
                  continue;
               }
               effects.Add(block.Name);
            }

            lock (scriptedEffects)
            {
               scriptedEffects.UnionWith(effects);
            }
         });

         Globals.ScriptedEffectNames = scriptedEffects;
         sw.Stop();
         Globals.LoadingLog.WriteTimeStamp("Scripted Effects", sw.ElapsedMilliseconds) ;

      }
   }
}