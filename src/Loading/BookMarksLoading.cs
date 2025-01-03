using Editor.DataClasses.GameDataClasses;
using Editor.DataClasses.Misc;
using Editor.ErrorHandling;
using Editor.Helper;
using Editor.Parser;

namespace Editor.Loading
{
   public static class BookMarksLoading
   {
      public static void Load()
      {
         var files = FilesHelper.GetFilesFromModAndVanillaUniquely("*.txt", "common", "bookmarks");
         if (files.Count == 0)
            return;

         List<Bookmark> bookMarks = [];

         // TODO: Parallelize this
         foreach (var file in files)
         {
            IO.ReadAllInANSI(file, out var newContent);
            Parsing.RemoveCommentFromMultilineString(ref newContent, out var content);
            var elements = Parsing.GetElements(0, ref content);

            if (elements.Count == 0)
            {
               _ = new LogEntry(LogType.Information, $"Empty bookmarks file: {file}");
               continue;
            }

            foreach (var element in elements)
            {
               if (element is not Block { Name: "bookmark" } block)
               {
                  _ = new LogEntry(LogType.Error, $"Error in bookmarks file: Invalid content: {element}, expected \"bookmark = {{<content>}}\"");
                  continue;
               }

               var name = "";
               var desc = "";
               var date = Date.MinValue;
               List<Tag> countryTags = [];

               var kvps = Parsing.GetKeyValueListWithoutQuotes(block.GetContent);
               foreach (var kvp in kvps)
               {
                  switch (kvp.Key)
                  {
                     case "name":
                        name = kvp.Value;
                        break;
                     case "desc":
                        desc = kvp.Value;
                        break;
                     case "date":
                        _ = Date.TryParse(kvp.Value, out date);
                        break;
                     case "country":
                        if (Tag.TryParse(kvp.Value, out var tag))
                           countryTags.Add(tag);
                        break;
                  }
               }

               if (string.IsNullOrWhiteSpace(name))
                  _ = new LogEntry(LogType.Warning, $"Warning in bookmarks file: Name is missing: {block}");
               if (string.IsNullOrWhiteSpace(desc))
                  _ = new LogEntry(LogType.Warning, $"Warning in bookmarks file: Description is missing: {block}");
               if (date == Date.MinValue)
                  _ = new LogEntry(LogType.Warning, $"Warning in bookmarks file: Date is missing or incorrect: {block}");
               if (countryTags.Count == 0)
                  _ = new LogEntry(LogType.Information, $"No Countries defined in bookmark: {block}");

               bookMarks.Add(new (name, desc, date, countryTags));
            }
         }
         bookMarks.Sort((a, b) => a.Date.CompareTo(b.Date));
         Globals.Bookmarks = bookMarks;
      }
   }
}