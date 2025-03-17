using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace Editor.DataClasses.Settings
{
   [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
   public class SavingSettings : SubSettings
   {
      public enum SaveOnExitType
      {
         Discard,
         AskToSave,
         Save
      }

      private bool _alwaysAskBeforeCreatingFiles = true;
      private FileSavingMode _fileSavingMode = FileSavingMode.AskOnce;
      private bool _playCrashSound = true;
      private string _logLocation = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
      private string _missionExportLocation = Path.Combine(Globals.AppDataPath, "MissionExports");
      private string _customWordsLocation = string.Empty;
      private SaveOnExitType _saveOnExit = SaveOnExitType.AskToSave;
      private FormattingSettings _formatting = new();

      [Description("<true> Asks for a filename or location beofre creating a new file\n<false> creates files with default names")]
      [CompareInEquals]
      public bool AlwaysAskBeforeCreatingFiles
      {
         get => _alwaysAskBeforeCreatingFiles;
         set => SetField(ref _alwaysAskBeforeCreatingFiles, value);
      }
      
      [Description("Define how often and if the Modforge should ask where to save edited objects.")]
      [CompareInEquals]
      public FileSavingMode FileSavingMode
      {
         get => _fileSavingMode;
         set => SetField(ref _fileSavingMode, value);
      }
      
      [Description("Play a sound when a crash occurs")]
      [CompareInEquals]
      public bool PlayCrashSound
      {
         get => _playCrashSound;
         set => SetField(ref _playCrashSound, value);
      }


      [Description("The location where the log will be saved. Default is in the folder of the executable")]
      [CompareInEquals]
      public string LogLocation
      {
         get => _logLocation;
         set => SetField(ref _logLocation, value);
      }

      [Description("The location of the custom words file (ANSI). This file is used to generate random but context fitting words for naming or localisation.")]
      [CompareInEquals]
      public string CustomWordsLocation
      {
         get => _customWordsLocation;
         set => SetField(ref _customWordsLocation, value);
      }

      [Description("Defines what should happen to unsaved changes, when the application is closed.")]
      [CompareInEquals]
      public SaveOnExitType SaveOnExit
      {
         get => _saveOnExit;
         set => SetField(ref _saveOnExit, value);
      }

      [CompareInEquals]
      [Description("Settings for formatting files")]
      [TypeConverter(typeof(CEmptyStringConverter))]
      public FormattingSettings Formatting
      {
         get => _formatting;
         set => SetField(ref _formatting, value);
      }

      [CompareInEquals]
      [Description("The location where the mission exports will be saved. Default is in \'ModforgeData\\MissionExports\' folder.")]
      public string MissionExportPath
      {
         get => _missionExportLocation;
         set => SetField(ref _missionExportLocation, value);
      }
   }

   public class FormattingSettings : PropertySettings
   {
      private string _modPrefix = "mmf";
      private bool _addModifiedCommentToFilesWhenSaving = true;
      private bool _addCommentAboveObjectsInFiles = true;

      [Description("The prefix which will be use in all created objects e.g. eventmodifier names, file names, etc...")]
      [CompareInEquals]
      public string ModPrefix
      {
         get => _modPrefix;
         set => SetField(ref _modPrefix, value);
      }

      [Description("If true a comment is added above the modified elements in a file when it is being saved.")]
      [CompareInEquals]
      public bool AddModifiedCommentToFilesWhenSaving
      {
         get => _addModifiedCommentToFilesWhenSaving;
         set => SetField(ref _addModifiedCommentToFilesWhenSaving, value);
      }

      [Description("If true a comment is added above the objects with it's localisation when it is being saved.")]
      [CompareInEquals]
      public bool AddCommentAboveObjectsInFiles
      {
         get => _addCommentAboveObjectsInFiles;
         set => SetField(ref _addCommentAboveObjectsInFiles, value);
      }
   }
}