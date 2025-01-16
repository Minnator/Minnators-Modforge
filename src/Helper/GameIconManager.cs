
using System.Diagnostics;
using System.Drawing.Imaging;
using Editor.ErrorHandling;
using Editor.Saving;

namespace Editor.Helper
{

   [AttributeUsage(AttributeTargets.Property)]
   public class GameIcon(GameIcons icon, bool showNumber = false) : Attribute
   {
      public GameIcons Icon { get; set; } = icon;
      public bool ShowNumber {get; set; } = showNumber;
   }

   public enum GameIcons
   {
      None = -1,
      Yes,
      No,
      Building,
      Core,
      Claim,
      DiscoverAchievement,
      AcceptedCultures,
   }


   public class GameIconDefinition
   {
      private Bitmap _icon;
      public static Dictionary<GameIcons, GameIconDefinition> Icons { get; set; } = new();
      
      public string[] IconPath { get; set; }

      public Bitmap Icon
      {
         get => _icon;
         set
         {
            _icon = ForcePadding(value, Globals.Settings.Rendering.IconTransparencyPadding);
         }
      }

      public GameIcons IconType { get; }

      public static void Initialize()
      {
         FromPath(GameIcons.DiscoverAchievement, "gfx", "interface", "achievements", "achievement_world_discoverer.dds");
         FromIconStrip(GameIcons.Yes, 0, 2, "gfx", "interface", "eligible_noneligible_strip.dds");
         FromIconStrip(GameIcons.No, 1, 2, "gfx", "interface", "eligible_noneligible_strip.dds");
         FromPath(GameIcons.None, "gfx", "interface", "default_fallback_texture.dds");
         FromPath(GameIcons.Building, "gfx", "interface", "technologyView_show_buildings.dds");
         FromPath(GameIcons.Core, "gfx", "interface", "core_icon.dds");
         FromPath(GameIcons.Claim, "gfx", "interface", "ideas_EU4", "fabricate_claims_cost.dds");
         FromPath(GameIcons.AcceptedCultures, "gfx", "interface", "accepted_cultures.dds");
      }

      public GameIconDefinition(GameIcons iconEnum)
      {
         IconPath = [];
         IconType = iconEnum;

         if (!Icons.TryAdd(iconEnum, this))
            Globals.ErrorLog.Write($"Error: Icon {iconEnum} already exists in Icon Dictionary!");
      }

      public static void CreateSpriteSheetPacked(List<Bitmap> icons, string outputPath)
      {
         if (icons == null || icons.Count == 0)
            throw new ArgumentException("The icons list is empty.", nameof(icons));

         // Packing layout variables
         const int spacing = 0;
         var currentX = 0;
         var rowHeight = 0;

         int maxWidth = 0, totalHeight = 0;

         foreach (var icon in icons)
         {
            if (currentX + icon.Width > maxWidth)
            {
               // Move to next row
               totalHeight += rowHeight + spacing;
               currentX = 0;
               rowHeight = 0;
            }

            rowHeight = Math.Max(rowHeight, icon.Height);
            currentX += icon.Width + spacing;
            maxWidth = Math.Max(maxWidth, currentX);
         }

         totalHeight += rowHeight; // Add height of the last row

         // Create the sprite sheet
         using var spriteSheet = new Bitmap(maxWidth, totalHeight, PixelFormat.Format32bppArgb);
         using (var g = Graphics.FromImage(spriteSheet))
         {
            g.Clear(Color.Transparent);

            currentX = 0;
            var currentY = 0;
            rowHeight = 0;

            // Draw each icon
            foreach (var icon in icons)
            {
               if (currentX + icon.Width > maxWidth)
               {
                  currentY += rowHeight + spacing;
                  currentX = 0;
                  rowHeight = 0;
               }

               g.DrawImage(icon, currentX, currentY, icon.Width, icon.Height);
               currentX += icon.Width + spacing;
               rowHeight = Math.Max(rowHeight, icon.Height);
            }
         }
         spriteSheet.Save(outputPath, ImageFormat.Png);
      }


      public static void UpdatePaddings()
      {
         foreach (var icon in Icons.Values)
         {
            icon.Icon = ForcePadding(icon.Icon, Globals.Settings.Rendering.IconTransparencyPadding);
         }
      }

      private static Bitmap ForcePadding(Bitmap bmp, int padding)
      {
         var imageRect = AnalyzePadding(ref bmp);
         
         // If the image is already padded correctly, return the original bitmap
            if (imageRect.X == padding && imageRect.Y == padding &&
             imageRect.Width == bmp.Width - 2 * padding &&
             imageRect.Height == bmp.Height - 2 * padding)
         {
            return bmp;
         }

         var newBmp = new Bitmap(imageRect.Width + 2 * padding, imageRect.Height + 2 * padding, PixelFormat.Format32bppArgb);

         using var g = Graphics.FromImage(newBmp);
         g.DrawImage(bmp, imageRect with { X = padding, Y = padding }, imageRect, GraphicsUnit.Pixel);

         bmp.Dispose(); 
         return newBmp; 
      }


