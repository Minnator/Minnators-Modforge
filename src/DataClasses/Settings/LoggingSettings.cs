using System.ComponentModel;
using Editor.ErrorHandling;

namespace Editor.DataClasses.Settings
{
   public class LoggingSettings : SubSettings
   {
      private LogType _loggingVerbosity = LogType.Information | LogType.Warning | LogType.Error | LogType.Critical;


      [Description("Defines which which severity of errors will be logged")]
      [CompareInEquals]
      public LogType LoggingVerbosity
      {
         get => _loggingVerbosity;
         set => SetField(ref _loggingVerbosity, value);
      }
   }
}