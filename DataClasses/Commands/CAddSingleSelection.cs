using Editor.Controls;
using Editor.Helper;

namespace Editor.Commands;

public class CAddSingleSelection : ICommand
{
   private readonly int _provinceId;
   public CAddSingleSelection (int provinceId, bool executeOnInit = true)
   {
      _provinceId = provinceId;

      if (executeOnInit) 
         Execute();
   
   }
   //TODO fix ids
   public void Execute()
   {
      Selection.AddProvinceToSelection(Globals.Provinces[_provinceId]);
   }

   public void Undo()
   {
     Selection.RemoveProvinceFromSelection(Globals.Provinces[_provinceId]);
   }

   public void Redo()
   {
      Execute();
   }

   public string GetDescription() => $"Add province [{Globals.Provinces[_provinceId].GetLocalisation()}] to selection";
}