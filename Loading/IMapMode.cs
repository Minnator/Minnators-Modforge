using System;
using System.Collections.Generic;
using System.Drawing;

namespace Editor.Loading;

public interface IMapMode
{
   public Bitmap Bitmap { get; set; }

   public void RenderMapMode(Func<int, Color> method);
   public string GetMapModeName();
   public Color GetProvinceColor(int id);
   public void Update(Rectangle rect);
   public void Update(List<int> ids);
   public void Update(int id);
}