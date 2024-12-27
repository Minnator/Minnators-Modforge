using Editor.Saving;

namespace Editor.ErrorHandling;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public class ErrorInformation(string description, string resolution) : Attribute
{
   public readonly string Description = description;
   public readonly string Resolution = resolution;
}

public enum ErrorType
{
   [ErrorInformation("This occurs because an file was not found but referenced elsewhere!", "Create the file missing file!")]
   FileNotFound = 1, // File errors 1-99
   [ErrorInformation("This occurs because an file could not be opened in Explorer!", $"Is {nameof(PathObj)} returning an invalid path?")]
   ExplorerCouldNotOpenFolder = 2,
   [ErrorInformation("This occurs because an file could not be opened by the default application!", $"Is {nameof(PathObj)} returning an invalid path?\nIs the Process started correctly?")]
   ApplicationCouldNotOpenFile = 3,
   [ErrorInformation("This occurs because a file which also exists in vanilla could not be found in the mod or in vanilla!", "Verify Game files to make sure the vanilla file is accessible or ensure the file in your mod is named correctly")]
   RequiredFileNotFound = 5,
   [ErrorInformation("This is caused by months outside the bounds of a year or by days outside the bounds of a month", "Check the date and correct it!")]
   IllegalDate = 100, // Parsing errors 100-299
   [ErrorInformation("This occurs because the syntax of the file is not correct.", "Check the file for any typos. The message contains the location of the issue and possible solutions!")]
   SyntaxError = 101,
   [ErrorInformation("This occurs because an date is not in a valid format!", "Check the date format and correct it!")]
   IllegalDateFormat = 101,
   [ErrorInformation("This occurs because your file is not in the correct format and thus can not be parse by the modforge", "Verify that your mod file is formatted correctly and if so contact a developer!")]
   // Misc Error 300-399
   TempParsingError = 1000,
   [ErrorInformation("This occurs because the error is not yet implemented!", "Implement the error! :)")]
   TODO_ERROR = 9999,
}

