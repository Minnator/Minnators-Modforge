using System.Diagnostics;
using Editor.Saving;

namespace Editor.DataClasses.Commands
{
   public class CompactingCommand : ICommand
   {
      private HistoryNode? _startNode;
      private HistoryNode? _endNode;
      private bool _isCompacted;



      public void Compact()
      {
         if (_isCompacted)
            return;

         if (_startNode == null || _endNode == null)
            return;
         
         if (!HistoryManager.EnsureLinearity(_startNode, _endNode, Globals.Settings.Misc.MaxCompactingSize))
         {
            MessageBox.Show("Compacting failed, the history is not linear enough!");
            return;
         }
         _isCompacted = true;
      }

      public void StartCompacting(HistoryNode start)
      {
         _startNode = start;
         _isCompacted = false;
      }

      public void EndCompacting(HistoryNode end)
      {
         _endNode = end;
         _isCompacted = false;
         Compact();
      }
      public List<Saveable> GetTargets() => [];

      public void Execute()
      {
         if (!_isCompacted)
            return;

         while (_startNode!.Children.Count > 0)
            foreach (var child in _startNode.Children)
               if (child.Id <= _endNode!.Id)
                  child.Command.Execute();
               else
                  break;
      }

      public void Undo()
      {
         if (!_isCompacted)
            return;

         var current = _endNode;
         while (current != _startNode)
         {
            current!.Command.Undo();
            current = current.Parent;
         }
      }

      public void Redo()
      {
         Execute();
      }

      public string GetDescription()
      {
         return $"Compacting {_endNode?.Id - _startNode?.Id} nodes";
      }

      public string GetDebugInformation(int indent)
      {
         return $"Compacting {_endNode?.Id - _startNode?.Id} nodes";
      }
   }
}