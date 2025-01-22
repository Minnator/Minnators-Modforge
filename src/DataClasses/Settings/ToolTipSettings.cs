using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
namespace Editor.DataClasses.Settings
{
   [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
   public class ToolTipSettings : SubSettings
   {
      private string _toolTipText = $"$MAPMODE_SPECIFIC$\n------------------\nId:   $Id$\nName: $TitleLocalisation$\nOwner: $Owner$ ($Owner%L$)\nArea: $Area$ ($Area%L$)";
      private bool _showToolTip = true;

      [Description("The text that will be shown in the tooltip")]
      [CompareInEquals]
      public string ToolTipText
      {
         get => _toolTipText;
         set => SetField(ref _toolTipText, value);
      }

      [Description("If the tooltip will be shown")]
      [CompareInEquals]
      public bool ShowToolTip
      {
         get => _showToolTip;
         set => SetField(ref _showToolTip, value);
      }
   }
}