using System;
using System.Collections;
using System.Diagnostics;
using System.Xml.Linq;
using Editor.DataClasses.Settings;
using Editor.Forms.PopUps;
using Editor.Helper;
using Editor.Saving;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;
using Timer = System.Windows.Forms.Timer;

namespace Editor.DataClasses.Commands;

public static class HistoryManager
{
   private static int _nodeId;
   private static HistoryNode _root;

   static HistoryManager()
   {
      Current = new (_nodeId++, new CInitial(), CommandHistoryType.Action);
      _root = Current;

      UndoDepthChanged += TriggerCompaction;
      RedoDepthChanged += TriggerCompaction;

      InitializeTimers();
   }


   // Events for when the undo/redo depth changes
   public static event EventHandler<Func<(int, int)>>? UndoDepthChanged;
   public static event EventHandler<Func<(int, int)>>? RedoDepthChanged;

   public static EventHandler UndoEvent = delegate { };
   public static EventHandler RedoEvent = delegate { };

   private static int _lastCompactionDepth = 0;
   private static Timer _autoCompactingTimer;
   private static Timer _updateToolStripTimer;
   private static DateTime _nextCompactionTime = DateTime.Now;
   
   // Add a new command to the history
   public static void AddCommand(ICommand newCommand, CommandHistoryType type = CommandHistoryType.Action)
   {
      var newNode = new HistoryNode(_nodeId++, newCommand, type, Current);
      Current.Children.Add(newNode);
      Current = newNode;

      UndoDepthChanged?.Invoke(null, GetUndoDepth);
   }

   // Check if there are any commands to undo or redo
   public static bool CanUndo => Current.Parent != null! || Current is CompactHistoryNode { HasStepUndo: true };

   // Check if there are any commands to redo
   public static bool CanRedo => Current.Children.Count > 0 || Current is CompactHistoryNode { HasStepRedo: true };
   public static HistoryNode Current { get; private set; }

   public static void UpdateToolStrip()
   {
      UndoDepthChanged?.Invoke(null, GetUndoDepth);
      RedoDepthChanged?.Invoke(null, GetRedoDepth);
   }

   // Undo the last command
   public static void Undo(bool stepUndo)
   {
      if (CanUndo)
      {
         if (Current.Type == CommandHistoryType.Compacting && Current is CompactHistoryNode compNode)
         {
            if (!compNode.HasStepUndo)
            {
               Current = Current.Parent;
               Undo(true); // we have no more StepUndos left so we go up one node
               return;
            }
            if (stepUndo)
            {
               compNode.StepUndo();
            }
            else
            {
               compNode.FullUndo();
               Current = Current.Parent;
            }
         }
         else
         {
            Current.Command.Undo();
            Current = Current.Parent;
         }

      }

      UpdateToolStrip();
      UndoEvent.Invoke(null, EventArgs.Empty);
   }

   public static void Redo(bool stepRedo, int childIndex = -1)
   {
      if (childIndex == -1)
         childIndex += Current.Children.Count;
      if (CanRedo && childIndex < Current.Children.Count)
      {
         if (Current is CompactHistoryNode compNode)
         {
            if (!compNode.HasStepRedo)
            {
               Current = Current.Children[childIndex];
               if (Current is CompactHistoryNode compact)
                  compact.FullRedo();
               else
                  Current.Command.Redo();
               goto end;
            }
            if (stepRedo)
               compNode.StepRedo();
            else
               compNode.FullRedo();
         }
         else
         {
            Current = Current.Children[childIndex];
            if (Current is CompactHistoryNode compact)
               compact.FullRedo();
            else
               Current.Command.Redo();
         }
         end:
         UpdateToolStrip();
         RedoEvent.Invoke(null, EventArgs.Empty);
      }

   }

   public static void RevertTo(int id)
   {
      if (id == Current.Id)
         return;
      var (undo, redo) = GetPathBetweenNodes(Current.Id, id);
      RestoreState(undo, redo);
   }

