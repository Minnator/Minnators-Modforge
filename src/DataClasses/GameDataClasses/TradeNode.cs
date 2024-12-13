using System.Text;
using Editor.DataClasses.Misc;
using Editor.Helper;
using Editor.Saving;

namespace Editor.DataClasses.GameDataClasses
{
   public class TradeNode : ProvinceCollection<Province>
   {
      public override bool GetSaveStringIndividually => false;

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
      public new static TradeNode Empty  { get; } = new (string.Empty, Color.Empty, ObjEditingStatus.Immutable);

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
         throw new EvilActions("Trade nodes can not be saved individually!");
      }

      public override string GetFullFileString(List<Saveable> changed, List<Saveable> unchanged)
      {
         StringBuilder sb = new();
         var nodesSorted = TradeNodeHelper.TopologicalSort(Globals.TradeNodes.Values.ToList());
         foreach (var node in nodesSorted)
         {
            if (Globals.Settings.Saving.AddCommentAboveObjectsInFiles)
               sb.Append("# ").Append(node.SavingComment()).AppendLine();
            SaveTradeNodes.FormatTradeNode(node, ref sb);
         }
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
         if (!Globals.TradeNodes.TryAdd(Name, this))
            MessageBox.Show($"The TradeNode {Name} does already exist and can not be created.", $"TradeNode {Name} already exists!", MessageBoxButtons.OK, MessageBoxIcon.Error);
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