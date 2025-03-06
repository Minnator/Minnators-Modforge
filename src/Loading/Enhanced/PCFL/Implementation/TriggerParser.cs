using Editor.ErrorHandling;
using Editor.Helper;

namespace Editor.Loading.Enhanced.PCFL.Implementation;

public static class TriggerParser
{
   public static IErrorHandle ParseTriggerOfValueReplace(string value, out bool isReplace, out bool outType, out string? outReplace)
   {
      outReplace = null;
      isReplace = false;
      outType = false;

      var error = IsValueReplaceable(value, out isReplace, out outReplace);
      if (!error.Ignore())
         return error;

      return ParseTriggerOfValue(value, out outType);
   }

   public static IErrorHandle ParseTriggerOfValue(string value, out bool outType)
   {
      var handle = Converter.ParseBool(value, out var boolObj);
      outType = (bool)boolObj;
      return handle;
   }

   // Integer Methods
   public static IErrorHandle ParseTriggerOfValueReplace(string value, out bool isReplace, out int outType, out string? outReplace)
   {
      outReplace = null;
      isReplace = false;
      outType = 0;

      var error = IsValueReplaceable(value, out isReplace, out outReplace);
      if (!error.Ignore())
         return error;

      return ParseTriggerOfValue(value, out outType);
   }

   public static IErrorHandle ParseTriggerValues(string value, PCFL_Type type, out Value outValue)
   {
      IErrorHandle handle;
      switch (type)
      {
         case PCFL_Type.Int:
            handle = ParseTriggerOfValue(value, out int intObj);
            outValue = new Value<int>(intObj);
            break;
         case PCFL_Type.Float:
            handle = ParseTriggerOfValue(value, out float floatObj);
            outValue = new Value<float>(floatObj);
            break;
         case PCFL_Type.String:
            handle = ParseTriggerOfValue(value, out string stringObj);
            outValue = new Value<string>(stringObj);
            break;
         case PCFL_Type.Bool:
            handle = ParseTriggerOfValue(value, out bool boolObj);
            outValue = new Value<bool>(boolObj);
            break;
         default:
            throw new ArgumentOutOfRangeException(nameof(type), type, null);
      }

      return handle;
   }

   public static IErrorHandle ParseTriggerOfValue(string value, out int outType)
   {
      var handle = Converter.ParseInt(value, out var intObj);
      outType = (int)intObj;
      return handle;
   }

   // Float Methods
   public static IErrorHandle ParseTriggerOfValueReplace(string value, out bool isReplace, out float outType, out string? outReplace)
   {
      outReplace = null;
      isReplace = false;
      outType = 0f;

      var error = IsValueReplaceable(value, out isReplace, out outReplace);
      if (!error.Ignore())
         return error;

      return ParseTriggerOfValue(value, out outType);
   }

   public static IErrorHandle ParseTriggerOfValue(string value, out float outType)
   {
      var handle = Converter.ParseFloat(value, out var floatObj);
      outType = (float)floatObj;
      return handle;
   }

   // String Methods
   public static IErrorHandle ParseTriggerOfValueReplace(string value, out bool isReplace, out string outType, out string? outReplace)
   {
      outReplace = null;
      isReplace = false;
      outType = string.Empty;

      var error = IsValueReplaceable(value, out isReplace, out outReplace);
      if (!error.Ignore())
         return error;

      return ParseTriggerOfValue(value, out outType);
   }

   public static IErrorHandle ParseTriggerOfValue(string value, out string outType)
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