using System.Diagnostics;
using Editor.Controls;
using Editor.Helper;

namespace Editor.Commands;

public class CSelectionMarkNext : ICommand
{
   private readonly int _provinceId;
   private int[] _selectedProvinces = null!;
   public CSelectionMarkNext(int provinceId, bool executeOnInit = true)
   {
      _provinceId = provinceId;

      if (executeOnInit)
         Execute();
   }

   public void Execute()
   {
      _selectedProvinces = [.. Selection.GetSelectedProvincesIds];
      Selection.AddProvinceToSelection(Globals.Provinces[_provinceId], true);
   }

   public void Undo()
   {
      Selection.ClearSelection();
      Selection.AddProvincesToSelection(_selectedProvinces);
   }

   public void Redo()
   {
      Execute();
   }

   public string GetDescription() => $"Mark province [{Globals.Provinces[_provinceId].GetLocalisation()}] as selected";
}