using Editor.Commands;
using Editor.Controls;

namespace Editor.Forms
{
   public partial class HistoryTree : Form
   {
      private readonly Action<int> _callback;
      public HistoryTree(Action<int> callback)
      {
         InitializeComponent();
         _callback = callback;

         ScrollToBottom(); //TODO fix
      }

      private void ScrollToBottom()
      {
         if (HistoryTreeView.Nodes.Count > 0)
         {
            // Get the last node at the deepest level
            TreeNode lastNode = GetLastNode(HistoryTreeView.Nodes[0]);
            // Ensure the last node is visible
            lastNode.EnsureVisible();
         }
      }

      private TreeNode GetLastNode(TreeNode node)
      {
         while (node.Nodes.Count > 0)
         {
            node = node.Nodes[node.Nodes.Count - 1];
         }
         return node;
      }

      public void Visualize(HistoryNode rootNode)
      {
         HistoryTreeView.Nodes.Clear();
         var root = new HistoryTreeNode("Root", CommandHistoryType.Action);
         AddToNode(rootNode, root);
         foreach (TreeNode child in root.Nodes)
            HistoryTreeView.Nodes.Add(child);
         HistoryTreeView.ExpandAll();
      }

      public int CoutChildNodes(TreeNode node)
      {
         var count = 0;
         foreach (TreeNode child in node.Nodes)
         {
            count++;
            count += CoutChildNodes(child);
         }

         return count;
      }
      
      public void VisualizeFull(HistoryNode rootNode)
      {
         HistoryTreeView.Nodes.Clear();
         HistoryTreeView.Nodes.Add(AddToNodeFull(rootNode));
         HistoryTreeView.ExpandAll();
      }
      
      private static TreeNode AddToNodeFull(HistoryNode history)
      {
         var node = new HistoryTreeNode(history.Command.GetDescription(), history.Type)
         {
            Tag = history.Id
         };

         foreach (var child in history.Children)
            node.Nodes.Add(AddToNodeFull(child));

         return node;
      }

      private static void AddToNode(HistoryNode history, HistoryTreeNode parent)
      {
         // only add nodes if they are an Action
         if (history.Type is CommandHistoryType.Action)
         {
            var node = new HistoryTreeNode(history.Command.GetDescription(), history.Type)
            {
               Tag = history.Id
            };
            parent.Nodes.Add(node);
            parent = node;
         }

         foreach (var child in history.Children)
            AddToNode(child, parent);
      }

      private void RestoreButton_Click(object sender, EventArgs e)
      {
         if (HistoryTreeView.SelectedNode is null)
            return;

         _callback.Invoke((int)HistoryTreeView.SelectedNode.Tag);
         Close();
      }

      private void ShowAllSelections_CheckedChanged(object sender, EventArgs e)
      {
         if (ShowAllSelections.Checked)
         {
            VisualizeFull(Globals.HistoryManager.GetRoot());
         }
         else
         {
            Visualize(Globals.HistoryManager.GetRoot());
         }
      }
   }
}
