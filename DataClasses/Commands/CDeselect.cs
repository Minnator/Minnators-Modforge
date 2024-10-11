﻿using System.Collections.Generic;
using Editor.Controls;
using Editor.Helper;

namespace Editor.Commands;

public class CDeselectCommand :ICommand
{
   private readonly string _attr;
   private readonly object _value;
   private readonly List<int> _selectionDelta = [];
   
   public CDeselectCommand(string attr, object value, bool executeOnInit = true)
   {
      _attr = attr;
      _value = value;

      GetSelectionDelta(Selection.GetSelectedProvincesIds.ToList());

      if (executeOnInit)
         Execute();
   }

   private void GetSelectionDelta(List<int> selection)
   {
      if (int.TryParse(_value.ToString(), out var compareTo))
      {
         foreach (var i in selection)
         {
            if (int.TryParse(Globals.Provinces[i].GetAttribute(_attr)!.ToString(), out var compareResult))
               if (compareResult <= compareTo)
                  _selectionDelta.Add(i);
         }
      }
      else
      {
         foreach (var i in selection)
         {
            if (Globals.Provinces[i].GetAttribute(_attr)!.Equals(_value))
               _selectionDelta.Add(i);
         }
      }
   }

   public void Execute()
   {
      Selection.RemoveProvincesFromSelection(_selectionDelta);
   }

   public void Undo()
   {
      Selection.AddProvincesToSelection(_selectionDelta);
   }

   public void Redo()
   {
      Execute();
   }

   public string GetDescription()
   {
      return $"Deselected all Provinces with attribute [{_attr} = {_value}] in selection";
   }
}