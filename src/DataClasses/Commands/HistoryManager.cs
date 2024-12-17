using System.Collections;

namespace Editor.DataClasses.Commands;

public static class HistoryManager
{
   private static int _nodeId;
   private static HistoryNode _root;

   static HistoryManager()
   {
      Current = new (_nodeId++, new CInitial(), CommandHistoryType.Action);
      _root = Current;
   }


   // Events for when the undo/redo depth changes
   public static event EventHandler<int>? UndoDepthChanged;
   public static event EventHandler<int>? RedoDepthChanged;

   public static EventHandler UndoEvent = delegate { };
   public static EventHandler RedoEvent = delegate { };

   
   public static HistoryNode GetCurrent => Current;

   // Add a new command to the history
   public static void AddCommand(ICommand newCommand, CommandHistoryType type = CommandHistoryType.Action)
   {
      var newNode = new HistoryNode(_nodeId++, newCommand, type, Current);
      Current.Children.Add(newNode);
      Current = newNode;

      UndoDepthChanged?.Invoke(null, GetUndoDepth());
   }

   // Check if there are any commands to undo or redo
   public static bool CanUndo => Current.Parent != null!;

   // Check if there are any commands to redo
   public static bool CanRedo => Current.Children.Count > 0;
   public static HistoryNode Current { get; private set; }

   // Undo the last command
   public static void Undo()
   {
      if (CanUndo)
      {
         Current.Command.Undo();
         Current = Current.Parent;
      }

      UndoDepthChanged?.Invoke(null, GetUndoDepth());
      UndoEvent.Invoke(null, EventArgs.Empty);
   }

   public static void Redo(int childIndex = -1)
   {
      if (childIndex == -1)
         childIndex += Current.Children.Count;
      if (CanRedo && childIndex < Current.Children.Count)
      {
         Current = Current.Children[childIndex];
         Current.Command.Redo();
      }

      RedoDepthChanged?.Invoke(null, GetRedoDepth());
      RedoEvent.Invoke(null, EventArgs.Empty);
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
         node.Command.Undo(); // Cant use Undo() because it would change the current node

      foreach (var node in redo)
         node.Command.Redo(); // Cant use Redo() because it would change the current node
      
      Current = redo[^1];

      UndoDepthChanged?.Invoke(null, GetUndoDepth());
      RedoDepthChanged?.Invoke(null, GetRedoDepth());
   }

   public static int GetUndoDepth()
   {
      var depth = 0;
      var node = Current;
      while (node.Parent != null!)
      {
         depth++;
         node = node.Parent;
      }

      return depth;
   }

   public static int GetRedoDepth()
   {
      var depth = 0;
      var node = Current;
      while (node.Children.Count > 0)
      {
         depth++;
         node = node.Children[0];
      }

      return depth;
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
      UndoDepthChanged?.Invoke(null, GetUndoDepth());
      RedoDepthChanged?.Invoke(null, GetRedoDepth());
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

}


//============================================================
// A Node for the HistoryManager to store the history of commands for undo/redo

public class HistoryNode(int id, ICommand command, CommandHistoryType type, HistoryNode parent = null!) : IEnumerable<(HistoryNode Node, int Level)>
{
   public int Id { get; } = id;
   public ICommand Command { get; } = command;
   public HistoryNode Parent { get; } = parent;
   public CommandHistoryType Type { get; } = type;
   public List<HistoryNode> Children { get; } = [];

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