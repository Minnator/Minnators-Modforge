using System.Diagnostics;
using System.Numerics;
using Editor.DataClasses.GameDataClasses;
using Editor.Helper;
using Timer = System.Windows.Forms.Timer;

namespace Editor.DataClasses.MapModes
{
   public class RGBMapMode : MapMode
   {
      private readonly Timer _timer = new();
      private int _totalRunTime = 0;
      private float _scaledTime = 0f;

      public enum RGBMapModeType
      {
         Diagonal,
         Horizontal,
         Vertical,
         Random,
         Rotating
      }

      public RGBMapMode()
      {
      }

      public override void SetActive()
      {
         _timer.Interval = Globals.Settings.Rendering.MsTimerIntervalMapModeTimer;
         _timer.Tick += Timer_Tick;
         _totalRunTime = 0;
         _timer.Start();
         Globals.Settings.ToolTip.ShowToolTip = false;
      }

      public override void SetInactive()
      {
         _timer.Stop();
         _timer.Tick -= Timer_Tick;
         Globals.Settings.ToolTip.ShowToolTip = true;
      }

      private void Timer_Tick(object? sender, EventArgs e)
      {
         _totalRunTime += _timer.Interval;
         _scaledTime = _totalRunTime / 3000f;
         RenderMapMode(GetProvinceColor);
         _timer.Interval = Globals.Settings.Rendering.MsTimerIntervalMapModeTimer;
      }

      public override MapModeType GetMapModeName()
      {
         return MapModeType.RGB;
      }

      public override int GetProvinceColor(Province id)
      {
         var center = id.Center;
         switch (Globals.Settings.Rendering.RGBMapModeType)
         {
            case RGBMapModeType.Diagonal:
               return HSVToRGBSmooth((center.X + center.Y) / 6000f + _scaledTime, 1, 1);
            case RGBMapModeType.Horizontal:
               return HSVToRGBSmooth((center.X) / 3000f + _scaledTime, 1, 1);
            case RGBMapModeType.Vertical:
               return HSVToRGBSmooth((center.Y) / 3000f + _scaledTime, 1, 1);
            case RGBMapModeType.Random:
               var temp = center.X + 12.9898f + center.Y * 78.233f;
               temp = MathF.Sin(temp) * 43758.5453123f;
               temp -= MathF.Floor(temp);
               return HSVToRGBSmooth(temp + _scaledTime, 1, 1);
            case RGBMapModeType.Rotating:
               return HSVToRGBSmooth((MathF.Cos(_scaledTime) * center.X + MathF.Sin(_scaledTime) * center.Y) / 3000, 1, 1);
            default:
               throw new ArgumentOutOfRangeException();
         }
         return 0x000000;
      }

      // Utility function for modulus with float
      private static float Mod(float x, float y)
      {
         return x - y * (float)Math.Floor(x / y);
      }

      static int HSVToRGBSmooth(float h, float s, float v)
      {
         var r = Geometry.Clamp(MathF.Abs(Mod(h * 6.0f + 0.0f , 6.0f) - 3.0f) - 1.0f, 0.0f, 1.0f);
         var g = Geometry.Clamp(MathF.Abs(Mod(h * 6.0f + 4.0f , 6.0f) - 3.0f) - 1.0f, 0.0f, 1.0f);
         var b = Geometry.Clamp(MathF.Abs(Mod(h * 6.0f + 2.0f , 6.0f) - 3.0f) - 1.0f, 0.0f, 1.0f);

         //Debug.WriteLine($"Color: {r}/{g}/{b}");
         r = r * r * (3.0f - 2.0f * r);
         g = g * g * (3.0f - 2.0f * g);
         b = b * b * (3.0f - 2.0f * b);

         r = v * (1.0f - s * (1.0f - r));
         g = v * (1.0f - s * (1.0f - g));
         b = v * (1.0f - s * (1.0f - b));

         // return an int representation of the color
         return ((int)(r * 255) << 16) | ((int)(g * 255) << 8) | (int)(b * 255);
      }

      public override string GetSpecificToolTip(Province provinceId)
      {
         return $"Color: {GetProvinceColor(provinceId)}";
      }
   }
}