﻿using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

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
      [TypeConverter(typeof(ExpandableObjectConverter))]
      public CompactingSettings CompactingSettings
      {
         get => _compactingSettings;
         set => SetField(ref _compactingSettings, value);
      }

      [Description("Settings for customization")]
      [CompareInEquals]
      [TypeConverter(typeof(ExpandableObjectConverter))]
      public CustomizationOptions CustomizationOptions
      {
         get => _customizationOptions;
         set => SetField(ref _customizationOptions, value);
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