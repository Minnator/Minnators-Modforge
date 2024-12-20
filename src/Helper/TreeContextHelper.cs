using Antlr4.Runtime.Tree;

namespace Editor.Helper
{
   public static class TreeContextHelper
   {
      public static int[] GetIntFromContext(ITerminalNode[] nodes)
      {
         return nodes.Select(x => int.Parse(x.GetText())).ToArray();
      }
   }
}