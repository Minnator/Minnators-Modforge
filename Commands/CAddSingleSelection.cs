using Editor.Controls;

namespace Editor.Commands;

public class CAddSingleSelection : ICommand
{
   private readonly int _provinceId;
   private readonly PannablePictureBox _pb;
   public CAddSingleSelection (int provinceId, PannablePictureBox pb, bool executeOnInit = true)
   {
      _provinceId = provinceId;
      _pb = pb;

      if (executeOnInit) 
         Execute();
   
   }

   public void Execute()
   {
      _pb.Selection.Add(_provinceId);
   }

   public void Undo()
   {
      _pb.Selection.Remove(_provinceId);
   }

   public void Redo()
   {
      Execute();
   }

   public string GetDescription() => $"Add province [{Data.Provinces[_provinceId].GetLocalisation()}] to selection";
}