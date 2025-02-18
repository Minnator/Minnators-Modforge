using System.Text;
using Editor.ErrorHandling;
using Editor.Parser;
using Editor.Saving;

namespace Editor.Loading.Enhanced
{
   public interface IEnhancedElement
   {
      public bool IsBlock { get; }
      public int StartLine { get; }
      public int Index { get; }
      public string GetContent();
      public string GetFormattedString(int tabs, ref StringBuilder sb);
      public void AppendFormattedContent(int tabs, ref StringBuilder sb);

      public static bool operator < (IEnhancedElement a, IEnhancedElement b) => a.Index < b.Index;
      public static bool operator > (IEnhancedElement a, IEnhancedElement b) => a.Index > b.Index;
      public static bool operator <= (IEnhancedElement a, IEnhancedElement b) => a.Index <= b.Index;
      public static bool operator >= (IEnhancedElement a, IEnhancedElement b) => a.Index >= b.Index;
   }

   public class EnhancedBlock(string name, int startLine, int index) : IEnhancedElement
   {
      public string Name { get; set; } = name;
      public bool IsBlock => true;
      public int StartLine { get; } = startLine;
      public int Index { get; } = index;

      public List<EnhancedContent> ContentElements { get; set; } = [];
      public List<EnhancedBlock> SubBlocks { get; set; } = [];
      public int SubBlockCount => SubBlocks.Count;
      public int ContentElementCount => ContentElements.Count;


      public List<EnhancedBlock> GetSubBlocks(bool onlyBlocks, PathObj po)
      {
         if (onlyBlocks && ContentElements.Count > 0)
         {
            _ = new LoadingError(po, $"Expected no content in block: {Name}!", StartLine, 0, ErrorType.UnexpectedContentElement);
         }
         return SubBlocks;
      }

      public List<EnhancedContent> GetContentElements(bool onlyContent, PathObj po)
      {
         if (onlyContent && SubBlocks.Count > 0)
         {
            _ = new LoadingError(po, $"Expected no subBlocks in block: {Name}!", StartLine, 0, ErrorType.UnexpectedBlockElement);
         }
         return ContentElements;
      }

      protected void AppendContent(int tabs, StringBuilder sb)
      {
         foreach (var block in SubBlocks)
            block.GetFormattedString(tabs + 1, ref sb);
         foreach (var element in ContentElements)
            element.GetFormattedString(tabs + 1, ref sb);
      }

      public string GetContent()
      {
         var sb = new StringBuilder();
         AppendContent(0, sb);
         return sb.ToString();
      }

      public string GetFormattedString(int tabs, ref StringBuilder sb)
      {
         AppendFormattedContent(tabs, ref sb);
         return sb.ToString();
      }

      public void AppendFormattedContent(int tabs, ref StringBuilder sb)
      {
         SavingUtil.OpenBlock(ref tabs, Name, ref sb);
         AppendContent(tabs, sb);
         SavingUtil.CloseBlock(ref tabs, ref sb);
      }

      public bool GetSubBlockByName(string name, out EnhancedBlock block)
      {
         return GetBlockByName(name, SubBlocks, out block);
      }

      public bool GetAllSubBlockByName(string name, out List<EnhancedBlock> blocks)
      {
         return GetAllBlockByName(name, SubBlocks, out blocks);
      }

      public IEnumerable<IEnhancedElement> GetElements() => EnhancedParser.MergeBlocksAndContent(SubBlocks, ContentElements);

      public static bool GetBlockByName(string name, ICollection<EnhancedBlock> blocks, out EnhancedBlock result)
      {
         result = blocks.FirstOrDefault(b => b.Name.Equals(name))!;
         return result is not null;
      }

      public static bool GetAllBlockByName(string name, ICollection<EnhancedBlock> blocks, out List<EnhancedBlock> result)
      {
         result = blocks.Where(b => b.Name.Equals(name)).ToList()!;
         return result.Count > 0;
      }

      public bool GetSubBlocksByName(string name, out List<EnhancedBlock> blocks)
      {
         return GetBlocksByName(name, SubBlocks, out blocks);
      }

      public static bool GetBlocksByName(string name, ICollection<EnhancedBlock> blocks, out List<EnhancedBlock> result)
      {
         result = blocks.Where(b => b.Name == name).ToList();
         return result.Count > 0;
      }

      public override string ToString()
      {
         return Name;
      }
   }


   public class EnhancedContent(string value, int startLine, int index) : IEnhancedElement
   {
      public string Value { get; set; } = value;
      public bool IsBlock { get; } = false;
      public int StartLine { get; } = startLine;
      public int Index { get; } = index;

      public string GetContent() => Value;

      public string GetFormattedString(int tabs, ref StringBuilder sb)
      {
         AppendFormattedContent(tabs, ref sb);
         return sb.ToString();
      }

      public void AppendFormattedContent(int tabs, ref StringBuilder sb)
      {
         var enumerator = GetLineKvpEnumerator(PathObj.Empty, false);
         foreach (var kvp in enumerator) 
            SavingUtil.AddString(tabs, kvp.Value, kvp.Key, ref sb);
      }

      public IEnumerable<(string, int)> GetLineEnumerator() 
      {
         var lines = Value.Split('\n');
         var lineNum = StartLine;
         foreach (var line in lines)
         {
            if (string.IsNullOrWhiteSpace(line))
            {
               lineNum++;
               continue;
            }
            yield return (line, lineNum);
            lineNum++;
         }
      }

      public IEnumerable<(string, int)> GetStringListEnumerator()
      {
         var lines = Value.Split('\n');
         var lineNum = StartLine;
         foreach (var line in lines)
         {
            if (string.IsNullOrWhiteSpace(line))
            {
               lineNum++;
               continue;
            }
            var strings = line.Split(' ');
            foreach (var str in strings)
               yield return (str, lineNum);
            lineNum++;
         }
      }

      public IEnumerable<LineKvp<string, string>> GetLineKvpEnumerator(PathObj pathObj, bool showError = true, bool trimQuotes = true)
      {

         var lines = Value.Split('\n');
         var lineNum = StartLine;
         foreach (var line in lines)
         {
            if (string.IsNullOrWhiteSpace(line))
            {
               lineNum++;
               continue;
            }
            var split = line.Split('=');
            if (split.Length != 2)
            {
               if (showError)
                  _ = new LoadingError(pathObj, "Expected a key value pair but got only one value", lineNum++, 0);
               continue;
            }
            yield return new(split[0].Trim(), trimQuotes ? split[1].TrimQuotes() : split[1].Trim(), lineNum);
            lineNum++;
         }
      }

      public override string ToString()
      {
         return Value;
      }
   }


}