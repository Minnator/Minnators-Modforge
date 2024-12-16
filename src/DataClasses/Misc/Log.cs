using System.Diagnostics;

namespace Editor.DataClasses.Misc;

public class Log
{
   private int _logCount;
   private int _totalTime;
   private readonly StreamWriter _logFile;
   private readonly object _lock = new();

   public Log(string path, string logName)
   {
      if (!Directory.Exists(path))
         Directory.CreateDirectory(path);
      _logFile = new (Path.Combine(path, $"{logName}.txt"));
   }

   public void Write(string message)
   {
      lock (_lock)
      {
         if (string.IsNullOrWhiteSpace(message))
            Debugger.Break();
         _logFile.WriteLine(message);
         _logFile.Flush();
      }

   }

   public void WriteTimeStamp(string operation, long milliseconds, char fillCharacter = '.')
   {
      lock (_lock)
      {
         _logFile.WriteLine($"{operation.PadRight(40, fillCharacter)} [{milliseconds.ToString().PadLeft(6, fillCharacter)}]ms");
         _logFile.Flush();
         _logCount++;
         _totalTime += (int)milliseconds;
      }
   }

   public void Close()
   {
      _logFile.WriteLine("---------------------------------------------------");
      WriteTimeStamp("Total time", _totalTime);
      WriteTimeStamp("Total operations", _logCount);
      _logFile.Flush();
      _logFile.Close();
   }
}
