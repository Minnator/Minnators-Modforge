using System.Runtime.CompilerServices;
using System.Text;
using Antlr4.Runtime;
using Editor.Helper;
using Editor.Saving;


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
         sb.AppendLine("Timestamp,Level,type,Message,source,CallerInfo");
         foreach (var entry in GetAlLogEntries)
         {
            if (entry is ErrorObject error)
               sb.AppendLine($"{entry.Timestamp:yyyy-MM-dd HH:mm:ss},{error.Level},{error.ErrorType},{error.Message},,{error.DebugInformation}");
            else if (entry is LoadingError loadingError)
               sb.AppendLine($"{entry.Timestamp:yyyy-MM-dd HH:mm:ss},{loadingError.Level},{loadingError.ErrorType},{loadingError.Message},{loadingError.Path},{loadingError.DebugInformation}");
            else
               sb.AppendLine($"{entry.Timestamp:yyyy-MM-dd HH:mm:ss},{entry.Level},,{entry.Message},,{entry.DebugInformation}");
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
      public LogEntry(LogType level, string message, string debugInformation = "", bool addToManager = true, bool isVanilla = false)
      {
         Level = level;
         Message = message;
         IsVanilla = isVanilla;
         DebugInformation = debugInformation;
         if (addToManager)
            LogManager.AddLogEntry(this);
      }

      public DateTime Timestamp { get; } = DateTime.Now;
      public LogType Level { get; }
      public string Message { get; }
      public bool IsVanilla { get; } = false;
      public string DebugInformation { get; } = "";

      public override string ToString()
      {
         return $"[{Timestamp:yyyy-MM-dd HH:mm:ss}] [{Level}] {Message}";
      }

      public int CompareTo(LogEntry logEntry)
      {
         return Timestamp.CompareTo(logEntry.Timestamp);
      }
   }

   public class LoadingError : ErrorObject // TODO remove FileRefLogEntry
   {
      public readonly int Line;
      public readonly int CharPos;
      public readonly string Path;
      public static string GetErrorMsg(string path, int line, int charPos, string msg)
      {
         return line != -1 ? (charPos != -1 ? $"Error in file \"{path}\" on line {line}|{charPos} : {msg}" : $"Error in file \"{path}\" in line {line} : {msg}") : $"Error in file \"{path}\" : {msg}";
      }

      /// <summary>
      /// Parsing Exception
      /// </summary>
      public LoadingError(PathObj path,
                          string msg,
                          int line = -1,
                          int charPos = -1,
                          ErrorType type = ErrorType.SyntaxError,
                          LogType level = LogType.Error,
                          bool addToManager = true,
                          [CallerMemberName] string memberName = "",
                          [CallerFilePath] string callerPath = "",
                          [CallerLineNumber] int lineNumber = 0) 
         : base(type, GetErrorMsg(path.ToModPath(), line, charPos, msg), level, addToManager, !path.IsModPath, memberName, callerPath, lineNumber)
      {
         Line = line;
         CharPos = charPos;
         Path = path.GetPath();
      }

      /// <summary>
      /// Object Construction Exception
      /// </summary>
      /// <param name="path"></param>
      /// <param name="msg"></param>
      /// <param name="type"></param>
      /// <param name="context"></param>
      public LoadingError(PathObj path,
                          string msg,
                          ErrorType type,
                          ParserRuleContext context,
                          [CallerMemberName] string memberName = "",
                          [CallerFilePath] string callerPath = "") : this(path, msg, context.Start.Line, context.Start.Column, type, memberName: memberName, callerPath: callerPath)
      {
      }

   }

   public interface IErrorHandle
   {
      public bool Ignore();
      public bool Log();
      public bool Then(Action<ErrorObject> action);
   }

   public class ErrorObject : LogEntry, IExtendedLogInformationProvider, IErrorHandle
   {
      private const string DEFAULT_INFORMATION = "N/A";
      private readonly string Description;
      private readonly string Resolution;
      private bool IsErrorHandled;
      public ErrorType ErrorType { get; init; }

      private ErrorObject(string message,
                          string resolution,
                          string description,
                          LogType level = LogType.Error,
                          bool addToManager = true,
                          bool isVanilla = false,
                          [CallerMemberName] string memberName = "",
                          [CallerFilePath] string callerPath = "",
                          [CallerLineNumber] int lineNumber = 0) :
         base(level, message, GetDebugInformationWithLocation(memberName, callerPath, lineNumber), addToManager, isVanilla)
      {
         IsErrorHandled = addToManager;
         Description = description;
         Resolution = resolution;
         ErrorType = ErrorType.None;
      }

         public ErrorObject(ErrorType type,
                            string message,
                            LogType level = LogType.Error,
                            bool addToManager = true,
                            bool isVanilla = false,
                            [CallerMemberName] string memberName = "",
                            [CallerFilePath] string callerPath = "",
                            [CallerLineNumber] int lineNumber = 0) :
         base(level, $"{Enum.GetName(type)}: " + message, GetDebugInformationWithLocation(memberName, callerPath, lineNumber), addToManager, isVanilla)
      {
         IsErrorHandled = addToManager;
         (Description, Resolution) = GetErrorInformation(type);
         ErrorType = type;
      }

      public static string GetDebugInformationWithLocation(string memberName = "", string filePath = "", int lineNumber = 0)
      {
         return $"[{filePath}:{lineNumber}] {memberName}";
      }

      private static (string Description, string Resolution) GetErrorInformation(ErrorType type)
      {
         var information = type.GetAttributeOfType<ErrorInformation>();
         return information is null ? (DEFAULT_INFORMATION, DEFAULT_INFORMATION) : (information.Description, information.Resolution);
      }
      

      public bool Then(Action<ErrorObject> action)
      {
         action(this);
         return false;
      }

      public void ConvertToLoadingError(PathObj path, string msg, int line = -1, int charPos = -1, ErrorType? type = null,
         LogType? level = null)
      {
         string newMsg;
         if (!string.IsNullOrEmpty(msg))
            newMsg = msg + '{' + Message + '}';
         else
            newMsg = '{' + Message + '}';
         _ = new LoadingError(path, newMsg, line, charPos, type ?? ErrorType, level ?? Level);
      }
      public string GetDescription() => Description;
      public string GetResolution() => Resolution;
      public string GetMessage() => Message;
      public bool Ignore() => false;
      public bool Log()
      {
         if (IsErrorHandled)
            return false;
         IsErrorHandled = true;
         LogManager.AddLogEntry(this);
         return false;
      }
   }

   public class ErrorHandle : IErrorHandle
   {
      
      private ErrorHandle() { }

      public static ErrorHandle Success = new ();

      public bool Ignore()
      {
         return true;
      }

      public bool Log()
      {
         return true;
      }

      public bool Then(Action<ErrorObject> action)
      {
         return true;
      }
   }

   public interface IExtendedLogInformationProvider
   {
      public string GetDescription();
      public string GetResolution();
      public string GetMessage();
   }
}