using System.Windows.Forms;

namespace Editor.Controls;

public class HistoryTreeNode(HistoryType historyType) : TreeNode
{
   public HistoryType HistoryType { get; } = historyType;

   public HistoryTreeNode(string text, HistoryType historyType): this(historyType)
   {
      Text = text;
   }
}