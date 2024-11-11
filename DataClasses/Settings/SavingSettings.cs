using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Editor.DataClasses.Settings
{
   [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
   public class SavingSettings : INotifyPropertyChanged
   {
      private string _errorLogLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
      private string _loadingLogLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
      private bool _alwaysAskBeforeCreatingFiles = true;
      private FileSavingMode _fileSavingMode = FileSavingMode.AskOnce;

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

      public override bool Equals(object? obj)
      {
         if (obj is not SavingSettings settings)
            return false;

         var properties = GetType().GetProperties()
            .Where(prop => Attribute.IsDefined(prop, typeof(CompareInEquals)));

         foreach (var property in properties)
            if (!Equals(property.GetValue(this), property.GetValue(settings)))
               return false;

         return true;
      }

      public override int GetHashCode()
      {
         var properties = GetType().GetProperties()
            .Where(prop => Attribute.IsDefined(prop, typeof(CompareInEquals)));
         var hash = 17;

         foreach (var property in properties)
            hash = unchecked(hash * 31 + (property.GetValue(this)?.GetHashCode() ?? 0));

         return hash;
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