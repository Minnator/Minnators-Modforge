using Editor.Controls;

namespace Editor.Commands;

public class CSelectionMarkNext : ICommand
{
   private readonly int _provinceId;
   private readonly PannablePictureBox _pb;
   private int[] _selectedProvinces = null!;
   public CSelectionMarkNext(int provinceId, PannablePictureBox pb, bool executeOnInit = true)
   {
      _provinceId = provinceId;
      _pb = pb;

      if (executeOnInit)
         Execute();

   }

   public void Execute()
   {
      _selectedProvinces = [.. _pb.Selection.SelectedProvinces];
      _pb.Selection.MarkNext(_provinceId);
   }

   public void Undo()
   {
      _pb.Selection.Clear();
      _pb.Selection.AddRange(_selectedProvinces);
   }

   public void Redo()
   {
      _pb.Selection.MarkNext(_provinceId);
   }

   public string GetDescription() => $"Mark province [{Data.Provinces[_provinceId].GetLocalisation()}] as selected";
}