using System.Text;
using Editor.DataClasses.Misc;
using Editor.Helper;
using Editor.Saving;

namespace Editor.DataClasses.GameDataClasses
{
   public class ProvinceGroup(string name, Color color) : ProvinceCollection<Province>(name, color)
   {
      public override void OnPropertyChanged(string? propertyName = null) { }
      public override SaveableType WhatAmI()
      {
         return SaveableType.ProvinceGroup;
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
         return new (Name, true);
      }

      public override string SavingComment()
      {
         return Localisation.GetLoc(Name);
      }

      public override string GetSaveString(int tabs)
      {
         var sb = new StringBuilder();
         SavingUtil.AddFormattedIntList(name, GetProvinceIds(), 0, ref sb);
         return sb.ToString();
      }

      public override string GetSavePromptString()
      {
         return $"Save province groups file";
      }
      
      public static EventHandler<ProvinceComposite> ColorChanged = delegate { };

      public override void ColorInvoke(ProvinceComposite composite)
      {
         ColorChanged.Invoke(this, composite);
      }

      public override void AddToColorEvent(EventHandler<ProvinceComposite> handler)
      {
         ColorChanged += handler;
      }

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
         Globals.ProvinceGroups.Remove(Name);
      }

      public override void AddGlobal()
      {
         Globals.ProvinceGroups.Add(Name, this);
      }
   }
}