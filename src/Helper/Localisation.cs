using Editor.DataClasses.Commands;
using Editor.DataClasses.GameDataClasses;
using Editor.DataClasses.Saveables;
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

   public static string GetDynamicProvinceLoc(Province p)
   {
      if (Globals.CustomProvinceNames.TryGetValue(p, out var names))
         return names.GetLoc(p);
      return GetLoc(p.TitleKey);
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
         if (locObject.Path.IsModPath && string.IsNullOrEmpty(value))
         {
            // TODO delete locObject
         }
         locObject.Value = value;
      }
   }

}

public interface ITitleAdjProvider
{
   public string TitleKey { get; }
   public string TitleLocalisation { get; }
   public string AdjectiveKey { get; }
   public string AdjectiveLocalisation { get; }
}


/// <summary>
/// The value will never be updated if it would not changed
/// </summary>
public class LocObject : Saveable
{
   public override void OnPropertyChanged(string? propertyName = null) { }
   public LocObject(string key, string value, ObjEditingStatus status = ObjEditingStatus.Modified)
   {
      Key = key;
      _value = value;
      EditingStatus = status;
   }
   public string Key { get; set; }
   private string _value;

   public void SilentSet(string value) => _value = value;

   public string Value
   {
      get => _value;
      set {
         if (value.Equals(_value))
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
         switch (value)
         {
            case ObjEditingStatus.Modified:
            case ObjEditingStatus.ToBeDeleted:
               SaveMaster.AddLocObject(this);
               break;
            case ObjEditingStatus.Unchanged:
            case ObjEditingStatus.Deleted:
               SaveMaster.RemoveFromToBeHandled(this);
               break;
         }

         //TODO: what if we undo here? - ~no.
         _editingStatus = value;
      }
   }

   public override SaveableType WhatAmI()
   {
      return SaveableType.Localisation;
   }

   public override string[] GetDefaultFolderPath()
   {
      return ["localisation"];
   }

   public override string GetFileEnding()
   {
      return $"_l_{Globals.Settings.Misc.Language.ToString().ToLower()}.yml";
   }

   public override KeyValuePair<string, bool> GetFileName()
   {
      return new("Modforge_loc", false);
   }

   public override string SavingComment()
   {
      return string.Empty;
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

   public override string ToString()
   {
      return $"{Key} : {Value}";
   }

   public override bool Equals(object? obj)
   {
      if (obj is not LocObject locObject) 
         return false;
      return Key.Equals(locObject.Key);
   }
}

public enum CustomProvLocType
{
   None = -1,
   CultureGroup,
   Culture,
   Tag,
}

public class CultProvLocObject(
   CustomProvLocType type,
   string key,
   string value,
   ObjEditingStatus status = ObjEditingStatus.Modified)
   : LocObject(key, value, status)
{
   public readonly CustomProvLocType Type = type;
   public string Capital = string.Empty;

   public CultProvLocObject(CustomProvLocType type, string key, string value, string capital, ObjEditingStatus status = ObjEditingStatus.Modified)
      : this(type, key, value, status)
   {
      Capital = capital;
   }

   public bool IsOfType(ref CustomProvLocType type) => Type == type;

   public override int GetHashCode() => Type.GetHashCode() ^ Key.GetHashCode();
   public override bool Equals(object? obj)
   {
      if (obj is not CultProvLocObject cultProvLocObject)
         return false;
      return Type == cultProvLocObject.Type && Key.Equals(cultProvLocObject.Key);
   }

   public override string ToString() => $"{Type} : {Key} : {Value}";

   public override string GetSaveString(int tabs)
   {
      if (Capital.Equals(string.Empty)) // We have no capital defined
         return $"{Key} = \"{Value}\"";
      return $"{Key} = {{ \"{Value}\" \"{Capital}\" }}"; // We have a capital defined
   }
}

public class CultProvLocContainer
{
   private List<CultProvLocObject> _tagObjects = [];
   private List<CultProvLocObject> _cultureObjects = [];
   private List<CultProvLocObject> _cultureGroupObjects = [];

   public void AddList(ICollection<CultProvLocObject> objs)
   {
      foreach (var obj in objs)
         Add(obj);
   }

   public void Add(CultProvLocObject obj)
   {
      switch (obj.Type)
      {
         case CustomProvLocType.Tag:
            _tagObjects.Add(obj);
            break;
         case CustomProvLocType.Culture:
            _cultureObjects.Add(obj);
            break;
         case CustomProvLocType.CultureGroup:
            _cultureGroupObjects.Add(obj);
            break;
      }
   }

   public string GetLoc(Province p)
   {
      // if we have a tag object, we will return the value
      var result = _tagObjects.FirstOrDefault(x => x.Key == p.Owner.Tag);
      if (result != null)
         return result.Value;

      // if we have a culture object, we will return the value
      result = _cultureObjects.FirstOrDefault(x => x.Key == p.Culture.Name);
      if (result != null)
         return result.Value;

      // if we have a culture group object, we will return the value
      if (!Globals.CultureGroups.TryGetValue(p.Culture.Name, out var group))
         return Localisation.GetLoc(p.TitleKey);
      result = _cultureGroupObjects.FirstOrDefault(x => x.Key == group.Name);
      return result?.Value ?? Localisation.GetLoc(p.TitleKey);
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
      HistoryManager.AddCommand(new CModifyLocalisation([locObject], newValue));
   }

   public static void AddLocObject(string key, string value, bool add)
   {
      HistoryManager.AddCommand(new CAddDelLocalisation(new (key, value), add));
   }

   public static void DeleteLocObject(LocObject locObject)
   {
      HistoryManager.AddCommand(new CAddDelLocalisation(locObject, false));
   }

   public static void ModifyProvinceLocalisation(bool adjective, string newValue)
   {
      foreach (var province in Selection.GetSelectedProvinces) 
         ModifyIfExistsOtherwiseAdd(adjective ? province.GetDescriptionLocKey : province.TitleKey, newValue);
   }
}