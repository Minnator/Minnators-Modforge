using System.Text;
using Editor.Helper;
using Editor.Saving;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Editor.src.Forms.PopUps;


namespace Editor.ErrorHandling
{

   // Attribute to store color for each enum value
   [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
   public class LogColorAttribute : Attribute
   {
      public Color Color { get; }
      public bool BlackColor { get; }

      public LogColorAttribute(string backColor, bool blackColor)
      {
         Color = ColorTranslator.FromHtml(backColor); // Convert HEX to Color
         BlackColor = blackColor;
      }

   }


   [Flags]
   public enum LogType
   {
      None = 0,

      [LogColor("#B00B00", false)] // Red for Error
      Error = 1,

      [LogColor("#DEAD00", false)] // Orange for Warning
      Warning = 2,

      [LogColor("#00BEEF", true)] // Blueish for Info
      Information = 4,

      [LogColor("#DE00AD", true)] // Pink for Debug
      Debug = 8,

      [LogColor("#B000B0", true)]
      Critical = 16,
   }

   public static class LogManager
   {
      public static List<LogEntry> ActiveEntries = [];
      public static List<LogEntry> Informations = [];
      public static List<LogEntry> Warnings = [];
      public static List<LogEntry> Errors = [];
      public static List<LogEntry> Debugs = [];
      public static List<LogEntry> Criticals = [];
      private static LogType _currentLogType = LogType.None;

      public static event EventHandler<LogEntry>? LogEntryAdded;

      public static List<LogEntry> GetAlLogEntries => [..Informations, ..Warnings, ..Errors, ..Debugs, ..Criticals];

      public static void AddLogEntries(List<LogEntry> list)
      {
         lock (ActiveEntries)
         {
            lock (list)
            {
               if (list.Count == 0)
                  return;
               if (ActiveEntries.Count == 0)
               {
                  ActiveEntries.AddRange(list);
                  return;
               }
               var i = 0;
               var j = 0;

               while (i < ActiveEntries.Count && j < list.Count)
               {
                  if (ActiveEntries[i].CompareTo(list[j]) <= 0)
                     i++;
                  else
                  {
                     ActiveEntries.Insert(i, list[j]);
                     j++;
                     i++;
                  }
               }

               while (j < list.Count)
               {
                  ActiveEntries.Add(list[j]);
                  j++;
               }
            }
         }
      }

      public static void RemoveLogEntries(LogType type)
      {
         lock (ActiveEntries)
         {
            ActiveEntries.RemoveAll(x => x.Level.HasFlag(type));
         }
      }

      private static void AddOrRemoveLogEntries(LogType type, LogType delta, LogType newLogType)
      {
         if (delta.HasFlag(type))
            if (newLogType.HasFlag(type))
               AddLogEntries(GetListForType(type));
            else
               RemoveLogEntries(type);
      }

      public static void ChangeVerbosity(LogType logType)
      {
         if (logType == LogType.None)
         {
            lock (ActiveEntries)
            {
               ActiveEntries.Clear();
            }
            return;
         }

         var delta = logType ^ _currentLogType;
         _currentLogType = logType;


         AddOrRemoveLogEntries(LogType.Critical, delta, logType);
         AddOrRemoveLogEntries(LogType.Error, delta, logType);
         AddOrRemoveLogEntries(LogType.Warning, delta, logType);
         AddOrRemoveLogEntries(LogType.Information, delta, logType);
         AddOrRemoveLogEntries(LogType.Debug, delta, logType);
      }

      public static void SaveLogToFile()
      {
         var file = Path.Combine(Globals.Settings.Saving.LogLocation, "LoadingLog.txt");
         var sb = new StringBuilder();
         foreach (var entry in GetAlLogEntries) 
            sb.AppendLine(entry.ToString());

         IO.WriteAllInANSI(file, sb.ToString(), false);
      }

      public static void SaveLogAsCsv()
      {
         var file = Path.Combine(Globals.Settings.Saving.LogLocation, "LoadingLog.csv");
         var sb = new StringBuilder();
         sb.AppendLine("Timestamp,Level,type,Message,source");
         foreach (var entry in GetAlLogEntries)
         {
            if (entry is ErrorObject error)
               sb.AppendLine($"{entry.Timestamp:yyyy-MM-dd HH:mm:ss},{error.Level},{error.ErrorType},{error.Message},{error.Path}");
            else if (entry is FileRefLogEntry fileRef)
               sb.AppendLine($"{entry.Timestamp:yyyy-MM-dd HH:mm:ss},{fileRef.Level},,{fileRef.Message},{fileRef.Path}");
            else
               sb.AppendLine($"{entry.Timestamp:yyyy-MM-dd HH:mm:ss},{entry.Level},,{entry.Message},");
         }

         IO.WriteAllInANSI(file, sb.ToString(), false);
      }

      private static List<LogEntry> GetListForType(LogType type)
      {
         switch (type)
         {
            case LogType.Error:
               return Errors;
            case LogType.Warning:
               return Warnings;
            case LogType.Information:
               return Informations;
            case LogType.Debug:
               return Debugs;
            case LogType.Critical:
               return Criticals;
            default:
               return [];
         }
      }
      private static void AddToList(LogEntry entry)
      {
         var list = GetListForType(entry.Level);
         lock (list)
         {
            list.Add(entry);
         }
      }

      public static void AddLogEntry(LogEntry entry)
      {
         var verbosity = entry.Level;
         if (Globals.Settings.Logging.LoggingVerbosity.HasFlag(verbosity))
         {
            if (_currentLogType.HasFlag(verbosity))
            {
               AddToList(entry);
               lock (ActiveEntries)
               {
                  ActiveEntries.Add(entry);
                  OnLogEntryAdded(entry);
               }
            }

         }
      }

      public static LogEntry Inform(string message)
      {
         return new(LogType.Information, message);
      }

      public static LogEntry Warn(string message)
      {
         return new(LogType.Warning, message);
      }

      public static LogEntry Error(string message)
      {
         return new(LogType.Error, message);
      }

      public static LogEntry Debug(string message)
      {
         return new(LogType.Debug, message);
      }

      public static LogEntry Critical(string message)
      {
         return new(LogType.Critical, message);
      }

      private static void OnLogEntryAdded(LogEntry e)
      {
         LogEntryAdded?.Invoke(null, e);
      }
   }

   public class LogEntry
   {
      public LogEntry(LogType level, string message)
      {
         Level = level;
         Message = message;
         LogManager.AddLogEntry(this);
      }

      public DateTime Timestamp { get; } = DateTime.Now;
      public LogType Level { get; }
      public string Message { get; }

      public override string ToString()
      {
         return $"[{Timestamp:yyyy-MM-dd HH:mm:ss}] [{Level}] {Message}";
      }

      public int CompareTo(LogEntry logEntry)
      {
         return Timestamp.CompareTo(logEntry.Timestamp);
      }
   }

   public class DebugError(string message, ErrorType type)
      : ErrorObject(LogType.Debug, type, message, string.Empty);

   public class FileRefLogEntry : LogEntry
   {
      public string Path { get; }
      public FileRefLogEntry(LogType level, string message, string path) : base(level, message)
      {
         Path = path;
      }

      public void OpenPath()
      {
         if (!ProcessHelper.OpenPathIfFileOrFolder(Path))
         {
            _ = new DebugError($"Could not open \"{Path}\"", ErrorType.ApplicationCouldNotOpenFile);
            ImprovedMessageBox.Show("Unable to open path as file or folder!", "Could not open file", ref Globals.Settings.PopUps.NotifyIfErrorFileCanNotBeOpenedRef, MessageBoxButtons.OK, MessageBoxIcon.Warning);
         }
      }

      public override string ToString()
      {
         return $"[{Timestamp:yyyy-MM-dd HH:mm:ss}] [{Level}] {Message}";
      }
   }

   public class LoadingError : ErrorObject
   {
      public static string GetErrorMsg(string path, int line, int charPos, string msg)
      {
         return line != -1 ? (charPos != -1 ? $"Error in file \"{path}\" on line {line}|{charPos} : {msg}" : $"Error in file \"{path}\" in line {line} : {msg}") : $"Error in file \"{path}\" : {msg}";
      }

      /// <summary>
      /// Parsing Exception
      /// </summary>
      /// <param name="path"></param>
      /// <param name="msg"></param>
      /// <param name="line"></param>
      /// <param name="charPos"></param>
      /// <param name="type"></param>
      /// <param name="level"></param>
      public LoadingError(PathObj path, string msg, int line = -1, int charPos = -1, ErrorType type = ErrorType.SyntaxError,
         LogType level = LogType.Error) : base(level, type, GetErrorMsg(path.ToPath(), line, charPos, msg))
      { }

      /// <summary>
      /// Object Construction Exception
      /// </summary>
      /// <param name="path"></param>
      /// <param name="msg"></param>
      /// <param name="type"></param>
      /// <param name="context"></param>
      public LoadingError(PathObj path, string msg, ErrorType type, ParserRuleContext context) : this(path, msg, context.Start.Line, context.Start.Column, type)
      { }
   }

   public class ErrorObject : FileRefLogEntry, IExtendedLogInformationProvider
   {
      private const string DEFAULT_INFORMATION = "N/A";

      private readonly string Description;
      private readonly string Resolution;
      public ErrorType ErrorType { get; init; }

      public ErrorObject(string message, string resolution, string description, string path = "") : base(LogType.Error, message, path)
      {
         Description = description;
         Resolution = resolution;
      }

      public ErrorObject(string message, ErrorType type, string path = "") : this(LogType.Error, type, message, path)
      {
      }

      protected ErrorObject(LogType level, ErrorType type, string message, string path = "") : base(level, $"{Enum.GetName(type)}: " + message, path)
      {
         (Description, Resolution) = GetErrorInformation(type);
         ErrorType = type;
      }

      private static (string Description, string Resolution) GetErrorInformation(ErrorType type)
      {
         var information = type.GetAttributeOfType<ErrorInformation>();
         return information is null ? (DEFAULT_INFORMATION, DEFAULT_INFORMATION) : (information.Description, information.Resolution);
      }

      public string GetDescription()
      {
         return Description;
      }

      public string GetResolution()
      {
         return Resolution;
      }

      public string GetMessage()
      {
         return Message;
      }
   }

   public interface IExtendedLogInformationProvider
   {
      public string GetDescription();
      public string GetResolution();
      public string GetMessage();
   }
}