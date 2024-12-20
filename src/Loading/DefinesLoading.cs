using System.Text.RegularExpressions;
using Editor.ErrorHandling;
using Editor.Helper;

namespace Editor.Loading
{
   public static partial class DefinesLoading
   {
      private static readonly Regex MaxGovernmentRankRegex = GenerateMaxGovernmentRankRegex();
      [GeneratedRegex(@"MAX_GOVERNMENT_RANK\s*=\s*(?<value>\d+)", RegexOptions.Compiled)]
      private static partial Regex GenerateMaxGovernmentRankRegex();

      public static void Load()
      {
         return;
         var files = FilesHelper.GetAllFilesInFolder("*.lua", "common", "defines");

         if (files.Count == 0)
         {
            if (!FilesHelper.GetFilePathUniquely(out var path, "common", "defines", "defines.lua"))
            {
               _ = new ErrorObject("Can not locate 00_defines.txt", ErrorType.RequiredFileNotFound);
               return;
            }
            files.Add(path);
         }

         var result = string.Empty;
         foreach (var file in files)
         {
            IO.ReadAllInANSI(file, out var rawContent);
            var regexMatch = MaxGovernmentRankRegex.Match(rawContent);
            if (regexMatch.Success)
               result = regexMatch.Groups["value"].Value;
         }

         if (!string.IsNullOrWhiteSpace(result))
         {
            _ = new ErrorObject("Can not locate \"MAX_GOVERNMENT_RANK\" in 00_defines.txt", ErrorType.RequiredFileNotFound);
            return;
         }
         Globals.MaxGovRank = int.Parse(result);
      }

   }

}