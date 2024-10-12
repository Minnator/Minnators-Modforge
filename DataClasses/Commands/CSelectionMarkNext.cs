using System.Diagnostics;
using Editor.Controls;
using Editor.DataClasses.GameDataClasses;
using Editor.Helper;

namespace Editor.Commands;

public class CSelectionMarkNext : ICommand
{
   private readonly Province _provinceId;
   private Province[] _selectedProvinces = null!;
   public CSelectionMarkNext(Province provinceId, bool executeOnInit = true)
   {
      _provinceId = provinceId;

      if (executeOnInit)
         Execute();
   }

   public void Execute()
   {
      _selectedProvinces = [.. Selection.GetSelectedProvinces];
      Selection.AddProvinceToSelection(_provinceId, true);
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

   public string GetDescription() => $"Mark province [{_provinceId.GetLocalisation()}] as selected";
}