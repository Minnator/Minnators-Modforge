using System;
using System.Collections.Generic;
using System.Linq;

namespace Editor.Commands;
#nullable enable
public class HistoryManager
{
   private int _nodeId;
   private HistoryNode _current;
   private HistoryNode _root;

   public HistoryManager(ICommand initial)
   {
      _current = new HistoryNode(_nodeId++, initial, CommandHistoryType.Action);
      _root = _current;
   }


   // Events for when the undo/redo depth changes
   public event EventHandler<int>? UndoDepthChanged;
   public event EventHandler<int>? RedoDepthChanged;
   
   // Add a new command to the history
   public void AddCommand(ICommand newCommand, CommandHistoryType type = CommandHistoryType.Action)
   {
      var newNode = new HistoryNode(_nodeId++, newCommand, type, _current);
      _current.Children.Add(newNode);
      _current = newNode;

      UndoDepthChanged?.Invoke(this, GetUndoDepth());
   }

   // Check if there are any commands to undo or redo
   public bool CanUndo => _current.Parent != null!;

   // Check if there are any commands to redo
   public bool CanRedo => _current.Children.Count > 0;

   // Undo the last command
   public void Undo()
   {
      if (CanUndo)
      {
         _current = _current.Parent;
         _current.Command.Undo();
      }

      UndoDepthChanged?.Invoke(this, GetUndoDepth());
   }

   public void Redo(int childIndex = 0)
   {
      if (CanRedo && childIndex < _current.Children.Count)
      {
         _current = _current.Children[childIndex];
         _current.Command.Redo();
      }

      RedoDepthChanged?.Invoke(this, GetRedoDepth());
   }

   public void RevertTo(int id)
   {
      var (undo, redo) = GetPathBetweenNodes(_current.Id, id);
      RestoreState(undo, redo);
   }

   private void RestoreState(List<HistoryNode> undo, List<HistoryNode> redo)
   {
      foreach (var node in undo) 
         node.Command.Undo(); // Cant use Undo() because it would change the current node

      foreach (var node in redo)
         node.Command.Redo(); // Cant use Redo() because it would change the current node
      
      _current = redo[^1];

      UndoDepthChanged?.Invoke(this, GetUndoDepth());
      RedoDepthChanged?.Invoke(this, GetRedoDepth());
   }

   public int GetUndoDepth()
   {
      var depth = 0;
      var node = _current;
      while (node.Parent != null!)
      {
         depth++;
         node = node.Parent;
      }

      return depth;
   }

   public int GetRedoDepth()
   {
      var depth = 0;
      var node = _current;
      while (node.Children.Count > 0)
      {
         depth++;
         node = node.Children[0];
      }

      return depth;
   }

   public ICommand GetCurrentCommand() => _current.Command;

   public (List<HistoryNode>, List<HistoryNode>) GetPathBetweenNodes(int from, int to)
   {
      // Cant handle edgecase if from == to
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

   private bool GetPath(HistoryNode subRoot, List<HistoryNode> path, int id)
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

   public HistoryNode GetRoot()
   {
      return _root;
   }

   public void Clear()
   {
      _nodeId = 0;
      _root = _current = new HistoryNode(_nodeId++, new CInitial(), CommandHistoryType.Action);
      _current = _root;
      UndoDepthChanged?.Invoke(this, GetUndoDepth());
      RedoDepthChanged?.Invoke(this, GetRedoDepth());
   }
}


//============================================================
// A Node for the HistoryManager to store the history of commands for undo/redo

public class HistoryNode(int id, ICommand command, CommandHistoryType type, HistoryNode parent = null!)
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


}