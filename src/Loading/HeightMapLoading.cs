using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using Editor.Helper;

namespace Editor.Loading
{
   public static class HeightMapLoading
   {
      public static unsafe void Load()
      {
         if (!FilesHelper.GetModOrVanillaPath(out var path, out var isModPath, "map", "world_normal.bmp"))
         {
            Globals.ErrorLog.Write("Could not find heightmap.bmp");
            return;
         }

         if (!FilesHelper.GetModOrVanillaPath(out var file, out _, "map", "terrain.bmp"))
         {
            Globals.ErrorLog.Write("Could not find terrain.bmp");
            return;
         }
         using var sourceheightMap = new Bitmap(path);
         using var heightMap = new Bitmap(Globals.MapWidth, Globals.MapHeight, PixelFormat.Format24bppRgb);
         var g = Graphics.FromImage(heightMap);
         g.InterpolationMode = InterpolationMode.HighQualityBicubic;
         g.DrawImage(sourceheightMap, 0, 0, Globals.MapWidth, Globals.MapHeight);
         //ApplyLighting(heightMap, new Bitmap(file), 2800, 400, 1, 5).Save("result.png", ImageFormat.Png);
         var sw = System.Diagnostics.Stopwatch.StartNew();
         //ApplyLightingSphere(heightMap, new Bitmap(file), 0.1f, 0.1f, 1, 5).Save("result.png", ImageFormat.Png);
         //ApplyLightingSphere(heightMap, new Bitmap(file), 0.1f, 0.1f, 2, 5).Save("result1.png", ImageFormat.Png);
         //ApplyLightingSphere(heightMap, new Bitmap(file), 0.1f, 0.1f, 2, 10).Save("result2.png", ImageFormat.Png);
         //ApplyLightingSphere(heightMap, new Bitmap(file), 0.1f, 0.1f, 1, 10).Save("result3.png", ImageFormat.Png);
         ApplyLightingSphere(heightMap, new Bitmap(file), 0.1f, 0.1f, -0.005f, 1).Save("result4.png", ImageFormat.Png);
         sw.Stop();
         Debug.WriteLine($"Rendering: {sw.ElapsedMilliseconds}");
         /*
         var heightMapData = heightMap.LockBits(new(0, 0, heightMap.Width, heightMap.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
         var heightMapStride = heightMapData.Stride;
         var heightMapScan0 = (byte*)heightMapData.Scan0;
         var heightMapPalette = heightMap.Palette.Entries;

         using var terrainBmp = new Bitmap(file);
         var terrainBmpData = terrainBmp.LockBits(new(0, 0, terrainBmp.Width, terrainBmp.Height), ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);
         var terrainStride = terrainBmpData.Stride;
         var terrainScan0 = terrainBmpData.Scan0;
         var terrainPalette = terrainBmp.Palette.Entries;
         
         using var resultMap = new Bitmap(heightMap.Width, heightMap.Height);
         var resultMapData = resultMap.LockBits(new(0, 0, resultMap.Width, resultMap.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
         var resultMapStride = resultMapData.Stride;
         var resultMapScan0 = resultMapData.Scan0;


         for (var x = 0; x < heightMap.Width; x++)
         {
            for (var y = 0; y < heightMap.Height; y++)
            {
               byte* line = heightMapScan0 + y * heightMapStride;
               var terrainMapColor = terrainPalette[*((byte*)terrainScan0 + x + y * terrainStride)];
               //var heightMapColor = heightMapPalette[*((byte*)heightMapScan0 + offset)];
               var factor = line[x* 3 + 2] / 255.0;
               //var factor = 1;
               var argb = terrainMapColor.A << 24 | (int)(terrainMapColor.R * factor) << 16 | (int)(terrainMapColor.G * factor) << 8 | (int)(terrainMapColor.B * factor);
               *((int*)resultMapScan0 + x + y * resultMapStride / 4) = argb;
               //*((int*)resultMapScan0 + x + y * resultMapStride / 4) = terrainMapColor;
            }
         }

         heightMap.UnlockBits(heightMapData);
         terrainBmp.UnlockBits(terrainBmpData);
         resultMap.UnlockBits(resultMapData);
         resultMap.Save("result.png", ImageFormat.Png);
         */

      }

