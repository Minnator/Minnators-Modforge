using System.Diagnostics;
using Editor.Helper;
using Editor.Loading.Enhanced;
using Editor.Parser;
using Parsing = Editor.Parser.Parsing;

namespace Editor.Loading
{
   
   public static class ScriptedEffectLoading
   {
      /// <summary>
      /// Gathers all names of the scripted Effects in common/scripted_effects/*.txt files
      /// </summary>
      public static void Load()
      {
         var files = PathManager.GetFilesFromModAndVanillaUniquely("*.txt", "common", "scripted_effects");

         ScriptedEffectImpl.ParseScriptedEffectDefinition(files, out var list);
      }
   }
}