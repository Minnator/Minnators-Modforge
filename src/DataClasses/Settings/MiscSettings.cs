﻿using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using Editor.NameGenerator;

namespace Editor.DataClasses.Settings
{
   public enum PreferredEditor
   {
      VSCode,
      NotepadPlusPlus,
      Other
   }


[SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
   public sealed class MiscSettings : SubSettings
   {
      private Language _language = Language.english;
      private string _lastModPath = string.Empty;
      private string _lastVanillaPath = string.Empty;
      private CompactingSettings _compactingSettings = new ();
      private CustomizationOptions _customizationOptions = new();
#if DEBUG
      private TestSettings _testSettings = new();
#endif
      private EXT_ControlSettings _extControlSettings = new();
      private NameGeneratorSettings _nameGenConfig = new();
      private TimeSpan _runTime;
      private bool _ownerChangesController;
      private bool _copyScenario = true;
      private bool _copyHistory = false;

      [Description("The time the application has been running across sessions.")]
      [CompareInEquals]
      public TimeSpan RunTime
      {
         get => _runTime;
         set => SetField(ref _runTime, value);
      }

      [Description("Determines if the owner changes the controller.")]
      [CompareInEquals]
      public bool OwnerChangesController
      {
         get => _ownerChangesController;
         set => SetField(ref _ownerChangesController, value);
      }

      [Description("Determines if the scenario should be copied when pasting a province to another")]
      [CompareInEquals]
      public bool CopyScenario
      {
         get => _copyScenario;
         set => SetField(ref _copyScenario, value);
      }

      [Description("Determines if the history should be copied when pasting a province to another")]
      [CompareInEquals]
      public bool CopyHistory
      {
         get => _copyHistory;
         set => SetField(ref _copyHistory, value);
      }

      [Description("The language in which the localisation will be shown")]
      [CompareInEquals]
      public Language Language
      {
         get => _language;
         set => SetField(ref _language, value);
      }

      [Description("The path to the last opened mod")]
      [CompareInEquals]
      public string LastModPath
      {
         get => _lastModPath;
         set => SetField(ref _lastModPath, value);
      }

      [Description("The last used Vanilla location")]
      [CompareInEquals]
      public string LastVanillaPath
      {
         get => _lastVanillaPath;
         set => SetField(ref _lastVanillaPath, value);
      }
      

      [Description("Settings for compacting commands")]
      [CompareInEquals]
      [TypeConverter(typeof(CEmptyStringConverter))]
      public CompactingSettings CompactingSettings
      {
         get => _compactingSettings;
         set => SetField(ref _compactingSettings, value);
      }

      [Description("Settings for customization")]
      [CompareInEquals]
      [TypeConverter(typeof(CEmptyStringConverter))]
      public CustomizationOptions CustomizationOptions
      {
         get => _customizationOptions;
         set => SetField(ref _customizationOptions, value);
      }

#if DEBUG
      [Description("Settings for testing")]
      [CompareInEquals]
      //[TypeConverter(typeof(ExpandableObjectConverter))]
      [Editor(typeof(CustomizationOptionsEditor), typeof(UITypeEditor))]
      public TestSettings TestSettings
      {
         get => _testSettings;
         set => SetField(ref _testSettings, value);
      }
#endif

      [Description("Determines if the achievement settings can be accessed")]
      [CompareInEquals]
      [Browsable(false)]
      public bool CanAccessAchievementSettings { get; set; } = true;

      [Description("Settings for the EXT controls")]
      [CompareInEquals]
      [TypeConverter(typeof(CEmptyStringConverter))]
      public EXT_ControlSettings EXT_ControlSettings
      {
         get => _extControlSettings;
         set => SetField(ref _extControlSettings, value);
      }

      [Description("Settings for the name generator")]
      [CompareInEquals]
      [TypeConverter(typeof(CEmptyStringConverter))]
      public NameGeneratorSettings NameGenConfig
      {
         get => _nameGenConfig;
         set => SetField(ref _nameGenConfig, value);
      }
   }

   public class NameGeneratorSettings : PropertySettings
   {
      private NameGenConfig _nameGenConfig = new();
      private string _customNamesFile = string.Empty;

      [Description("Settings for the name generator")]
      [CompareInEquals]
      [TypeConverter(typeof(CEmptyStringConverter))]
      public NameGenConfig NameGenConfig
      {
         get => _nameGenConfig;
         set => SetField(ref _nameGenConfig, value);
      }

      [Description("The file containing custom names")]
      [CompareInEquals]
      [TypeConverter(typeof(CEmptyStringConverter))]
      public string CustomNamesFile
      {
         get => _customNamesFile;
         set => SetField(ref _customNamesFile, value);
      }
   }


#if DEBUG
   public class CustomizationOptionsEditorForm : Form
   {
      private TestSettings _options;
      public TestSettings EditedOptions => _options;

      public CustomizationOptionsEditorForm(TestSettings options)
      {
         _options = options;
         this.Text = "Edit Customization Options";
         this.Width = 300;
         this.Height = 200;

         // Example UI
         TextBox txtValue = new TextBox { Text = _options.FooBar, Dock = DockStyle.Top };
         Button btnOK = new Button { Text = "OK", Dock = DockStyle.Bottom };
         btnOK.Click += (s, e) => { _options.FooBar = txtValue.Text; this.DialogResult = DialogResult.OK; };

         Controls.Add(txtValue);
         Controls.Add(btnOK);
      }
   }


   public class CustomizationOptionsEditor : UITypeEditor
   {
      public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
      {
         // This enables a modal dialog (like the color picker)
         return UITypeEditorEditStyle.Modal;
      }

      public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
      {
         if (provider != null)
         {
            IWindowsFormsEditorService editorService = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
            if (editorService != null)
            {
               // Open a custom form for editing
               using (CustomizationOptionsEditorForm form = new CustomizationOptionsEditorForm(value as TestSettings))
               {
                  if (editorService.ShowDialog(form) == DialogResult.OK)
                  {
                     return form.EditedOptions; // Return the updated object
                  }
               }
            }
         }
         return value;
      }
   }

   public class TestSettings : PropertySettings
   {
      public string FooBar { get; set; } = "fooBar";

      public override string ToString()
      {
         return "(Custom)"; // Ensures the editor is used instead of expanding properties
      }
   }
#endif

   public class EXT_ControlSettings : PropertySettings
   {
      private int _textBoxEditConfirmationInterval = 1500;

      [Description("The interval in milliseconds after which the text box will confirm the input")]
      [CompareInEquals]
      public int TextBoxEditConfirmationInterval
      {
         get => _textBoxEditConfirmationInterval;
         set => SetField(ref _textBoxEditConfirmationInterval, value);
      }
   }

   public class CustomizationOptions : PropertySettings
   {
      private bool _useEu4Cursor = true;
      private bool _useDiscordRichPresence = true;
      private bool _useDynamicProvinceNames = true;
      private PreferredEditor _preferredEditor = PreferredEditor.VSCode;

      [Description("Determines if the EU4 cursor should be used")]
      [CompareInEquals]
      public bool UseEu4Cursor
      {
         get => _useEu4Cursor;
         set => SetField(ref _useEu4Cursor, value);
      }

      [Description("Determines if the Discord Rich Presence should be used. \n Changing this requires restarting the application.")]
      [CompareInEquals]
      public bool UseDiscordRichPresence
      {
         get => _useDiscordRichPresence;
         set => SetField(ref _useDiscordRichPresence, value);
      }

      [Description("Determines if dynamic province names should be used")]
      [CompareInEquals]
      public bool UseDynamicProvinceNames
      {
         get => _useDynamicProvinceNames;
         set => SetField(ref _useDynamicProvinceNames, value);
      }

      [Description("The preferred editor for opening files")]
      [CompareInEquals]
      public PreferredEditor PreferredEditor
      {
         get => _preferredEditor;
         set => SetField(ref _preferredEditor, value);
      }
   }

   public class CompactingSettings : PropertySettings
   {
      public enum AutoCompStrategy {
         None,
         AfterXSize,
         EveryXMinutes
      }

      private int _maxCompactingSize = 500;
      private int _minNumForCompacting = 5;
      private AutoCompStrategy _autoCompactingStrategy = AutoCompStrategy.AfterXSize;
      private int _autoCompactingMinSize = 100;
      private int _autoCompactingDelay = 5;

      [Description("The maximum number of commands which will be compacted into a compacting command")]
      [CompareInEquals]
      public int MaxCompactingSize
      {
         get => _maxCompactingSize;
         set => SetField(ref _maxCompactingSize, value);
      }

      [Description("The minimum number of commands which will be compacted if applicable")]
      [CompareInEquals]
      public int MinNumForCompacting
      {
         get => _minNumForCompacting;
         set => SetField(ref _minNumForCompacting, value);
      }

      [Description("The strategy for auto compacting")]
      [CompareInEquals]
      public AutoCompStrategy AutoCompactingStrategy
      {
         get => _autoCompactingStrategy;
         set => SetField(ref _autoCompactingStrategy, value);
      }

      [Description("The minimum size for auto compacting to trigger")]
      [CompareInEquals]
      public int AutoCompactingMinSize
      {
         get => _autoCompactingMinSize;
         set => SetField(ref _autoCompactingMinSize, value);
      }

      [Description("The delay in minutes for auto compacting to trigger")]
      [CompareInEquals]
      public int AutoCompactingDelay
      {
         get => _autoCompactingDelay;
         set => SetField(ref _autoCompactingDelay, value);
      }
   }
}