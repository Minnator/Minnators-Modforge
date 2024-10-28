using Editor.Commands;
using Editor.Interfaces;

namespace Editor.DataClasses.Commands
{

   public enum CProvinceCollectionType
   {
      Add,
      Remove,
      Modify
   }

   public class CProvinceCollection<T>(CProvinceCollectionType type, List<ProvinceComposite> composites, List<Color> colors) : ICommand where T : ProvinceComposite
   {

      public CProvinceCollectionType Type = type;
      public List<ProvinceComposite> Composites = composites;
      public ProvinceCollection<T> oldParent;
      public ProvinceCollection<T> newParent;

      public CProvinceCollection(CProvinceCollectionType type, List<ProvinceComposite> composites, List<Color> colors, bool executeOnInit = true) : this(type, composites, colors)
      {
         if (executeOnInit)
            Execute();
      }

      public void Execute()
      {
         switch (type)
         {
            case CProvinceCollectionType.Add:
               //newParent.AddInt(composites);
               break;

         }
      }

      public void Undo()
      {
         switch (type)
         {
            case CProvinceCollectionType.Add:
               //newParent.RemoveInt(composites);
               break;

         }
      }

      public void Redo()
      {
         Execute();
      }

      public string GetDescription()
      {
         throw new NotImplementedException();
      }
   }
}