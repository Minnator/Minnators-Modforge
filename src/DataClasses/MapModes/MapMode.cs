using Editor.DataClasses.Saveables;
using Editor.Helper;

namespace Editor.DataClasses.MapModes;

public abstract class MapMode
{
   public Bitmap Bitmap { get; set; } = null!;
   public virtual bool IsLandOnly => false;
   public virtual bool ShowOccupation => false;
   public virtual Bitmap? Icon { get; protected set; }
   public virtual string IconFileName { get; } = null!;
   public virtual bool IsCollectionMapMode => false;
   public bool IsCurrent => MapModeManager.CurrentMapMode == this;

   public virtual void RenderMapMode()
   {
      // TODO optimize this method
      if (IsLandOnly && MapModeManager.PreviousLandOnly)
         MapDrawing.DrawOnMap(Globals.LandProvinces, Globals.ZoomControl, PixelsOrBorders.Pixels);
      else
         MapDrawing.DrawOnMap(Globals.Provinces, Globals.ZoomControl, PixelsOrBorders.Pixels);
      
      if (ShowOccupation)
         MapDrawing.DrawOccupations(false, Globals.ZoomControl);

      MapDrawing.DrawAllBorders(Color.Black.ToArgb(), Globals.ZoomControl, MapModeManager.ColorCache);
      Selection.RePaintSelection();
      Globals.ZoomControl.Invalidate();
      MapModeManager.PreviousLandOnly = IsLandOnly;
   }
   
   protected virtual void CropAndSetIcon()
   {
      var iconPath = Path.Combine(Globals.VanillaPath, "gfx", "interface", IconFileName);
      if (!File.Exists(iconPath))
      {
         MessageBox.Show($"MapMode Icon file not found: {iconPath}", iconPath, MessageBoxButtons.OK);
         return;
      }

      using var icon = ImageReader.ReadImage(iconPath);
      Icon = CropRawIcon(icon);
   }

   protected virtual Bitmap CropRawIcon(Bitmap rawBmp)
   {
      var bmp = new Bitmap(27, 21);
      using var g = Graphics.FromImage(bmp);
      g.DrawImage(rawBmp, new Rectangle(0, 0, 27, 21), new (7, 5, 27, 21), GraphicsUnit.Pixel);
      return bmp;
   }

   public abstract MapModeType MapModeType { get; }

   public abstract int GetProvinceColor(Province id);

   public virtual int GetSeaProvinceColor(Province id)
   {
      return id.Color.ToArgb();
   }
   
   public virtual void Update(ICollection<Province> ids)
   {
      if (ids.Count == 0)
         return;

      foreach (var id in ids) 
         Update(id, false);
      Globals.ZoomControl.Invalidate();
   }

   public virtual void UpdateProvince(object? sender, EventArgs e)
   {
      if (MapModeManager.CurrentMapMode != this)
         return;
      if (sender is not Province id || (IsLandOnly && !Globals.LandProvinces.Contains(id)))
         return;
      Update(id);
   }

   public virtual void UpdateProvinceCollection<T>(object? sender, ProvinceCollectionEventArguments<T> e) where T : ProvinceComposite
   {
      if (MapModeManager.CurrentMapMode != this)
         return;

      foreach (var composite in e.Composite) 
         UpdateComposite<T>(sender, composite);
   }

   public virtual void UpdateComposite<T> (object? sender, ProvinceComposite composite) where T : ProvinceComposite
   {
      if (MapModeManager.CurrentMapMode != this)
         return;

      Update(composite.GetProvinces());
   }

   public virtual void Update(Province province, bool invalidate = true)
   {
      var color = GetProvinceColor(province);
      MapModeManager.UpdateCache(province, color);
      MapDrawing.DrawOnMap(province, color, Globals.ZoomControl, PixelsOrBorders.Pixels);

      if (ShowOccupation)
         MapDrawing.DrawOccupation(province, false, Globals.ZoomControl);
      if (invalidate) 
         Globals.ZoomControl.Invalidate();

      Selection.RePaintSelection();
   }

   public abstract string GetSpecificToolTip(Province provinceId);

   public virtual void SetActive()
   {

   }
   
   public virtual void SetInactive()
   {

   }

}
