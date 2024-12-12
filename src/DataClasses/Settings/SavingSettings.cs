using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace Editor.DataClasses.Settings
{
   [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
   public class SavingSettings : SubSettings
   {
      private bool _alwaysAskBeforeCreatingFiles = true;
      private FileSavingMode _fileSavingMode = FileSavingMode.AskOnce;
      private bool _playCrashSound = true;
      private string _modPrefix = "mmf";
      private bool _addModifiedCommentToFilesWhenSaving = true;
      private bool _addCommentAboveObjectsInFiles = true;
      private string _logLocation = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");

      [Description(
         "<true> Asks for a filename or location beofre creating a new file\n<false> creates files with default names")]
      [TypeConverter(typeof(ExpandableObjectConverter))]
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

      [Description("The location where the log will be saved. Default is in the folder of the executable")]
      [CompareInEquals]
      public string LogLocation
      {
         get => _logLocation;
         set => SetField(ref _logLocation, value);
      }
   }
}