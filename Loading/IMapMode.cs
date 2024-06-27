using System;
using System.Drawing;

namespace Editor.Loading;

public interface IMapMode
{
   public Bitmap Bitmap { get; set; }

   public void RenderMapMode(Func<int, Color> method);
   public string GetMapModeName();
}