using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Editor.Helper;

namespace Editor.DataClasses.Settings
{
   [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
   public class SavingSettings : PropertyEquals, INotifyPropertyChanged
   {
      private string _errorLogLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
      private string _loadingLogLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
      private bool _alwaysAskBeforeCreatingFiles = true;
      private FileSavingMode _fileSavingMode = FileSavingMode.AskOnce;
      private bool _playCrashSound = true;
      private string _modPrefix = "mmf";
      private bool _addModifiedCommentToFilesWhenSaving = true;
      private bool _addCommentAboveObjectsInFiles = true;

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

      [Description("The location where the loading log will be saved")]
      [CompareInEquals]
      public string LoadingLogLocation
      {
         get => _loadingLogLocation;
         set => SetField(ref _loadingLogLocation, value);
      }

      [Description("The location where the error log will be saved")]
      [CompareInEquals]
      public string ErrorLogLocation
      {
         get => _errorLogLocation;
         set => SetField(ref _errorLogLocation, value);
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


      public event PropertyChangedEventHandler? PropertyChanged;

      private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
      {
         PropertyChanged?.Invoke(this, new(propertyName));
      }

      private bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
      {
         if (EqualityComparer<T>.Default.Equals(field, value))
            return false;
         field = value;
         OnPropertyChanged(propertyName);
         return true;
      }
   }
}