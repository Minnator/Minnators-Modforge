using System.Diagnostics;
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
      _selectedProvinces = [.. Globals.Selection.SelectedProvinces];
      Globals.Selection.MarkNext(_provinceId);
   }

   public void Undo()
   {
      Globals.Selection.Clear();
      Globals.Selection.AddRange(_selectedProvinces);
   }

   public void Redo()
   {
      Globals.Selection.MarkNext(_provinceId);
   }

   public string GetDescription() => $"Mark province [{Globals.Provinces[_provinceId].GetLocalisation()}] as selected";
}