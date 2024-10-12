using Editor.Controls;
using Editor.DataClasses.GameDataClasses;
using Editor.Helper;

namespace Editor.Commands;

public class CAddSingleSelection : ICommand
{
   private readonly Province _provinceId;
   public CAddSingleSelection (Province provinceId, bool executeOnInit = true)
   {
      _provinceId = provinceId;

      if (executeOnInit) 
         Execute();
   
   }
   //TODO fix ids
   public void Execute()
   {
      Selection.AddProvinceToSelection(_provinceId);
   }

   public void Undo()
   {
     Selection.RemoveProvinceFromSelection(_provinceId);
   }

   public void Redo()
   {
      Execute();
   }

   public string GetDescription() => $"Add province [{_provinceId.GetLocalisation()}] to selection";
}