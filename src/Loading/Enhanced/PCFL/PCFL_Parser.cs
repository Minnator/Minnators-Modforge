using Editor.ErrorHandling;
using Editor.Helper;
using Editor.Loading.Enhanced.PCFL.Scribbel;
using Editor.Saving;

namespace Editor.Loading.Enhanced.PCFL;

public static class PCFL_TriggerParser
{
   // Boolean Methods
   public static IErrorHandle ParseTriggerReplace(string value, out bool isReplace, out bool outType, out string? outReplace)
   {
      outReplace = null;
      isReplace = false;
      outType = false;
      
      var error = IsValueReplaceable(value, out isReplace, out outReplace);
      if (!error.Ignore())
         return error;

      return ParseTrigger(value, out outType);
   }

   public static IErrorHandle ParseTrigger(string value, out bool outType)
   {
      var handle = Converter.ParseBool(value, out var boolObj);
      outType = (bool)boolObj;
      return handle;
   }

   // Integer Methods
   public static IErrorHandle ParseTriggerReplace(string value, out bool isReplace, out int outType, out string? outReplace)
   {
      outReplace = null;
      isReplace = false;
      outType = 0;
      
      var error = IsValueReplaceable(value, out isReplace, out outReplace);
      if (!error.Ignore())
         return error;

      return ParseTrigger(value, out outType);
   }

   public static IErrorHandle ParseTrigger(string value, out int outType)
   {
      var handle = Converter.ParseInt(value, out var intObj);
      outType = (int)intObj;
      return handle;
   }

   // Float Methods
   public static IErrorHandle ParseTriggerReplace(string value, out bool isReplace, out float outType, out string? outReplace)
   {
      outReplace = null;
      isReplace = false;
      outType = 0f;
      
      var error = IsValueReplaceable(value, out isReplace, out outReplace);
      if (!error.Ignore())
         return error;

      return ParseTrigger(value, out outType);
   }

   public static IErrorHandle ParseTrigger(string value, out float outType)
   {
      var handle = Converter.ParseFloat(value, out var floatObj);
      outType = (float)floatObj;
      return handle;
   }

   // String Methods
   public static IErrorHandle ParseTriggerReplace(string value, out bool isReplace, out string outType, out string? outReplace)
   {
      outReplace = null;
      isReplace = false;
      outType = string.Empty;
      
      var error = IsValueReplaceable(value, out isReplace, out outReplace);
      if (!error.Ignore())
         return error;

      return ParseTrigger(value, out outType);
   }

   public static IErrorHandle ParseTrigger(string value, out string outType)
   {
      return EnhancedParser.IsValidString(value, out outType);
   }


   // Helper Methods
   public static IErrorHandle IsValueReplaceable(string value, out bool isReplace, out string? outReplace)
   {
      outReplace = null;
      isReplace = false;

      if (value.StartsWith('$') && value.EndsWith('$'))
      {
         if (value.Length < 3)
            return new ErrorObject(ErrorType.TempParsingError, "Invalid replace value. Must not be empty", addToManager: false);
         outReplace = value[1..^1];
         isReplace = true;
      }
      return ErrorHandle.Success;
   }
}



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