using Editor.DataClasses.GameDataClasses;

namespace Editor.Formatters
{
   public enum BraceStyle
   {
      Base,
      NewLine,
   }

   public interface IFormatter
   {
      public string Format(Province input);
   }
}