using System.Diagnostics;
using System.Reflection;
using Editor.DataClasses.GameDataClasses;
using Editor.ErrorHandling;
using Editor.Loading.Enhanced;
using Editor.Saving;

namespace Editor.Helper
{

   public static class Converter
   {
      private static readonly Dictionary<Type, TryParseDelegate> _conversionDict = new()
      {
         {
            typeof(int),
            ParseInt
         },
         {
            typeof(float),
            ParseFloat
         },
         {
            typeof(double),
            ParseDouble
         },
         {
            typeof(bool),
            ParseBool
         },
         {
            typeof(string),
            ParseString
         },
         {
            typeof(Tag),
            Tag.GeneralParse
         },
         {
            typeof(Province),
            Province.GeneralParse
         },
         {
            typeof(Culture),
            Culture.GeneralParse
         }
      };

      private static IErrorHandle ParseInt(string value, out object result)
      {
         int result1;
         if (!int.TryParse(value, out result1))
         {
            result = 0;
            return new ErrorObject(ErrorType.TypeConversionError, "Could not parse int!");
         }

         result = result1;
         return ErrorHandle.Sucess;
      }

      private static IErrorHandle ParseFloat(string value, out object result)
      {
         float result1;
         if (!float.TryParse(value, out result1))
         {
            result = 0.0f;
            return new ErrorObject(ErrorType.TypeConversionError, "Could not parse float!",
               addToManager: false);
         }

         result = result1;
         return ErrorHandle.Sucess;
      }

      private static IErrorHandle ParseDouble(string value, out object result)
      {
         double result1;
         if (!double.TryParse(value, out result1))
         {
            result = 0.0;
            return new ErrorObject(ErrorType.TypeConversionError, "Could not parse double!",
               addToManager: false);
         }

         result = result1;
         return ErrorHandle.Sucess;
      }

      private static IErrorHandle ParseBool(string value, out object result)
      {
         bool result1;
         if (!bool.TryParse(value, out result1))
         {
            switch (value)
            {
               case "yes":
                  result = true;
                  return ErrorHandle.Sucess;
               case "no":
                  result = false;
                  return ErrorHandle.Sucess;
               default:
                  result = false;
                  return new ErrorObject(ErrorType.TypeConversionError, "Could not parse bool!",
                     addToManager: false);
            }
         }
         else
         {
            result = result1;
            return ErrorHandle.Sucess;
         }
      }

      private static IErrorHandle ParseString(string value, out object result)
      {
         result = value;
         return ErrorHandle.Sucess;
      }

      public static IErrorHandle Convert<T>(string? str, out object value)
      {
         Debug.Assert(_conversionDict.ContainsKey(typeof(T)), $"\"{typeof(T)}\" is not yet supported in the Converter! Add to Dictionary!");
         IErrorHandle errorHandle = _conversionDict[typeof(T)](str, out var result);
         value = result;
         return errorHandle;
      }

      public static IErrorHandle Convert<T>(string? str, PropertyInfo info, out T value)
      {
         Debug.Assert(_conversionDict.ContainsKey(typeof(T)), $"\"{typeof(T)}\" is not yet supported in the Converter! Add to Dictionary!");
         IErrorHandle errorHandle = _conversionDict[typeof(T)](str, out var result);
         value = (T)result;
         return errorHandle;
      }

      public static IErrorHandle Convert<T>(string value, out T output)
      {
         Debug.Assert(_conversionDict.ContainsKey(typeof(T)), $"\"{typeof(T)}\" is not yet supported in the Converter! Add to Dictionary!");
         IErrorHandle errorHandle = _conversionDict[typeof(T)](value, out var result);
         output = (T)result;
         return errorHandle;
      }

      private delegate IErrorHandle TryParseDelegate(string value, out object result);
   }
}