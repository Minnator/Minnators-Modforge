using System.Text;
using Editor.ErrorHandling;
using Editor.Helper;
using Editor.Loading.Enhanced.PCFL.Implementation;
using Editor.Saving;

namespace Editor.DataClasses.Saveables;
public class SuperRegion : ProvinceCollection<Region>, ITarget
{
   public bool RestrictCharter { get; set; } = false;
   public SuperRegion(string name, Color color, ObjEditingStatus status = ObjEditingStatus.Modified) : base(name, color, status)
   {
   }

   public SuperRegion(string name, Color color, ref PathObj path, ICollection<Region> provinces) : base(name, color, ref path, provinces)
   {
   }

   public override void OnPropertyChanged(string? propertyName = null) { }
  

   public override SaveableType WhatAmI()
   {
      return SaveableType.SuperRegion;
   }

   public override string[] GetDefaultFolderPath()
   {
      return ["map"];
   }

   public override string GetFileEnding()
   {
      return ".txt";
   }

   public override string SavingComment()
   {
      return Localisation.GetLoc(Name);
   }

   public override KeyValuePair<string, bool> GetFileName()
   {
      return new("superregion", true);
   }

   public override string GetSaveString(int tabs)
   {
      List<string> regionNames = [];
      foreach (var region in SubCollection)
         regionNames.Add(region.Name);

      var sb = new StringBuilder();
      SavingUtil.AddFormattedStringListOnePerRow(Name, regionNames, 0, ref sb);
      return sb.ToString();
   }

   public override string GetSavePromptString()
   {
      return $"Save super regions file";
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
   public static EventHandler<ProvinceCollectionEventArguments<Region>> ItemsModified = delegate { };

   public override void Invoke(ProvinceCollectionEventArguments<Region> eventArgs)
   {
      ItemsModified.Invoke(this, eventArgs);
   }

   public override void AddToEvent(EventHandler<ProvinceCollectionEventArguments<Region>> eventHandler)
   {
      ItemsModified += eventHandler;
   }

   public override string GetHeader()
   {
      return "#\r\n#  ⎛⎝(•ⱅ•)⎠⎞\r\n";
   }

   public override void RemoveGlobal()
   {
      Globals.SuperRegions.Remove(Name);
   }

   public override void AddGlobal()
   {
      if (!Globals.SuperRegions.TryAdd(Name, this))
         MessageBox.Show($"The SuperRegion {Name} does already exist and can not be created.", $"SuperRegion {Name} already exists!", MessageBoxButtons.OK, MessageBoxIcon.Error);
   }

   public new static SuperRegion Empty  { get; } = new (string.Empty, System.Drawing.Color.Empty, ObjEditingStatus.Immutable);

   public static IErrorHandle TryParse(string input, out SuperRegion area)
   {
      if (Globals.SuperRegions.TryGetValue(input, out area!))
         return ErrorHandle.Success;
      return new ErrorObject(ErrorType.TypeConversionError, $"SuperRegion \"{input}\" not found!", addToManager: false);
   }
}