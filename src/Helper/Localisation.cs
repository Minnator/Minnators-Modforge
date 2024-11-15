using Editor.DataClasses.Commands;
using Editor.DataClasses.Misc;
using Editor.Events;
using Editor.Saving;

namespace Editor.Helper;

public static class Localisation
{
   private static readonly LocObject SearchLoc = new("", string.Empty, ObjEditingStatus.Immutable);
   


   public static string GetLoc (string key)
   {
      SearchLoc.Key = key;
      return GetLocObject(key, out var value) ? value?.Value ?? "no_loc" : key;
   }

   public static bool GetLocObject(string key, out LocObject? locObject)
   {
      SearchLoc.Key = key;
      return Globals.Localisation.TryGetValue(SearchLoc, out locObject);
   }

   /// <summary>
   /// If the value has not changed, it will not be added or updated
   /// If the value has changed, it will be updated
   /// If the key does not exist, it will be added
   /// </summary>
   /// <param name="key"></param>
   /// <param name="value"></param>
   public static void AddOrModifyLocObject(string key, string value)
   {
      if (!GetLocObject(key, out var locObject))
      {
         if (!string.IsNullOrEmpty(value))
            Globals.Localisation.Add(new(key, value));
      }
      else
      {
         if (locObject == null)
            return;
         if (locObject.Path.isModPath && string.IsNullOrEmpty(value))
         {
            // TODO delete locObject
         }
         locObject.Value = value;
      }
   }

   public static void Initialize()
   {
      CountryGuiEvents.SetGuiEventHandlers();
   }
}

/// <summary>
/// The value will never be updated if it would not changed
/// </summary>
public class LocObject : Saveable
{
   public LocObject(string key, string value, ObjEditingStatus status = ObjEditingStatus.Modified)
   {
      Key = key;
      _value = value;
      EditingStatus = status;
   }
   public string Key { get; set; }
   private string _value;

   public string Value
   {
      get => _value;
      set {
         if (value == _value)
            return;
         _value = value;
         EditingStatus = ObjEditingStatus.Modified;
      }
   }

   public sealed override ObjEditingStatus EditingStatus
   {
      get => _editingStatus;
      set
      {
         if (_editingStatus == ObjEditingStatus.Immutable)
            return;
         if (Equals(value, _editingStatus))
            return;
         if (Equals(value, ObjEditingStatus.Modified))
            FileManager.AddLocObject(this);
         _editingStatus = value;
      }
   }

   public override SaveableType WhatAmI()
   {
      return SaveableType.Localisation;
   }

   public override string SavingComment()
   {
      return string.Empty;
   }

   public override PathObj GetDefaultSavePath()
   {
      return new(["localisation"]);
   }

   public override string GetSaveString(int tabs)
   {
      return $"{Key}:0 \"{Value}\"";
   }

   public override string GetSavePromptString()
   {
      return $"localisation: \"{Key}\"";
   }

   public override int GetHashCode()
   {
      return Key.GetHashCode();
   }

   public override bool Equals(object? obj)
   {
      if (obj is not LocObject locObject) 
         return false;
      return Key.Equals(locObject.Key);
   }
}

public class LocEventArgs : EventArgs
{
   public LocObject LocObject { get; }
   public string NewValue { get; }

   public LocEventArgs(LocObject locObject, string newValue)
   {
      LocObject = locObject;
      NewValue = newValue;
   }
}

public static class LocObjectModifications
{
   private static readonly LocObject SearchLoc = new("", string.Empty, ObjEditingStatus.Immutable);
   public static void ModifyIfExistsOtherwiseAdd(string key, string newValue)
   {
      SearchLoc.Key = key;
      if (Globals.Localisation.TryGetValue(SearchLoc, out var loc))
      {
         if (loc.Value != newValue)
            ModifyLocObject(loc, newValue);
      }
      else
         AddLocObject(key, newValue, true);
   }

   public static void ModifyLocObject(LocObject locObject, string newValue)
   {
      Globals.HistoryManager.AddCommand(new CModifyLocalisation(locObject, newValue));
   }

   public static void AddLocObject(string key, string value, bool add)
   {
      Globals.HistoryManager.AddCommand(new CAddDelLocalisation(new (key, value), add));
   }

   public static void DeleteLocObject(LocObject locObject)
   {
      Globals.HistoryManager.AddCommand(new CAddDelLocalisation(locObject, false));
   }
}