   private static void RestoreState(List<HistoryNode> undo, List<HistoryNode> redo)
   {
      foreach (var node in undo) 
      {
         if (node is CompactHistoryNode compNode)
            compNode.FullUndo();
         else
            node.Command.Undo(); // Cant use Undo() because it would change the current node
      }

      foreach (var node in redo)
      {
         if (node is CompactHistoryNode compNode)
            compNode.FullRedo();
         else
            node.Command.Redo(); // Cant use Redo() because it would change the current node
      }
      
      Current = redo[^1];

      UpdateToolStrip();
   }

   public static (int, int) GetUndoDepth()
   {
      var depth = 0;
      var total = 0;
      var node = Current;
      while (node.Parent != null!)
      {
         depth++;
         if (node is CompactHistoryNode compNode)
            total += compNode.CompactedNodes.Count;
         else
            total++;
         node = node.Parent;
      }

      return (depth, total);
   }

   public static (int, int) GetRedoDepth()
   {
      var depth = 0;
      var total = 0;
      var node = Current;
      while (node.Children.Count > 0)
      {
         depth++;
         if (node.Children[0] is CompactHistoryNode compNode)
            total += compNode.CompactedNodes.Count;
         else
            total++;
         node = node.Children[0];
      }

      return (depth, total);
   }
   
   public static (List<HistoryNode>, List<HistoryNode>) GetPathBetweenNodes(int from, int to)
   {
      List<HistoryNode> p1 = [];
      List<HistoryNode> p2 = [];

      GetPath(_root, p1, from);
      GetPath(_root, p2, to);

      int i = 0, intersection = -1;

      var a = i != p1.Count;
      var b = i != p2.Count;

      while (a && b)
      {
         if (a != b)
         {
            intersection = i - 1;
            break;
         }
         if (p1[i] == p2[i])
         {
            i++;
         }
         else
         {
            intersection = i - 1;
            break;
         }
         a = i != p1.Count;
         b = i != p2.Count;
      }

      for (var j = 0; j <= intersection; j++) 
      {
         p1.RemoveAt(0);
         p2.RemoveAt(0);
      }

      p1.Reverse();
      
      return (p1, p2);
   }

   private static bool GetPath(HistoryNode subRoot, List<HistoryNode> path, int id)
   {
      if (subRoot == null!)
         return false;


      path.Add(subRoot);

      if (subRoot.Id == id)
      {
         return true;
      }

      foreach (var child in subRoot.Children)
         if (GetPath(child, path, id))
         {
            return true;
         }

      path.Remove(subRoot);
      return false;
   }

   public static HistoryNode Root => _root;

   public static void Clear()
   {
      _nodeId = 0;
      _root = Current = new HistoryNode(_nodeId++, new CInitial(), CommandHistoryType.Action);
      Current = _root;
      UndoDepthChanged?.Invoke(null, GetUndoDepth);
      RedoDepthChanged?.Invoke(null, GetRedoDepth);
   }

   public static HistoryNode GetNodeAbove(HistoryNode node, int n)
   {
      var current = node;
      for (var i = 0; i < n; i++)
      {
         if (current.Parent == null!)
            return current;
         current = current.Parent;
      }

      return current;
   }

   public static HistoryNode GetNodeWithId(int id)
   {
      return GetNodeWithId(_root, id);
   }

   private static HistoryNode GetNodeWithId(HistoryNode node, int id)
   {
      if (node.Id == id)
         return node;

      foreach (var child in node.Children)
      {
         var result = GetNodeWithId(child, id);
         if (result != null!)
            return result;
      }

      return null!;
   }

   // ----------------------------------------- Compacting ----------------------------------------- \\

   public static void Uncompact(HistoryNode node)
   {
      for (var i = node.Children.Count - 1; i >= 0; i--)
      {
         Uncompact(node.Children[i]);
         if (node.Children[i] is CompactHistoryNode compNode)
            compNode.UnCompact();
      }
   }

   public static void Compact()
   {
      // We need to uncompact the tree first so that we can find all optimal groups
      Uncompact(_root);

      var groups = FindGroups(_root);
      var compGroups = FindCompactableGroups(groups);

      foreach (var group in compGroups)
      {
         if (group.Count < Globals.Settings.Misc.CompactingSettings.MinNumForCompacting)
            continue;

         var node = new CompactHistoryNode(_nodeId++, group);
         node.InsertInTree();
         if (Current == group[^1])
            Current = node;
      }

      UpdateToolStrip();
   }

