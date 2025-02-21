using System.Diagnostics;
using System.Text;
using Editor.DataClasses.Saveables;
using Editor.Helper;
using Editor.Saving;

namespace Editor.DataClasses.GameDataClasses
{
   public class Climate : ProvinceCollection<Province>
   {
      public Climate(string name, Color color, ObjEditingStatus status = ObjEditingStatus.Modified) : base(name, color, status)
      {
      }

      public Climate(string name, Color color, ref PathObj path, ICollection<Province> provinces) : base(name, color, ref path, provinces)
      {

      }

      public static EventHandler<ProvinceComposite> ClimateColorChanged = delegate { };

      public static EventHandler<ProvinceCollectionEventArguments<Province>> ItemsModified = delegate { };
      public override void OnPropertyChanged(string? propertyName = null) {  }
      public override SaveableType WhatAmI() => SaveableType.Climate;

      public override string GetSaveString(int tabs)
      {
         var sb = new StringBuilder();
         SavingUtil.AddFormattedProvinceList(tabs, SubCollection, Name, ref sb);
         return sb.ToString();
      }
      public override string[] GetDefaultFolderPath() => ["map"];
      public override string GetFileEnding() => ".txt";
      public override KeyValuePair<string, bool> GetFileName() => new("climate", true);
      public override string SavingComment() => Localisation.GetLoc(Name);
      public override string GetSavePromptString() => "Select a file for the climate.txt";
      public override void ColorInvoke(ProvinceComposite composite) => ClimateColorChanged.Invoke(this, composite);
      public override void AddToColorEvent(EventHandler<ProvinceComposite> handler) => ClimateColorChanged += handler;
      public override void Invoke(ProvinceCollectionEventArguments<Province> eventArgs) => ItemsModified.Invoke(this, eventArgs);
      public override void AddToEvent(EventHandler<ProvinceCollectionEventArguments<Province>> eventHandler) => ItemsModified += eventHandler;
      public override void RemoveGlobal() => Globals.Climates.Remove(Name);
      public override void AddGlobal() => Globals.Climates.Add(Name, this);
   }
}