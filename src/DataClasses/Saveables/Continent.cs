using System.Text;
using Editor.ErrorHandling;
using Editor.Helper;
using Editor.Loading.Enhanced.PCFL.Implementation;
using Editor.Saving;

namespace Editor.DataClasses.Saveables;
public class Continent : ProvinceCollection<Province>, ITarget
{
   public Continent(string name, Color color, ObjEditingStatus status = ObjEditingStatus.Modified) : base(name, color, status)
   {
   }

   public Continent(string name, Color color, ref PathObj path, ICollection<Province> provinces) : base(name, color, ref path, provinces)
   {
      SubCollection = provinces;
   }

   public override SaveableType WhatAmI()
   {
      return SaveableType.Continent;
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
      return new ("continent", true);
   }

   public override string SavingComment()
   {
      return Localisation.GetLoc(Name);
   }
   public override void OnPropertyChanged(string? propertyName = null) { }

   public override string GetSaveString(int tabs)
   {
      var sb = new StringBuilder();
      SavingUtil.AddFormattedIntList(Name, GetProvinceIds(), 0, ref sb);
      return sb.ToString();
   }

   public override string GetSavePromptString()
   {
      return $"Save continents file";
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
      Globals.Continents.Remove(Name);
   }

   public override void AddGlobal()
   {
      if (!Globals.Continents.TryAdd(Name, this))
         MessageBox.Show($"The Continent {Name} does already exist and can not be created.", $"Continent {Name} already exists!", MessageBoxButtons.OK, MessageBoxIcon.Error);
   }

   public new static Continent Empty { get; } = new(string.Empty, System.Drawing.Color.Empty, ObjEditingStatus.Immutable);



   public static IErrorHandle TryParse(string input, out Continent area)
   {
      if (Globals.Continents.TryGetValue(input, out area!))
         return ErrorHandle.Success;
      return new ErrorObject(ErrorType.TypeConversionError, $"Continent \"{input}\" not found!", addToManager: false);
   }
}