   private static List<List<HistoryNode>> FindCompactableGroups(Dictionary<List<int>, List<HistoryNode>> groups)
   {
      var compGroups = new List<List<HistoryNode>>();

      foreach (var group in groups.Values)
      {
         // Sort nodes by ID for consistent order
         var sortedNodes = group.OrderBy(node => node.Id).ToList();

         List<HistoryNode> currentGroup = [];

         for (var i = 0; i < sortedNodes.Count; i++)
         {
            var currentNode = sortedNodes[i];

            // Check if the current group is linear
            if (currentGroup.Count > 0)
            {
               var lastNode = currentGroup[^1];
               if (lastNode.Children.Count != 1 || lastNode.Children[0] != currentNode)
               {
                  // Linearity breaks, finalize the current group
                  if (currentGroup.Count > 1) 
                     compGroups.Add(currentGroup);
                  currentGroup.Clear();
               }
            }

            // Add the current node to the current group
            currentGroup.Add(currentNode);

            // Handle the last node in the list
            if (i == sortedNodes.Count - 1 && currentGroup.Count > 1)
               compGroups.Add([..currentGroup]);
         }
      }

      return compGroups;
   }


   private static Dictionary<List<int>, List<HistoryNode>> FindGroups(HistoryNode root)
   {
      var groups = new Dictionary<List<int>, List<HistoryNode>>(new ListComparer<int>());
      TraverseTree(root, node =>
      {
         if (node.IsCompacted) return;

         // Add the node to the dictionary based on its targets
         if (!groups.TryGetValue(node.Command.GetTargetHash(), out var nodeList))
         {
            nodeList = [];
            groups[node.Command.GetTargetHash()] = nodeList;
         }
         nodeList.Add(node);
      });
      return groups;
   }

   private static void TraverseTree(HistoryNode node, Action<HistoryNode> action)
   {
      if (node == null!) 
         return;

      action(node);
      for (var i = node.Children.Count - 1; i >= 0; i--) 
         TraverseTree(node.Children[i], action);
   }

   // ========================================= Auto compacting code ========================================= \\

   public static void TriggerCompaction(object? sender, Func<(int, int)> _)
   {
      var (undoDepth, _) = GetUndoDepth();
      switch (Globals.Settings.Misc.CompactingSettings.AutoCompactingStrategy)
      {
         case CompactingSettings.AutoCompStrategy.None:
         case CompactingSettings.AutoCompStrategy.EveryXMinutes:
            return;
         case CompactingSettings.AutoCompStrategy.AfterXSize:
            // We only compact if the undo depth is greater than the last compaction depth and the amount to trigger a compaction,
            // to not end in a compaction loop ast compaction is an expensive operation
            if (undoDepth >= Globals.Settings.Misc.CompactingSettings.AutoCompactingMinSize + _lastCompactionDepth)
            {
               Compact();
               (_lastCompactionDepth, var _) = GetUndoDepth();
            }

            Globals.MapWindow.CompactionToolStrip.Text = $"Compacting in: {Globals.Settings.Misc.CompactingSettings.AutoCompactingMinSize + _lastCompactionDepth - undoDepth} changes";
            break;
      }
   }

   public static void InitializeTimers()
   {
      if (Globals.Settings.Misc.CompactingSettings.AutoCompactingStrategy != CompactingSettings.AutoCompStrategy.EveryXMinutes)
         return;
      StopTimers();
      _nextCompactionTime = DateTime.Now + TimeSpan.FromMinutes(Globals.Settings.Misc.CompactingSettings.AutoCompactingDelay);
      _autoCompactingTimer = new() {Interval = 60000 * Globals.Settings.Misc.CompactingSettings.AutoCompactingDelay};
      _autoCompactingTimer.Tick += (_, _) =>
      {
         TriggerCompaction(null, GetUndoDepth);
         Globals.MapWindow.UpdateCompactionToolStripTime(_nextCompactionTime - DateTime.Now);
         _nextCompactionTime = DateTime.Now + TimeSpan.FromMinutes(Globals.Settings.Misc.CompactingSettings.AutoCompactingDelay);
      };
      _autoCompactingTimer.Start();
      _updateToolStripTimer = new() {Interval = 5000};
      _updateToolStripTimer.Tick += (_, _) =>
      {
         Globals.MapWindow.UpdateCompactionToolStripTime(_nextCompactionTime - DateTime.Now);
      };
      _updateToolStripTimer.Start();
   }

