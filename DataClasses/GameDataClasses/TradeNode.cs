using System.Net;
using System.Text;
using Editor.DataClasses.Commands;
using Editor.Helper;
using Editor.Interfaces;
using Editor.Savers;

namespace Editor.DataClasses.GameDataClasses
{
   public class TradeNode(string name, Color color, Province location) : ProvinceCollection<Province>(name, color)
   {
      public Province Location { get; set; } = location;
      public bool IsInland { get; set; } = false;
      public HashSet<Province> Members { get; set; } = [];
      public List<string> Incoming { get; set; } = [];
      public List<Outgoing> Outgoing { get; set; } = [];
      public static TradeNode Empty => new ("", Color.Empty, Province.Empty);

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

      public override ModifiedData WhatAmI()
      {
         return ModifiedData.TradeNode;
      }

      public override string SavingComment()
      {
         return Localisation.GetLoc(Name);
      }

      public override PathObj GetDefaultSavePath()
      {
         return new (["common", "tradenodes"]);
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
      public EventHandler<ProvinceComposite> ColorChanged = delegate { };
      public EventHandler<ProvinceComposite> ItemAddedToArea = delegate { };
      public EventHandler<ProvinceComposite> ItemRemovedFromArea = delegate { };
      public EventHandler<ProvinceComposite> ItemModified = delegate { };

      public override void Invoke(ProvinceComposite composite)
      {
         ColorChanged.Invoke(this, composite);
      }

      public override void AddToEvent(EventHandler<ProvinceComposite> handler)
      {
         ColorChanged += handler;
      }

      public override void Invoke(CProvinceCollectionType type, ProvinceComposite composite)
      {
         switch (type)
         {
            case CProvinceCollectionType.Add:
               ItemAddedToArea.Invoke(this, composite);
               break;
            case CProvinceCollectionType.Remove:
               ItemRemovedFromArea.Invoke(this, composite);
               break;
            case CProvinceCollectionType.Modify:
               ItemModified.Invoke(this, composite);
               break;
         }
      }

      public override void AddToEvent(CProvinceCollectionType type, EventHandler<ProvinceComposite> eventHandler)
      {
         switch (type)
         {
            case CProvinceCollectionType.Add:
               ItemAddedToArea += eventHandler;
               break;
            case CProvinceCollectionType.Remove:
               ItemRemovedFromArea += eventHandler;
               break;
            case CProvinceCollectionType.Modify:
               ItemModified += eventHandler;
               break;
         }
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