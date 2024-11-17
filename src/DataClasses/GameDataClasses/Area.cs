using System.Text;
using Editor.DataClasses.Misc;
using Editor.Helper;
using Editor.Saving;

namespace Editor.DataClasses.GameDataClasses;
#nullable enable
public class Area : ProvinceCollection<Province>
{

   public Area(string name, ICollection<Province> provinces, Color color) : base (name, color)
   {
      SubCollection = provinces;
   }

   // Contains the provinces in the area will be editable as the array is only a few elements long
   
   public string Edict { get; set; } = string.Empty;
   public float Prosperity { get; set; } = 0;
   public bool IsStated { get; set; } = false;

   public Region Region
   {
      get
      {
         return GetFirstParentOfType(SaveableType.Region) as Region ?? Region.Empty;
      }
   }

   public override bool Equals(object? obj)
   {
      if (obj is Area other)
         return Name == other.Name;
      return false;
   }

   public override int GetHashCode()
   {
      return Name.GetHashCode();
   }

   public override void OnPropertyChanged(string? propertyName = null) {  }

   public override SaveableType WhatAmI()
   {
      return SaveableType.Area;
   }

   public override string[] GetDefaultFolderPath()
   {
      return ["map"];
   }

   public override string GetFileEnding()
   {
      return ".txt";
   }

   public override KeyValuePair<string, bool> GetFileName()
   {
      return new("area", true);
   }

   public override string SavingComment()
   {
      return Localisation.GetLoc(Name);
   }

   public override string GetSaveString(int tabs)
   {
      var sb = new StringBuilder();
      SavingUtil.AddFormattedIntList(Name, GetProvinceIds(), 0, ref sb);
      return sb.ToString();
   }

   public override string GetSavePromptString()
   {
      return $"Save areas file";
   }

   public new static Area Empty => new("", [], Color.Empty);

   public static EventHandler<ProvinceComposite> AreaColorChanged = delegate { };

   public static EventHandler<ProvinceCollectionEventArguments<Province>> ItemsModified = delegate { };

   public override void Invoke(ProvinceCollectionEventArguments<Province> eventArgs)
   {
      ItemsModified.Invoke(this, eventArgs);
   }

   public override void AddToEvent(EventHandler<ProvinceCollectionEventArguments<Province>> eventHandler)
   {
      ItemsModified += eventHandler;
   }
   public override void RemoveGlobal()
   {
      Globals.Areas.Remove(Name);
   }

   public override void AddGlobal()
   {
      Globals.Areas.Add(Name, this);
   }

   public override void ColorInvoke(ProvinceComposite composite)
   {
      AreaColorChanged.Invoke(this, composite);
   }

   public override void AddToColorEvent(EventHandler<ProvinceComposite> handler)
   {
      AreaColorChanged += handler;
   }

}