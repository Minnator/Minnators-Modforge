using System.Diagnostics;
using Editor.DataClasses.GameDataClasses;
using Timer = System.Windows.Forms.Timer;

namespace Editor.DataClasses.MapModes
{
   public class MemeMode : MapMode
   {
      private Timer _timer = new();
      private int _frameIndex = 0;
      private readonly string _pathAndFile = $"{Path.Combine(Globals.AppDirectory, "BadApple")}";
      private const string ending = "";
      private Bitmap[] frames;
      private Point _offset;
      private float _scale = 1f;


      public MemeMode()
      {
         var filesInFolder = Directory.GetFiles(_pathAndFile).OrderBy(f => new FileInfo(f).CreationTime).ToArray();
         // only take 10 first images
         frames = new Bitmap[filesInFolder.Length];

         var cnt = 0;
         foreach (var file in filesInFolder) 
            frames[cnt++] = new (file);
      }

      public override void SetActive()
      {
         _timer.Interval = Globals.Settings.Rendering.MsTimerIntervalMapModeTimer;
         _timer.Tick += Timer_Tick;
         _timer.Start();
         Globals.Settings.ToolTip.ShowToolTip = false;
      }

      public override void SetInactive()
      {
         _timer.Stop();
         _timer.Tick -= Timer_Tick;
         Globals.Settings.ToolTip.ShowToolTip = true;
      }

      public override MapModeType GetMapModeName()
      {
         return MapModeType.BadApple;
      }

      private void Timer_Tick(object? sender, EventArgs e)
      {
         var _scaleX = (float)Globals.MapWidth / frames[0].Width;
         var _scaleY = (float)Globals.MapHeight / frames[0].Height;

         _scale = MathF.Min(_scaleX, _scaleY);

         if (_scaleX > _scaleY)
            _offset = new((int)((Globals.MapWidth / _scale - frames[0].Width) / 2f), 0);
         else
            _offset = new(0, (int)((Globals.MapHeight / _scale - frames[0].Height) / 2f));

         RenderMapMode(GetProvinceColor);


         _frameIndex++;
         if (_frameIndex >= frames.Length)
            _frameIndex = 0;
      }

      public override int GetProvinceColor(Province id)
      {
         lock (frames)
         {
            var currentframe = frames[_frameIndex];
            // Bounds Checks
            var centerX = (int)((id.Center.X) / _scale - _offset.X);
            var centerY = (int)((id.Center.Y) / _scale - _offset.Y);
            if (centerX >= currentframe.Width || centerY >= currentframe.Height || centerX < 0 || centerY < 0)
               return Color.DimGray.ToArgb();
            return currentframe.GetPixel(centerX, centerY).ToArgb();
         }
      }

      public override string GetSpecificToolTip(Province provinceId)
      {
         return "Bad Apple";
      }
   }
}