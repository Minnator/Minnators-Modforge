using Editor.Controls;
using Editor.DataClasses.GameDataClasses;
using Editor.Helper;

namespace Editor.DataClasses.Commands
{
   public class CModifyExistingCountry : ICommand
   {
      private readonly Tag _tag;
      private readonly List<Province> _deltaIds = [];
      private readonly List<KeyValuePair<Province, Tag>> _oldOwner = []; // Tag = the old owner, controller and core as it is only set if all 3 match up
      private readonly bool _add;
      private CollectionEditor _collectionEditor = null!;

      public CModifyExistingCountry(string s, List<Province> ids, bool add, CollectionEditor collectionEditor, bool executeOnInit = true)
      {
         if (Tag.TryParse(s, out var tag))
            _tag = tag;
         else
         {
            MessageBox.Show($"Invalid country tag: \"{s}\"");
            return;
         }
         _deltaIds = ids;
         _add = add;
         _collectionEditor = collectionEditor;

         if (executeOnInit)
            Execute();
      }

      public void Execute()
      {
         if (!_tag.IsValid())
            return;
         // Setting the owner will update the CountryMapMode
         if (_add) // Remove old owner, and it's core, set new owner and new core
         {
            foreach (var id in _deltaIds)
            {
               if (!Globals.ProvinceIdToProvince.TryGetValue(id, out var province))
                  continue;
               if (province.Owner == _tag) // Already the owner
                  continue;

               var oldOwner = province.Owner;
               if (province.Controller != oldOwner || !province.Cores.Contains(oldOwner) && oldOwner != Tag.Empty) // Owner, Controller and Core must match
                  continue;
                     
               _oldOwner.Add(new (province, oldOwner)); // Save the old owner, controller and core

               province.Cores.Remove(oldOwner); // Remove the old owner from the core list
               province.Cores.Add(_tag);
               province.Owner = _tag; 
               province.Controller = _tag; // This will update the CountryMapMode
            }
         }
         else
         {
            foreach (var id in _deltaIds)
            {
               if (!Globals.Provinces.TryGetValue(id, out var province))
                  continue;
               var oldOwner = province.Owner;
               
               if (oldOwner != _tag) // Not the owner
                  continue;

               if (province.Controller != oldOwner || !province.Cores.Contains(oldOwner)) // Owner, Controller and Core must match
                  continue;

               province.Owner = Tag.Empty; 
               province.Cores.Remove(_tag); // Remove the owner from the core list
               province.Controller = Tag.Empty; // This will update the CountryMapMode
            }
         }
      }

      public void Undo()
      {
         foreach (var (id, oldOwner) in _oldOwner)
         {
            if (!Globals.Provinces.TryGetValue(id, out var province) || !oldOwner.IsValid())
               continue;
            province.Owner = oldOwner;
            province.Cores.Add(oldOwner);
            province.Controller = oldOwner;
         }
      }

      public void Redo()
      {
         Execute();

         // Update the collection editor
         if (_add)
         {
            List<string> provinceNames = [];
            foreach (var id in _deltaIds)
               provinceNames.Add(id.ToString());
            _collectionEditor.AddItemsUnique(provinceNames);
         }
         else
         {
            List<string> provinceNames = [];
            foreach (var id in _deltaIds)
               provinceNames.Add(id.ToString());
            _collectionEditor.RemoveItems(provinceNames);
         }
      }

      public string GetDescription()
      {
         return _add ? $"Add {_tag} to {_deltaIds.Count} provinces" : $"Remove {_tag} from {_deltaIds.Count} provinces";
      }

   }
}