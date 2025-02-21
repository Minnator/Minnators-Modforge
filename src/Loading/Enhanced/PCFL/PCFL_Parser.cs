namespace Editor.Loading.Enhanced.PCFL;



/*
  Hirachie:
  EffectOberKlasse EOK: Spezifische Effekte oder Trigger oder generalisierte ScopeSwitches
  
  Trigger(List<EOK>)
  ScopeSwitch(List<EOK>)

  scope country: all_owned_provinces



  ScopeSwitch Province
      TriggerObj ([TriggerIsCore(TUR)])
         EffectObj add_x value = 4


   
   every_owned_province = {
   	limit = {
   		is_core = TUR
   	}
      add_x
   }

 */


// Paradox Clusterfuck Language Parser
/*
public static class PCFL_Parser
{
   private class StackInfo(int index, PCFL_Scope scope, IEnhancedElement[] elements, List<ITarget> targets)
   {
      public StackInfo(int index, PCFL_Scope scope, IEnhancedElement[] elements) : this(index, scope, elements, [])
      {
      }

      public int Index { get; set; } = index;
      public PCFL_Scope Scope { get; } = scope;
      public List<ITarget> Targets { get; set; } = targets;
      public IEnhancedElement[] Elements { get;} = elements;
   }

   public static List<PCFL_EffectBase> EffectsFromElements(IEnumerable<IEnhancedElement> inElements, PCFL_Scope initScope)
   {
      var elements = inElements.ToArray();

      var stack = new Stack<StackInfo>();
      stack.Push(new(0, initScope, elements));



      while (stack.Count > 0)
      {
         var info = stack.Peek();

         var currentElement = elements[info.Index++];

         // resolve scope if needed


         // we have no more elements to work through so we go back to base
         if (info.Index >= elements.Length)
         {
            info.Index = 0;
         }
      }

      return new();
   }
}

*/