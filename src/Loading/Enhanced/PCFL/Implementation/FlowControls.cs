using System.Text;

namespace Editor.Loading.Enhanced.PCFL.Implementation;

public class IfFLowControl : IToken, ITrigger
{
   private const string EFFECT_DESCRIPTION = "Only executes the effects if the triggers are met.";
   private const string EFFECT_EXAMPLE = "if = {\n\tlimit = {\n\t\t<triggers>\n\t}\n\t<effects>\n}";
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

   public void GetTokenString(int tabs, ref StringBuilder sb)
   {
      throw new NotImplementedException();
   }

   public string GetTokenName() => "if";
   public string GetTokenDescription() => EFFECT_DESCRIPTION;

   public string GetTokenExample() => EFFECT_EXAMPLE;

   public bool Evaluate(ITarget target)
   {
      throw new NotImplementedException();
   }
}