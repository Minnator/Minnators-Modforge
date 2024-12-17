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
   [ErrorInformation("This is caused by months outside the bounds of a year or by days outside the bounds of a month", "Check the date and correct it!")]
   IllegalDate = 100, // Parsing errors 100-299
   [ErrorInformation("This occurs because an date is not in a valid format!", "Check the date format and correct it!")]
   IllegalDateFormat = 101,

}

