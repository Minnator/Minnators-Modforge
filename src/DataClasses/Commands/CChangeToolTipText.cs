using Editor.Saving;

namespace Editor.DataClasses.Commands;

public class CChangeToolTipText : ICommand
{
   private readonly string _oldToolTip;
   private readonly string _newToolTip;

   public CChangeToolTipText(string oldToolTip, string newToolTip, bool executeOnInit = true)
   {
      _oldToolTip = oldToolTip;
      _newToolTip = newToolTip;

      if (executeOnInit)
         Execute();
   }

   public void Execute()
   {
      Globals.Settings.ToolTip.ToolTipText = _newToolTip;
   }

   public void Undo()
   {
      Globals.Settings.ToolTip.ToolTipText = _oldToolTip;
   }

   public void Redo()
   {
      Execute();
   }

   public List<int> GetTargetHash()
   {
      return [Globals.Settings.ToolTip.GetHashCode()];
   }

   public string GetDescription()
   {
      return $"Modified Tooltip string by [{_oldToolTip.Length - _newToolTip.Length}] chars";
   }

   public string GetDebugInformation(int indent)
   {
      return $"Changed Tooltip from \"{_oldToolTip}\" to \"{_newToolTip}\"";
   }
}