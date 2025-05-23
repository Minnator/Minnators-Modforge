using System.Text;
using Editor.Loading.Enhanced;
using Editor.Loading.Enhanced.PCFL.Implementation;
using Editor.Parser;
using Editor.Saving;

namespace Editor.Forms.Feature
{
   public partial class IdCollectionEditor : Form
   {
      public IdCollectionEditor()
      {
         InitializeComponent();
      }

      private void checkBox1_CheckedChanged(object sender, EventArgs e)
      {

      }

      private void RemoveButton_Click(object sender, EventArgs e) => RemoveIntsFromElements(GetTargetInts());

      private void RemoveIntsFromElements(List<int> integers)
      {
         var (blocks, elements) = EnhancedParser.GetElements(SourceTB.Text);
         var sb = new StringBuilder();
         var tabs = 0;

         foreach (var block in blocks)
         {
            SavingUtil.OpenBlock(ref tabs, block.Name, ref sb);
            TraverseBlocks(block.GetElements(), integers, ref tabs, ref sb);
            sb.AppendLine();
            SavingUtil.CloseBlock(ref tabs, ref sb);
         }

         foreach (var element in elements)
            ProcessElement(element, integers, ref tabs, ref sb);

         OutputTB.Text = sb.ToString();
      }

      private void TraverseBlocks(IEnumerable<IEnhancedElement> elements, List<int> toRemove, ref int tabs, ref StringBuilder sb)
      {
         foreach (var element in elements)
         {
            if (element is EnhancedBlock block)
               TraverseBlocks(block.GetElements(), toRemove, ref tabs, ref sb);
            else if (element is EnhancedContent value)
               ProcessElement(value, toRemove, ref tabs, ref sb);
         }
      }

      private void ProcessElement(EnhancedContent content, List<int> toRemove, ref int tabs, ref StringBuilder sb)
      {
         List<int> integers = [];
         foreach (var (line, _) in content.GetLineEnumerator())
            integers.AddRange(EnhancedParsing.GetListIntFromString(line, -1, PathObj.Empty));
         integers = integers.Except(toRemove).ToList();
         FormatIntInSb(ref tabs, integers, sb, PaddingCheckBox.Checked);
      }


      private void SortUpButton_Click(object sender, EventArgs e) => SortAcending();
      private void SortDown_Click(object sender, EventArgs e) => SortDecending();
      private void ExceptButton_Click(object sender, EventArgs e) => Except();
      private void IntersectButton_Click(object sender, EventArgs e) => Intersect();
      private void UnionButton_Click(object sender, EventArgs e) => Union();
      private void CopyButton_Click(object sender, EventArgs e) => Clipboard.SetText(OutputTB.Text);

      private void Except() => OutputTB.Text = FormatInts(GetSourceInts().Except(GetTargetInts()).ToList(), PaddingCheckBox.Checked);
      private void Intersect() => OutputTB.Text = FormatInts(GetSourceInts().Intersect(GetTargetInts()).ToList(), PaddingCheckBox.Checked);
      private void Union() => OutputTB.Text = FormatInts(GetSourceInts().Union(GetTargetInts()).ToList(), PaddingCheckBox.Checked);
      private void SortAcending() => OutputTB.Text = FormatInts(GetSourceInts().OrderBy(x => x).ToList(), PaddingCheckBox.Checked);
      private void SortDecending() => OutputTB.Text = FormatInts(GetSourceInts().OrderByDescending(x => x).ToList(), PaddingCheckBox.Checked);


      private List<int> GetSourceInts() => GetAllSimpleInts(SourceTB, out var integers) ? integers : [];
      private List<int> GetTargetInts() => GetAllSimpleInts(InputTB, out var integers) ? integers : [];

      // The entire text of the given textBox must be ints separated by any amount of spaces, returns or tabs
      private bool GetAllSimpleInts(TextBox tb, out List<int> integers)
      {
         integers = [];
         foreach (var line in tb.Text.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries))
            foreach (var number in line.Split([' ', '\t'], StringSplitOptions.RemoveEmptyEntries))
               if (int.TryParse(number, out var result))
                  integers.Add(result);
               else
               {
                  MessageBox.Show($"Invalid number: {number}!\nThis option is only available for pure integer lists.", "Error", MessageBoxButtons.OK,
                                  MessageBoxIcon.Error);
                  return false;
               }

         return true;
      }

      private string FormatInts(List<int> integers, bool padding)
      {
         if (integers.Count == 0)
            return string.Empty;

         var sb = new StringBuilder();
         var tabs = 0;
         FormatIntInSb(ref tabs, integers, sb, padding);

         return sb.ToString();
      }

      private void FormatIntInSb(ref int tabs, List<int> integers, StringBuilder sb, bool padding)
      {
         var perLine = 0;
         var startOfLine = true;
         foreach (var number in integers)
         {
            if (startOfLine)
            {
               sb.Append('\t', tabs);
               startOfLine = false;
               perLine++;
            } 
            else if (perLine == 20)
            {
               sb.AppendLine();
               sb.Append('\t', tabs);
               perLine = 1;
               startOfLine = true;
            }
            else if (!startOfLine)
            {
               perLine++;
            }

            if (padding)
               sb.Append(number.ToString().PadLeft(4, ' '));
            else
               sb.Append(number);

            sb.Append(' ');
         }
      }
   }
}
