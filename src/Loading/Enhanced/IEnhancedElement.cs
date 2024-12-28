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

      public IEnumerator<IEnhancedElement> GetElements() => Elements.GetEnumerator();
      public IEnumerator<IEnhancedElement> GetContentElements() => Elements.Where(e => !e.IsBlock).GetEnumerator();
      public IEnumerator<IEnhancedElement> GetBLockElements() => Elements.Where(e => e.IsBlock).GetEnumerator();

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
         var enumerator = GetContentEnumerator(PathObj.Empty, false);
         while (enumerator.MoveNext())
         {
            var lkvp = enumerator.Current;
            SavingUtil.AddString(tabs, lkvp.Value, lkvp.Key, ref sb);
         }
      }

      public IEnumerator<LineKvp<string, string>> GetContentEnumerator(PathObj pathObj, bool showError = true)
      {
         return GetLineKvps(Value, pathObj, StartLine, showError).GetEnumerator();
      }


      private static List<LineKvp<string, string>> GetLineKvps(string str, PathObj pathObj, int startingLine, bool showError)
      {
         var lines = str.Split('\n');
         List<LineKvp<string, string>> lineKvps = [];
         var lineNum = startingLine;
         foreach (var line in lines)
         {
            lineNum++;
            if (string.IsNullOrWhiteSpace(line))
               continue;
            var split = line.Split('=');
            if (split.Length != 2)
            {
               if (showError)
                  _ = new LoadingError(pathObj, lineNum, 0, "Expected a key value pair but got only one value");
               continue;
            }
            lineKvps.Add(new(split[0].Trim(), split[1].TrimQuotes(), lineNum));
         }
         return lineKvps;
      }

      public override string ToString()
      {
         return Value;
      }
   }


}