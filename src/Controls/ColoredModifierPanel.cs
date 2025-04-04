﻿using Editor.DataClasses.GameDataClasses;
using Editor.Parser;

namespace Editor.Controls
{
   public class ColoredModifierPanel : DualSelectionFlowPanel
   {
      public ColoredModifierPanel()
      {
         OnAfterAdd += AfterAddClick;
      }
      
      public void AddModifier(ModifierDefinition mod, string value, bool clear = true)
      {
         if (clear)
            FlowLayoutPanel.Controls.Clear();
         //SetDualContentInternal(mod.CodeName, value);
         FlowLayoutPanel.Controls.Add(GetDualContentPanel(mod.CodeName, value, FlowLayoutPanel.Width - 25));
         DualContent.Add(new(mod.Index.ToString(), value));

         // get the last control in the flowlayoutpanel
         var control = FlowLayoutPanel.Controls[^1];
         if (control is not TableLayoutPanel tlp || tlp.GetControlFromPosition(1, 0) is not Label right)
            return;

         right.ForeColor = mod.GetColor(value);
      }

      public void AddModifiers(List<ModifierDefinition> modifiers, List<string> values)
      {
         FlowLayoutPanel.Controls.Clear();
         DualContent.Clear();
         if (modifiers.Count != values.Count)
            return;
         for (var i = 0; i < modifiers.Count; i++)
         {
            var mod = modifiers[i];
            var value = values[i];

            AddModifier(mod, value, false);
         }

         var content = DualContent;
      }


      private void AfterAddClick(object? sender, TableLayoutPanel tlp)
      {
         if (tlp.GetControlFromPosition(1, 0) is not Label right || LeftComboBox.SelectedIndex < 0 || Indexing == IndexingType.None || Indexing == IndexingType.Right)
            return;

         right.ForeColor = ModifierParser.ModifierDefinitions[LeftComboBox.SelectedIndex].GetColor(right.Text);
      }


   }
}