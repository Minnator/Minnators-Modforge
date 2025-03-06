namespace Editor.Loading.Enhanced.PCFL.Implementation;

public class IfFLowControl : IToken, ITrigger
{
   public ITrigger Trigger;
   public List<IToken> SubTokens; // If Effect

   public IfFLowControl(ITrigger trigger, List<IToken> subTokens)
   {
      Trigger = trigger;
      SubTokens = subTokens;
   }

   // Targets must be sorted alphanumerically
   public void Activate(ITarget target)
   {
      if (SubTokens.Count == 0)
         return;

      if (Trigger.Evaluate(target))
         foreach (var token in SubTokens)
            token.Activate(target);

   }

   public bool Evaluate(ITarget target)
   {
      throw new NotImplementedException();
   }
}