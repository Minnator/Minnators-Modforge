using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Editor.DataClasses.Settings
{
   public class PopUpSettings : SubSettings
   {
      private bool _askWhenSavingAllChanges = true;

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
   }
}