using Editor.DataClasses.GameDataClasses;
using Editor.Loading.Enhanced.PCFL.Scribbel;

namespace Editor.Loading.Enhanced.PCFL
{



   // can also be used in triggers
   public class IfFLowControl : PCFL_Token
   {
      public Trigger Trigger;
      public List<PCFL_Token> SubTokens; // If Effect
      
      public IfFLowControl(Trigger trigger, List<PCFL_Token> subTokens)
      {
         Trigger = trigger;
         SubTokens = subTokens;
      }

      // Targets must be sorted alphanumerically
      public override void Activate(ITarget target)
      {
         //if (SubTokens.Count == 0)
         //   return;

         if (Trigger.Evaluate(target))
            foreach (var token in SubTokens)
               token.Activate(target);

      }
   }
}