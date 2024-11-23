using System.Text;
using Editor.DataClasses.Misc;
using Editor.Helper;
using Editor.Saving;

namespace Editor.DataClasses.GameDataClasses
{
   public class TradeNode : ProvinceCollection<Province>
   {
      public TradeNode(string name, Color color, ObjEditingStatus status = ObjEditingStatus.Modified) : base(name, color, status)
      {
         Location = Province.Empty;
      }

      public TradeNode(string name, Color color, ref PathObj path, ICollection<Province> provinces, Province location) : base(name, color, ref path, provinces)
      {
         Location = location;
      }

      public override void OnPropertyChanged(string? propertyName = null) { }
      public Province Location { get; set; }
      public bool IsInland { get; set; } = false;
      public List<string> Incoming { get; set; } = [];
      public List<Outgoing> Outgoing { get; set; } = [];
      public new static TradeNode Empty => new (string.Empty, Color.Empty, ObjEditingStatus.Immutable);

      public override string ToString()
      {
         return Name;
      }

      public override bool Equals(object? obj)
      {
         if (obj is TradeNode node)
            return Name == node.Name;
         return false;
      }

      public override int GetHashCode()
      {
         return Name.GetHashCode();
      }

      public override SaveableType WhatAmI()
      {
         return SaveableType.TradeNode;
      }

      public override string[] GetDefaultFolderPath()
      {
         return ["common", "tradenodes"];
      }

      public override string GetFileEnding()
      {
         return ".txt";
      }

      public override KeyValuePair<string, bool> GetFileName()
      {
         return new("00_tradenodes", true);
      }

      public override string SavingComment()
      {
         return Localisation.GetLoc(Name);
      }

      public override string GetSaveString(int tabs)
      {
         StringBuilder sb = new();
         SaveTradeNodes.FormatTradeNode(this, ref sb);
         return sb.ToString();
      }

      public override string GetSavePromptString()
      {
         return $"Save trade node {Name}";
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
         Globals.TradeNodes.Remove(Name);
      }

      public override void AddGlobal()
      {
         Globals.TradeNodes.Add(Name, this);
      }
   }

   public class Outgoing(string target)
   {
      public string Target { get; set; } = target;
      public List<int> Path { get; set; } = [];
      public List<PointF> Control { get; set; } = [];
      public int SplitIndex = -1;
      public Rectangle Bounds { get; set; } = Rectangle.Empty;

      public void CalculateSplitIndex()
      {
      
      }

      public (List<PointF>, List<PointF>) GetSplitRoutes;

      public void CalculateBounds()
      {
         Bounds = Geometry.GetBoundsFloat(Control);
      }
   }
}