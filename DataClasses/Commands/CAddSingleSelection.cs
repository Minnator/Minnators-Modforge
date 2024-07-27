using Editor.Controls;

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

   public void Execute()
   {
      Globals.Selection.Add(_provinceId);
   }

   public void Undo()
   {
     Globals.Selection.Remove(_provinceId);
   }

   public void Redo()
   {
      Execute();
   }

   public string GetDescription() => $"Add province [{Globals.Provinces[_provinceId].GetLocalisation()}] to selection";
}