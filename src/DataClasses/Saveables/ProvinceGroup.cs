using System.Text;
using Editor.ErrorHandling;
using Editor.Helper;
using Editor.Loading.Enhanced.PCFL.Implementation;
using Editor.Saving;

namespace Editor.DataClasses.Saveables
{
   public class ProvinceGroup : ProvinceCollection<Province>, ITarget
   {
      public ProvinceGroup(string name, Color color, ObjEditingStatus status = ObjEditingStatus.Modified) : base(name, color, status)
      {
      }

      public ProvinceGroup(string name, Color color, ref PathObj path, ICollection<Province> provinces) : base(name, color, ref path, provinces)
      {
      }

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
         return new ("provincegroup", true);
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
         return $"Province Group: {Name}";
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

      public static IErrorHandle TryParse(string input, out ProvinceGroup area)
      {
         if (Globals.ProvinceGroups.TryGetValue(input, out area!))
            return ErrorHandle.Success;
         return new ErrorObject(ErrorType.TypeConversionError, $"ProvinceGroup \"{input}\" not found!", addToManager: false);
      }
   }
}