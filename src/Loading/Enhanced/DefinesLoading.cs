using System.Text.RegularExpressions;
using Editor.DataClasses.GameDataClasses;
using Editor.ErrorHandling;
using Editor.Helper;
using Editor.Saving;

namespace Editor.Loading.Enhanced
{
   public static partial class DefinesLoading
   {

      public static readonly Regex LuaReplace = LuaCommentReplace();

      [GeneratedRegex(@"--.*|,", RegexOptions.Compiled)]
      private static partial Regex LuaCommentReplace();


      public static void Load()
      {
         LoadVanilla();
      }

      private static bool LoadVanilla()
      {

         if (!FilesHelper.GetVanillaPath(out var filepath,"common", "defines.lua"))
         {
            _ = new LoadingError(null!, "Vanilla defines.lua not found!", -1, -1, ErrorType.FileNotFound, LogType.Critical);
            return false;
         }

         var po = PathObj.FromPath(filepath);

         if (!PreProcessDefines(po, out var value).Log())
            return false;

         var (blocks, _) = po.LoadBase(EnhancedParser.FileContentAllowed.BlocksOnly, value);

         

         if (blocks.Count != 1) 
            _ = new LoadingError(po, "defines.lua must contain exactly one block!", blocks.Count > 1 ? blocks[1].StartLine : 0, -1, ErrorType.InvalidFileStructure);
         // We recover by only using the first block found.

         Define.GlobalNameSpace = blocks[0].Name;

         byte current = 0;

         foreach (var block in blocks[0].GetSubBlocks(true, po))
         {
            var contents = block.GetContentElements(true, po);

            if (contents.Count != 1)
               _ = new LoadingError(po, $"Namespace \"{block.Name}\" must contain exactly one content element.!", block.StartLine, -1, ErrorType.InvalidFileStructure);
            // We recover by only using the first block found.
            
            Define.NameSpaces.Add(current, block.Name.Trim());

            var content = contents[0];
            foreach (var kvp in content.GetLineKvpEnumerator(po, trimQuotes:false))
            {
               if (!GetDefineType(kvp.Value, out var convValue, out var type))
               {
                  _ = new LoadingError(po, $"Failed to parse value \"{kvp.Value}\"!", kvp.Line, -1, ErrorType.InvalidValue);
                  continue;
               }  
               var define = new Define(type, current, kvp.Key, convValue);
               if (!Globals.Defines.TryAdd(define.GetNameSpaceString(), define))
               {
                  _ = new LoadingError(po, $"Define \"{define.GetNameSpaceString()}\" already exists!", kvp.Line, -1, ErrorType.DuplicateObjectDefinition);
               }
            }

            current++;
         }
         return true;
      }

      private static void LoadMod()
      {

      }

      private static IErrorHandle PreProcessDefines(PathObj po, out string value)
      {
         if (!IO.ReadAllInANSI(po, out value))
            return new LoadingError(po, "Failed to read file!", -1, -1, ErrorType.FileNotFound, addToManager:false);

         value = LuaReplace.Replace(value, " ");
         return ErrorHandle.Success;
      }

      private static bool GetDefineType(string value, out object parsedValue, out Define.DefineType type)
      {
         if (value.StartsWith('\"') && value.EndsWith('\"'))
         {
            parsedValue = value;
            type = Define.DefineType.String;
            return true;
         }
         if (value.Contains('.') && float.TryParse(value, out var floatvalue))
         {
            parsedValue = floatvalue;
            type = Define.DefineType.Float;
            return true;
         }
         if (int.TryParse(value, out var intValue))
         {
            parsedValue = intValue;
            type = Define.DefineType.Int;
            return true;
         }
         
         parsedValue = null!;
         type = Define.DefineType.String;
         return false;
      }
   }
}