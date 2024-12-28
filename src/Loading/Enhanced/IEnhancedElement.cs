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
      public string GetContent();
      public string GetFormattedString(int tabs, ref StringBuilder sb);
      public void AppendFormattedContent(int tabs, ref StringBuilder sb);
   }

   public class EnhancedBlock(string name, int startLine) : IEnhancedElement
   {
      public string Name { get; set; } = name;
      public List<IEnhancedElement> Elements { get; set; } = [];
      public bool IsBlock => true;
      public int StartLine { get; } = startLine;
      public List<EnhancedContent> ContentElements => Elements.Where(e => !e.IsBlock).Select(e => e as EnhancedContent).ToList()!;
      public List<EnhancedBlock> SubBlocks => Elements.Where(e => e.IsBlock).Select(e => e as EnhancedBlock).ToList()!;

      public EnhancedBlock() : this(string.Empty, 1)
      {

      }

      protected void AppendContent(int tabs, StringBuilder sb)
      {
         foreach (var element in Elements)
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
      
      public override string ToString()
      {
         return Name;
      }
   }

   public class EnhancedContent(string value, int startLine) : IEnhancedElement
   {
      public string Value { get; set; } = value;
      public bool IsBlock { get; } = false;
      public int StartLine { get; } = startLine;

      public EnhancedContent() : this(string.Empty, 1)
      {

      }

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

      public IEnumerable<(string, int)> GetLineEnumerator(PathObj pathObj, bool showError = true) 
      {
         var lines = Value.Split('\n');
         var lineNum = StartLine;
         foreach (var line in lines)
         {
            lineNum++;
            if (string.IsNullOrWhiteSpace(line))
               continue;
            yield return (line, lineNum);
         }
      }

      public IEnumerable<LineKvp<string, string>> GetLineKvpEnumerator(PathObj pathObj, bool showError = true)
      {

         var lines = Value.Split('\n');
         var lineNum = StartLine;
         foreach (var line in lines)
         {
            lineNum++;
            if (string.IsNullOrWhiteSpace(line))
               continue;
            var split = line.Split('=');
            if (split.Length != 2)
            {
               if (showError)
                  _ = new LoadingError(pathObj, "Expected a key value pair but got only one value", lineNum, 0);
               continue;
            }
            yield return new(split[0].Trim(), split[1].TrimQuotes(), lineNum);
         }
      }

      public override string ToString()
      {
         return Value;
      }
   }


}