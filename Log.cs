using System.IO;

namespace Editor;

public class Log(string path, string logName)
{

   private readonly StreamWriter _logFile = new(Path.Combine(path, logName + ".txt"));

   public void Write(string message)
   {
      _logFile.WriteLine(message);
      _logFile.Flush();
   }

   public void WriteTimeStamp(string operation, long milliseconds, char fillCharacter = '.')
   {
      _logFile.WriteLine($"{operation.PadRight(40, fillCharacter)} [{milliseconds.ToString().PadLeft(6, fillCharacter)}]ms");
      _logFile.Flush();
   }
}
