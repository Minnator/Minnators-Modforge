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
   None = 0,
   [ErrorInformation("This occurs because an file was not found but referenced elsewhere!", "Create the file missing file!")]
   FileNotFound = 1, // File errors 1-99
   [ErrorInformation("This occurs because an file could not be opened or read!", "Check the file path and permissions! Also make sure the file is not used by another process")]
   FileCouldNotBeOpened = 2,
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
   [ErrorInformation("This occurs because a reference to a province could not be resolved!", "Is the given province defined in definition.csv?")]
   UnresolveableProvinceReference = 102,
   [ErrorInformation("This occurs because more content elements where found than allowed.", "Are there any bracket mistakes in the file?")]
   UnexpectedContentElement = 103,
   [ErrorInformation("This occurs because more block elements where found than allowed.", "Are there any bracket mistakes in the file?")]
   UnexpectedBlockElement = 104,
   [ErrorInformation("This occurs because the block name is not valid in the context of its usage!", "Check the block name for typos, and the surrounding blocks for bracket mistakes")]
   IllegalBlockName = 105,
   [ErrorInformation("This occurs because the data type is not expected in its context!", "Check the data type and correct it.")]
   UnexpectedDataType = 106,
   [ErrorInformation("This occurs because an element is already defined and can not be defined again!", "Check the file for duplicate elements and remove or rename them!")]
   DuplicateElement = 107,
   [ErrorInformation("This occurs because a province is defined multiple times or its color is not unique!", "A province has no pixels in provinces.bmp. Either remove it from definition.csv or give it pixels.")]
   ProvinceDefinitionError = 108,
   [ErrorInformation("This occurs because a reference to an area could not be resolved!", "Is the given area defined in area.txt?")]
   UnresolveableAreaReference = 109,
   [ErrorInformation("This occurs because an object is defined multiple times!", "Check the file for duplicate object definitions and remove or rename them!")]
   DuplicateObjectDefinition = 110,
   [ErrorInformation("This occurs because a reference to a region could not be resolved!", "Is the given region defined in region.txt?")]
   UnresolveableRegionReference = 111,
   [ErrorInformation("This occurs because an attribute is defined multiple times!", "Check the file for duplicate attribute definitions and ensure they are defined uniquely!")]
   DuplicateAttributeDefinition = 112,
   [ErrorInformation("This occurs because a value could not be converted to the expected type!", "Check the value and correct it!")]
   TypeConversionError = 113,
   [ErrorInformation("This occurs because the map size is not valid!", "Are the size defined in default.map and the provinces.bmp file the same?")]
   InvalidMapSize = 114,
   [ErrorInformation("This occurs because a province id is higher than the max_provinces value in default.map or less than 1!", "Check the province id and correct it!")]
   InvalidProvinceId = 115,
   [ErrorInformation("This occurs because a country tag is used but never defined!", "Check the country tag definition and usages of the affected tag!")]
   UndefinedCountryTag = 116,
   [ErrorInformation("This occurs because a religion is used but never defined!", "Check the religion definition and usages of the affected religion!")]
   UnresolveableReligionReference = 117,
   [ErrorInformation("An attribute is found, where it is not recognized!", "Check the object and make sure all its attributes are valid")]
   UnexpectedAttribute = 118,
   [ErrorInformation("This occurs because a trade good is used but never defined!", "Check the trade good definition and usages of the affected trade good!")]
   UnresolveableTradeGoodReference = 119,
   [ErrorInformation("This occurs because a monarch name is misformed!", "Is a regnal number defined?")]
   MisformedMonarchName = 120,
   [ErrorInformation("This occurs because a file has an invalid structure!", "Check the file and it's structure and correct it!")]
   InvalidFileStructure = 121,
   [ErrorInformation("This occurs because a value is not in a valid range for the given target type!", "Check the value and correct it!")]
   InvalidValue = 122,
   [ErrorInformation("This occurs because an object is not found!", "Check the object, its naming and references and correct it if necessary!")]
   ObjectNotFound = 123,
   [ErrorInformation("This occurs as some trigger has been called or created with illegal arguments", "Verify trigger attributes and usage")]
   PCFL_TriggerValidationError = 124,
   [ErrorInformation("This occurs because a trigger is empty and can be removed", "Check the trigger and remove if necessary")]
   EmptyLimitBlock = 135,
   [ErrorInformation("This occurs because an date is not in a valid format!", "Check the date format and correct it!")]
   IllegalDateFormat = 101,
   // ObjectInteraction Error
   [ErrorInformation("This occurs because a key was not found in a dictionary!", "Contact a developer with the latest log exported as a .csv")]
   INTERNAL_KeyNotFound = 300, // ObjectInteraction Error 300-399
   [ErrorInformation("An uninitialized object is being referenced!", "Create the object before using it!")]
   INTERNAL_UsageOfUndefinedObject = 301,
   // Misc Error 400-599
   [ErrorInformation("This occurs because given file has an unsupported file ending", "Choose a different file.")]
   NotSupportedAudioExtension = 600, // Audio Error 600-649
   [ErrorInformation("This occurs because your file is not in the correct format and thus can not be parse by the modforge", "Verify that your mod file is formatted correctly and if so contact a developer!")]
   TempParsingError = 1000,
   [ErrorInformation("This occurs because the error is not yet implemented!", "Implement the error! :)")]
   TODO_ERROR = 9999,
}

