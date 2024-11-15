using System.Text;

namespace Editor.Formatters
{
   public interface IFormatter
   {
      void Format(StringBuilder sb, object obj);

      public void Format(StringBuilder sb, List<KeyValuePair<string, object>> toFormat);
   }
}