   public static void StopTimers()
   {
      _autoCompactingTimer?.Stop();
      _updateToolStripTimer?.Stop();
   
   }
}


public class CompactHistoryNode : HistoryNode
{
   public List<HistoryNode> CompactedNodes { get; }
   private int current;

   public CompactHistoryNode(int id, List<HistoryNode> compactedNodes) : base(id, null!, CommandHistoryType.Compacting, compactedNodes[0].Parent)
   {
      CompactedNodes = compactedNodes;
      current = CompactedNodes.Count - 1;
   }

   public void InsertInTree()
   {
      // remove the compacted nodes from the tree
      Debug.Assert(CompactedNodes[0].Parent != null, "Parent must never be null when inserting a compacted node into the tree");

      CompactedNodes[0].Parent.Children.Remove(CompactedNodes[0]);
      CompactedNodes[0].Parent = null!;

      foreach (var endChild in CompactedNodes[^1].Children)
      {
         endChild.Parent = this;
         Children.Add(endChild);
      }

      CompactedNodes[^1].Children.Clear();

      // insert the node into the tree
      Parent.Children.Add(this);
   }

   public void UnCompact()
   {
      // remove the compacted nodes from the tree
      Parent.Children.Remove(this);

      foreach (var endChild in Children)
      {
         endChild.Parent = CompactedNodes[^1];
         CompactedNodes[^1].Children.Add(endChild);
      }

      Children.Clear();

      // insert the node into the tree
      CompactedNodes[0].Parent = Parent;
      Parent.Children.Add(CompactedNodes[0]);
   }

   public void StepUndo()
   {
      current--;
      Debug.Assert(current >= 0, $"False invocation of \"{nameof(StepUndo)}\" without checking availability first!");

      CompactedNodes[current].Command.Undo();
   }

   public void StepRedo()
   {
      current++;
      Debug.Assert(current < CompactedNodes.Count, $"False invocation of \"{nameof(StepRedo)}\" without checking availability first!");

      CompactedNodes[current].Command.Redo();
   }

   public void FullUndo()
   {
      List<HistoryNode> invertedNodes = new(CompactedNodes);
      invertedNodes.Reverse();
      foreach (var node in invertedNodes)
         node.Command.Undo();
   }

   public void FullRedo()
   {
      foreach (var node in CompactedNodes)
         node.Command.Redo();
   }

   public bool HasStepUndo => current > 0;
   public bool HasStepRedo => current < CompactedNodes.Count - 1;

   public string GetDescription()
   {
      return $"Compacting {CompactedNodes.Count} Nodes";
   }
}

//============================================================
// A Node for the HistoryManager to store the history of commands for undo/redo

public class HistoryNode(int id, ICommand command, CommandHistoryType type, HistoryNode parent = null!) : IEnumerable<(HistoryNode Node, int Level)>
{
   public int Id { get; } = id;
   public ICommand Command { get; } = command;
   public HistoryNode Parent { get; set; } = parent;
   public CommandHistoryType Type { get; } = type;
   public List<HistoryNode> Children { get; init; } = [];
   public bool IsCompacted { get; set; } = false;

   public HistoryNode GetChildWithId(int id)
   {
      return Children.Single(child => child.Id == id);
   }

   public IEnumerator<(HistoryNode Node, int Level)> GetEnumerator()
   {
      return Traverse(this, 0).GetEnumerator();
   }

   IEnumerator IEnumerable.GetEnumerator()
   {
      return GetEnumerator();
   }

   private IEnumerable<(HistoryNode, int)> Traverse(HistoryNode node, int level)
   {
      if (node == null) 
         yield break;

      yield return (node, level);

      foreach (var child in node.Children)
         foreach (var descendant in Traverse(child, level + 1))
            yield return descendant;
   }
}