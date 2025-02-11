using Editor.ErrorHandling;

namespace Editor.DataClasses.GameDataClasses
{
   public class ReligiousGroup(string name)
   {
      public string Name { get; set; } = name;
      public List<Religion> Religions { get; set; } = [];
   }

   public class Religion(string name)
   {
      public string Name { get; set; } = name;
      public Color Color { get; set; } = Color.Empty;

      public static Religion Empty { get; } = new(string.Empty)
      {
         Color = Color.DimGray
      };

      public override string ToString()
      {
         return Name;
      }

      public static IErrorHandle GeneralParse(string? str, out object result)
      {
         var handle = TryParse(str, out var culture);
         result = culture;
         return handle;
      }

      public static IErrorHandle TryParse(string input, out Religion religion)
      {
         if (string.IsNullOrEmpty(input))
         {
            religion = Empty;
            return new ErrorObject(ErrorType.TypeConversionError, "Could not parse religion!", addToManager: false);
         }

         if (!Globals.Religions.TryGetValue(input, out religion!))
         {
            religion = Empty;
            return new ErrorObject(ErrorType.UnresolveableReligionReference, $"Religion \"{input}\" was not defined!",
               addToManager: false);
         }

         return ErrorHandle.Success;
      }
   }
}