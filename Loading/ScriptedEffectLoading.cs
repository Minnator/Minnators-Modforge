using System.Diagnostics;
using System.Text;
using Editor.DataClasses;
using Editor.DataClasses.GameDataClasses;
using Editor.Helper;

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

         // TODO could be sped up by not using GetElements but instead just writing a simple parser for the scripted_effects files which only detects openings,
         // TODO as GetElements is overkill and slower on larger files as here
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