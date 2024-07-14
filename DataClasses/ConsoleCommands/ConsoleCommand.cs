namespace Editor.DataClasses.ConsoleCommands;

public abstract class ConsoleCommand
{
   public abstract string Command { get; }
   public abstract string Description { get; }
   public abstract string Help { get; }
   public abstract string[] Aliases { get; }

   public virtual RichTextBox Output => Globals.ConsoleForm!.Output;
   public abstract void Execute(string[] args);

   public virtual void ExecuteHelp()
   {
      Output.AppendText($"->: {Help}\n");
   }

   public virtual void ExecuteDescription()
   {
      Output.AppendText($"->: {Description}\n");
   }

   public virtual void ExecuteAliases()
   {
      Output.AppendText($"->: Aliases [{string.Join(", ", Aliases)}]\n");
   }

   public virtual bool CheckArgs(string[] args)
   {
      return true;
   }

   public virtual bool CheckArgCount(string[] args)
   {
      return true;
   }

   public virtual bool CheckArg(string arg)
   {
      return true;
   }

   public virtual void OnExecuteSuccess()
   {
   }

   public virtual void OnExecuteFail()
   {
   }

   public virtual void OnExecuteError(Exception e)
   {
   }

   public virtual void OnExecuteWarning()
   {
   }


}