      static Bitmap ApplyLightingSphere(Bitmap normalMap, Bitmap texture, float lightX, float lightY, float lightZ, int shininess)
      {
         if (normalMap.PixelFormat != PixelFormat.Format24bppRgb)
            throw new ArgumentException("Normal map must be 24bpp.");
         if (texture.PixelFormat != PixelFormat.Format8bppIndexed)
            throw new ArgumentException("Texture must be 8bpp indexed.");

         Bitmap litTexture = new Bitmap(texture.Width, texture.Height, PixelFormat.Format32bppArgb);
         ColorPalette palette = texture.Palette;

         // Normalize the light vector
         float lightMag = (float)Math.Sqrt(lightX * lightX + lightY * lightY + lightZ * lightZ);
         lightX /= lightMag;
         lightY /= lightMag;
         lightZ /= lightMag;

         int width = texture.Width;
         int height = texture.Height;

         // Lock bitmaps for direct memory access
         BitmapData normalData = normalMap.LockBits(new Rectangle(0, 0, normalMap.Width, normalMap.Height),
             ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
         BitmapData textureData = texture.LockBits(new Rectangle(0, 0, texture.Width, texture.Height),
             ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);
         BitmapData litData = litTexture.LockBits(new Rectangle(0, 0, texture.Width, texture.Height),
             ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

         unsafe
         {
            byte* normalPtr = (byte*)normalData.Scan0;
            byte* texturePtr = (byte*)textureData.Scan0;
            byte* litPtr = (byte*)litData.Scan0;

            int normalStride = normalData.Stride;
            int textureStride = textureData.Stride;
            int litStride = litData.Stride;

            for (int y = 0; y < height; y++)
            {
               // Compute latitude (theta) from Mercator projection
               float v = (float)y / height;
               float theta = (float)(Math.PI * v - Math.PI / 2); // Latitude
               float cosTheta = (float)Math.Cos(theta);
               float sinTheta = (float)Math.Sin(theta);

               for (int x = 0; x < width; x++)
               {
                  // Compute longitude (phi) from Mercator projection
                  float u = (float)x / width;
                  float phi = (float)(2 * Math.PI * u - Math.PI); // Longitude
                  float cosPhi = (float)Math.Cos(phi);
                  float sinPhi = (float)Math.Sin(phi);

                  // Get normal from the normal map
                  byte* normalPixel = normalPtr + y * normalStride + x * 3;
                  float nx = (normalPixel[2] / 255f) * 2 - 1; // Red channel (X-axis)
                  float ny = (normalPixel[1] / 255f) * 2 - 1; // Green channel (Y-axis)
                  float nz = (normalPixel[0] / 255f);         // Blue channel (Z-axis)

                  // Normalize the normal vector
                  float normalMag = (float)Math.Sqrt(nx * nx + ny * ny + nz * nz);
                  if (normalMag > 0)
                  {
                     float invNormalMag = 1.0f / normalMag;
                     nx *= invNormalMag;
                     ny *= invNormalMag;
                     nz *= invNormalMag;
                  }

                  // Compute diffuse lighting
                  float diffuse = Math.Max(0, nx * lightX + ny * lightY + nz * lightZ);

                  // Simplified specular lighting (camera pointing vertically)
                  float hx = lightX;
                  float hy = lightY;
                  float hz = lightZ + 1; // Halfway vector
                  float halfMag = (float)Math.Sqrt(hx * hx + hy * hy + hz * hz);
                  hx /= halfMag;
                  hy /= halfMag;
                  hz /= halfMag;

                  float specular = (float)Math.Pow(Math.Max(0, nx * hx + ny * hy + nz * hz), shininess);

                  // Total intensity
                  float intensity = diffuse + specular;

                  // Get the texture color
                  byte* texturePixel = texturePtr + y * textureStride + x;
                  byte originalIndex = *texturePixel;
                  Color originalColor = palette.Entries[originalIndex];

                  // Apply lighting to the color
                  int newR = Math.Clamp((int)(originalColor.R * intensity), 0, 255);
                  int newG = Math.Clamp((int)(originalColor.G * intensity), 0, 255);
                  int newB = Math.Clamp((int)(originalColor.B * intensity), 0, 255);

                  // Write to the 32bpp output bitmap
                  byte* litPixel = litPtr + y * litStride + x * 4;
                  litPixel[0] = (byte)newB; // Blue channel
                  litPixel[1] = (byte)newG; // Green channel
                  litPixel[2] = (byte)newR; // Red channel
                  litPixel[3] = 255;        // Alpha channel (opaque)
               }
            }
         }

         // Unlock bitmaps
         normalMap.UnlockBits(normalData);
         texture.UnlockBits(textureData);
         litTexture.UnlockBits(litData);

         return litTexture;
      }

      static Bitmap ApplyLighting(Bitmap normalMap, Bitmap texture, float lightX, float lightY, float lightZ,
         int shininess)
      {
         if (normalMap.PixelFormat != PixelFormat.Format24bppRgb)
            throw new ArgumentException("Normal map must be 24bpp.");
         if (texture.PixelFormat != PixelFormat.Format8bppIndexed)
            throw new ArgumentException("Texture must be 8bpp indexed.");

         Bitmap litTexture = new Bitmap(texture.Width, texture.Height, PixelFormat.Format32bppArgb);
         ColorPalette palette = texture.Palette;

         // Lock bitmaps for direct memory access
         BitmapData normalData = normalMap.LockBits(new Rectangle(0, 0, normalMap.Width, normalMap.Height),
            ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
         BitmapData textureData = texture.LockBits(new Rectangle(0, 0, texture.Width, texture.Height),
            ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);
         BitmapData litData = litTexture.LockBits(new Rectangle(0, 0, litTexture.Width, litTexture.Height),
            ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

         unsafe
         {
            byte* normalPtr = (byte*)normalData.Scan0;
            byte* texturePtr = (byte*)textureData.Scan0;
            byte* litPtr = (byte*)litData.Scan0;

            int normalStride = normalData.Stride;
            int textureStride = textureData.Stride;
            int litStride = litData.Stride;

            // Precompute light vector normalization constant
            float lightMag = (float)Math.Sqrt(lightX * lightX + lightY * lightY + lightZ * lightZ);
            float invLightMag = 1.0f / lightMag;
            float lzNorm = lightZ * invLightMag;

            for (int y = 0; y < normalMap.Height; y++)
            {
               // Compute row-specific values for light vector
               float ly = lightY - y;
               float lyNorm = ly * invLightMag;

               for (int x = 0; x < normalMap.Width; x++)
               {
                  // Compute column-specific light vector component
                  float lx = lightX - x;
                  float lxNorm = lx * invLightMag;

                  // Get normal vector from the normal map
                  byte* normalPixel = normalPtr + y * normalStride + x * 3;
                  float nx = (normalPixel[2] / 255f) * 2 - 1; // Red channel (X-axis)
                  float ny = (normalPixel[1] / 255f) * 2 - 1; // Green channel (Y-axis)
                  float nz = (normalPixel[0] / 255f); // Blue channel (Z-axis)

                  // Normalize the normal vector (avoiding division if already normalized)
                  float normalMag = (float)Math.Sqrt(nx * nx + ny * ny + nz * nz);
                  if (normalMag > 0)
                  {
                     float invNormalMag = 1.0f / normalMag;
                     nx *= invNormalMag;
                     ny *= invNormalMag;
                     nz *= invNormalMag;
                  }

                  // Compute diffuse intensity
                  float diffuse = Math.Max(0, nx * lxNorm + ny * lyNorm + nz * lzNorm);

                  // Simplified specular highlight
                  float hx = lxNorm;
                  float hy = lyNorm;
                  float hz = lzNorm + 1; // Halfway vector assuming camera points vertically
                  float halfMag = (float)Math.Sqrt(hx * hx + hy * hy + hz * hz);
                  float invHalfMag = 1.0f / halfMag;
                  hx *= invHalfMag;
                  hy *= invHalfMag;
                  hz *= invHalfMag;

                  float specular = (float)Math.Pow(Math.Max(0, nx * hx + ny * hy + nz * hz), shininess);

                  // Total light intensity
                  float intensity = diffuse + specular;

                  // Get the texture color
                  byte* texturePixel = texturePtr + y * textureStride + x;
                  byte originalIndex = *texturePixel;
                  Color originalColor = palette.Entries[originalIndex];

                  // Apply lighting to the color
                  int newR = Math.Clamp((int)(originalColor.R * intensity), 0, 255);
                  int newG = Math.Clamp((int)(originalColor.G * intensity), 0, 255);
                  int newB = Math.Clamp((int)(originalColor.B * intensity), 0, 255);

                  // Write to the 32bpp output bitmap
                  byte* litPixel = litPtr + y * litStride + x * 4;
                  litPixel[0] = (byte)newB; // Blue channel
                  litPixel[1] = (byte)newG; // Green channel
                  litPixel[2] = (byte)newR; // Red channel
                  litPixel[3] = 255; // Alpha channel (opaque)
               }
            }
         }
         // Unlock bitmaps
         normalMap.UnlockBits(normalData);
         texture.UnlockBits(textureData);
         litTexture.UnlockBits(litData);

         return litTexture;
      }


      public static unsafe void AddNormalsLightning(ref Bitmap map, Bitmap normals)
      {
         if (map.Size != normals.Size)
            throw new ArgumentException("The bitmaps must have the same size");

         using var result = new Bitmap(map.Width, map.Height, PixelFormat.Format32bppArgb);
         var mapData = map.LockBits(new(0, 0, map.Width, map.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
         var normalsData = normals.LockBits(new(0, 0, normals.Width, normals.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
         var resultData = result.LockBits(new(0, 0, result.Width, result.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

         var mapStride = mapData.Stride;
         var normalsStride = normalsData.Stride;
         var resultStride = resultData.Stride;
         
         var mapScan0 = (byte*)mapData.Scan0;
         var normalsScan0 = (byte*)normalsData.Scan0;
         var resultScan0 = (byte*)resultData.Scan0;

         Parallel.For(0, result.Width, x =>
         {
            for (var y = 0; y < normals.Height; y++)
            {
               byte* line = normalsScan0 + y * normalsStride;
               var terrainMapColor = Color.FromArgb(*((int*)mapScan0 + x + y * mapStride));
               //var heightMapColor = heightMapPalette[*((byte*)heightMapScan0 + offset)];
               var factor = line[x * 3 + 2] / 255.0;
               //var factor = 1;
               var argb = terrainMapColor.A << 24 | (int)(terrainMapColor.R * factor) << 16 | (int)(terrainMapColor.G * factor) << 8 | (int)(terrainMapColor.B * factor);
               *((int*)resultScan0 + x + y * resultStride / 4) = argb;
               //*((int*)resultMapScan0 + x + y * resultMapStride / 4) = terrainMapColor;
            }
         });

         map.UnlockBits(mapData);
         normals.UnlockBits(normalsData);
         result.UnlockBits(resultData);

         map = result;
      }
   }
}