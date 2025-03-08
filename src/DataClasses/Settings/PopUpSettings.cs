using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Editor.DataClasses.Settings
{
   public class PopUpSettings : SubSettings
   {
      private bool _askWhenSavingAllChanges = true;
      private bool _notifyIfErrorFileCanNotBeOpened = true;
      private bool _tryContinueExecutionPopUp = true;

      [Description("Sets if there will be a Pop up when saving all changes to prevent accidents")]
      [CompareInEquals]
      public bool AskWhenSavingAllChanges
      {
         get => _askWhenSavingAllChanges;
         set => SetField(ref _askWhenSavingAllChanges, value);
      }

      [JsonIgnore]
      [Browsable(false)]
      public ref bool AskWhenSavingAllChangesRef => ref _askWhenSavingAllChanges;

      [Description("Sets if there will be a Pop up when a file containing an error can not be opened.")]
      [CompareInEquals]
      public bool NotifyIfErrorFileCanNotBeOpened
      {
         get => _notifyIfErrorFileCanNotBeOpened;
         set => SetField(ref _notifyIfErrorFileCanNotBeOpened, value);
      }

      [JsonIgnore]
      [Browsable(false)]
      public ref bool TryContinueExecutionPopUp => ref _tryContinueExecutionPopUp;

      [JsonIgnore]
      [Browsable(false)]
      public ref bool NotifyIfErrorFileCanNotBeOpenedRef => ref _notifyIfErrorFileCanNotBeOpened;
   }
}