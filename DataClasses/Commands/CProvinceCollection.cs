using Editor.Commands;
using Editor.Interfaces;

namespace Editor.DataClasses.Commands
{

   public enum ProvinceCollectionType
   {
      Add,
      Remove,
      Modify
   }

   public class CProvinceCollection<T>(ProvinceCollectionType type, List<ProvinceComposite> composites, List<Color> colors) : ICommand where T : ProvinceComposite
   {

      public ProvinceCollectionType Type = type;
      public List<ProvinceComposite> Composites = composites;
      public ProvinceCollection<T> oldParent;
      public ProvinceCollection<T> newParent;

      public CProvinceCollection(ProvinceCollectionType type, List<ProvinceComposite> composites, List<Color> colors, bool executeOnInit = true) : this(type, composites, colors)
      {
         if (executeOnInit)
            Execute();
      }

      public void Execute()
      {
         switch (type)
         {
            case ProvinceCollectionType.Add:
               //newParent.AddInt(composites);
               break;

         }
      }

      public void Undo()
      {
         switch (type)
         {
            case ProvinceCollectionType.Add:
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