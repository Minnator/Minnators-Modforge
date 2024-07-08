using System;
using System.Windows.Forms;

namespace Editor.DataClasses.ConsoleCommands;

public abstract class ConsoleCommand
{
   public abstract string Command { get; }
   public abstract string Description { get; }
   public abstract string Help { get; }
   public abstract string[] Aliases { get; }

   public virtual RichTextBox Output => Globals.ConsoleForm!.Output;
   public abstract void Execute(string[] args);

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