      private static Rectangle AnalyzePadding(ref Bitmap bmp)
      {
         Debug.Assert(bmp.PixelFormat == PixelFormat.Format32bppArgb, "Only 32bpp images are supported for Padding analysis");

         var width = bmp.Width;
         var height = bmp.Height;

         int left = width, right = -1, top = height, bottom = -1;
         var foundPixel = false;

         bool foundTop = false, foundBottom = false, foundRight = false, foundLeft = false;

         var bitmapData = bmp.LockBits(
                                       new Rectangle(0, 0, width, height),
                                       ImageLockMode.ReadOnly,
                                       PixelFormat.Format32bppArgb);

         try
         {
            var stride = bitmapData.Stride;

            unsafe
            {
               var scan0 = (byte*)bitmapData.Scan0;

               // Spiral bounds
               int startX = 0, startY = 0;
               int endX = width - 1, endY = height - 1;

               while (startX <= endX && startY <= endY)
               {
                  if (foundTop && foundRight && foundBottom && foundLeft)
                     break;
                  
                  // Top edge
                  if (!foundTop)
                  {
                     for (var x = startX; x <= endX; x++)
                     {
                        if (IsNonTransparent(scan0, stride, x, startY))
                        {
                           foundTop = true;
                           foundPixel = true;
                           left = Math.Min(left, x);
                           right = Math.Max(right, x);
                           top = Math.Min(top, startY);
                        }
                     }
                  }

                  // Right edge
                  if (!foundRight)
                  {
                     for (var y = startY + 1; y <= endY; y++)
                     {
                        if (IsNonTransparent(scan0, stride, endX, y))
                        {
                           foundRight = true;
                           foundPixel = true;
                           right = Math.Max(right, endX);
                           bottom = Math.Max(bottom, y);
                        }
                     }
                  }

                  // Bottom edge
                  if (!foundBottom)
                  {
                     for (var x = endX - 1; x >= startX; x--)
                     {
                        if (IsNonTransparent(scan0, stride, x, endY))
                        {
                           foundBottom = true;
                           foundPixel = true;
                           left = Math.Min(left, x);
                           bottom = Math.Max(bottom, endY);
                        }
                     }
                  }

                  // Left edge

                  if (!foundLeft)
                  {
                     for (var y = endY - 1; y > startY; y--)
                     {
                        if (IsNonTransparent(scan0, stride, startX, y))
                        {
                           foundLeft = true;
                           foundPixel = true;
                           left = Math.Min(left, startX);
                           top = Math.Min(top, y);
                        }
                     }
                  }

                  // Move the spiral inward
                  startX++;
                  startY++;
                  endX--;
                  endY--;
               }
            }
         }
         finally
         {
            bmp.UnlockBits(bitmapData);
         }

         if (!foundPixel)
         {
            return Rectangle.Empty;
         }

         return new (left, top, right - left + 1, bottom - top + 1);
      }


      private static unsafe bool IsNonTransparent(byte* scan0, int stride, int x, int y) => (scan0 + y * stride + x * 4)[3] > 0; // Alpha channel

      public static Bitmap GetIcon(GameIcons iconEnum)
      {
         if (Icons.TryGetValue(iconEnum, out var icon))
            return icon.Icon;
         _ = new ErrorObject(ErrorType.INTERNAL_KeyNotFound, $"Trying to access a non existing game icon! {icon}", LogType.Critical);
         if (Icons.TryGetValue(GameIcons.None, out icon))
            return icon.Icon;
         throw new EvilActions($"Trying to access a non existing game icon! {icon}. FALLBACK icon not defined");
      }

      private static GameIconDefinition FromPath(GameIcons iconEnum, params string[] path)
      {
         if (!VerifyInputs(path, iconEnum))
            return null!;
         return new (iconEnum)
         {
            Icon = ReadImage(Path.Combine(Globals.VanillaPath, Path.Combine(path))),
            IconPath = path
         };
      }

      private static Bitmap ReadImage(string path) => ImageReader.ReadImage(path);

      private static bool VerifyInputs(string[] path, GameIcons iconEnum)
      {
         if (path.Length == 0)
         {
            Globals.ErrorLog.Write($"Error: Empty Icon Path {path}");
            return false;
         }
         if (Icons.TryGetValue(iconEnum, out _))
            return false;
         if (!path[^1].EndsWith(".dds") && !path[^1].EndsWith(".tga"))
         {
            Globals.ErrorLog.Write($"Error: Illegal Icon type {path}");
            return false;
         }
         return true;
      }

      private static void FromIconStrip(GameIcons iconEnum, int index, int framesCount, params string[] path)
      {
         if (!VerifyInputs(path, iconEnum))
            return;
         FilesHelper.GetVanillaPath(out var vPath, path);

         var rawImage = ReadImage(vPath);

         var icon = new Bitmap(rawImage.Width / framesCount, rawImage.Height);
         using var graphics = Graphics.FromImage(icon);
         graphics.DrawImage(rawImage, new Rectangle(0, 0, icon.Width, icon.Height), new (index * icon.Width, 0, icon.Width, icon.Height), GraphicsUnit.Pixel);

         GameIconDefinition _ = new (iconEnum)
         {
            Icon = icon,
            IconPath = path
         };

      }
   }
}