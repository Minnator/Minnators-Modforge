using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;
using Editor.DataClasses.GameDataClasses;
using Editor.DataClasses.Saveables;
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
         LoadMod();
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

            if (!Define.NameSpaces.TryAdd(current, block.Name.Trim())) 
               _ = new LoadingError(po, $"Namespace \"{block.Name}\" already exists!", block.StartLine, -1, ErrorType.DuplicateObjectDefinition);

            var content = contents[0];
            foreach (var kvp in content.GetLineKvpEnumerator(po, trimQuotes:false))
            {
               if (!GetDefineType(kvp.Value, out var convValue, out var type))
               {
                  _ = new LoadingError(po, $"Failed to parse value \"{kvp.Value}\"!", kvp.Line, -1, ErrorType.InvalidValue);
                  continue;
               }  
               var define = new Define(type, current, kvp.Key, convValue);
               lock (Globals.Defines)
               {
                  if (!Globals.Defines.TryAdd(define.GetNameSpaceString(), define)) 
                     _ = new LoadingError(po, $"Define \"{define.GetNameSpaceString()}\" already exists!", kvp.Line, -1, ErrorType.DuplicateObjectDefinition);
               }
            }

            current++;
         }
         return true;
      }

      private static void LoadMod()
      {
         var files = FilesHelper.GetAllFilesInFolder(searchPattern:"*.lua", Globals.ModPath, "common", "defines");

         foreach (var file in files)
         {
            var po = PathObj.FromPath(file);
            if (!PreProcessDefines(po, out var value).Log())
               break;
            var (_,content) = po.LoadBase(EnhancedParser.FileContentAllowed.ContentOnly, value);
            
            Debug.Assert(content.Count <= 1, "There must only be one content element in a define.lua file in the mod part!");
            
            // Should be only one but in case of maleformed file recover by reading all contents
            foreach(var contents in content){
               foreach (var line in contents.GetLineKvpEnumerator(po, trimQuotes: false))
               {
                  if (!Globals.Defines.TryGetValue(line.Key.Trim(), out var define))
                  {
                     _ = new LoadingError(po, $"Define \"{line.Key}\" not found!", line.Line, -1, ErrorType.ObjectNotFound);
                     continue;
                  }

                  if (!define.SetValue(line.Value)) 
                     _ = new LoadingError(po, $"Failed to set value \"{line.Value}\" for define \"{line.Key}\". Value should be of type \"{define.Type}\"!", line.Line, -1, ErrorType.InvalidValue);
               }
            }

         }
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
         if (value.Contains('.') && float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var result))
         {
            parsedValue = result;
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