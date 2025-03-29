using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using Editor.DataClasses.GameDataClasses;
using Editor.Helper;
using Editor.Loading.Enhanced.PCFL.Implementation;
using Editor.Saving;
using Windows.Devices.Power;
using Editor.ErrorHandling;
using Editor.Parser;
using Windows.UI.Popups;

namespace Editor.Loading.Enhanced
{
   public class ScriptedEffectUsage : IToken
   {
      private IToken[] _tokens;
      public List<LineKvp<string, string>> ReplacementValues { get; init; }
      public string Name { get; init; }

      public void Activate(ITarget target)
      {
         foreach (var token in _tokens)
         {
            token.Activate(target);
         }
      }

      public void GetTokenString(int tabs, ref StringBuilder sb)
      {
         // Add effect: effect_name = yes
         if (ReplacementValues.Count == 0)
            SavingUtil.AddBool(tabs, true, Name, ref sb);
         else
         {
            // Add effect: effect_name = {
            //    key = value
            //    ...
            // }
            SavingUtil.OpenBlock(ref tabs, Name, ref sb);
            foreach (var kvp in ReplacementValues) 
               SavingUtil.AddLineKvp(tabs, kvp, ref sb);
            SavingUtil.CloseBlock(ref tabs, ref sb);
         }
      }

      public string GetTokenName() => Name;

      public string GetTokenDescription()
      {
         return $"Call of scripted effect \"{Name}\"";
      }

      public string GetTokenExample()
      {
         return "effect = yes \nor\neffect = { \n\t<replaceKey> = value \n\t... \n}";
      }
   }

   public partial class ScriptedEffectImpl
   {
      public string name;
      public Dictionary<string, int[]> replacePos;
      public string effect; // with all references $...$ removed

      // Save the string
      // With possibility of replacement => easier in string or stringbuilder

      // remove all references $...$
      // name => pos in string
      // insert at pos
      // count length of inserted string
      // insert next at pos + count

      public IToken? CreateEffect(EnhancedBlock? block, LineKvp<string, string>? kvp, ParsingContext context, PathObj po)
      {
         if (block is not null)
         {
            // We have a block call which indicates that there has to be at least one argument in the blocks content elements
            if (block.Count == 0)
            {
               _ = new LoadingError(po, $"Scripted effect '{name}' requires at least one argument in block call.");
               return null;
            }

            var kvps = block.GetContentLines();
            if (ReplaceValues(kvps, po, out var replaced))
            {
               return new ScriptedEffectUsage
               {
                  // TODO parse 'replaced' to tokens
                  Name = name,
                  ReplacementValues = kvps
               };
            }
         }
         else
         {
            // we have a kvp call which indicates that the value has to be 'yes' as no is not allowed
            if (kvp is null || kvp.Value.Value.ToLower().Equals("yes"))
            {
               _ = new LoadingError(po, $"Scripted effect '{name}' requires 'yes' as value.");
               return null;
            }

            return new ScriptedEffectUsage
            {
               // TODO parse 'replaced' to tokens
               Name = name,
               ReplacementValues = []
            };
         }

         // in case of block check all replace values
         // in case of kvp check if value == yes and no replace values 
         // Returns list of parsed tokens of scripted Effect as ScriptedEffectUsage
         // Throw error if parsing fails due to wrong context

         return new ScriptedEffectUsage();
      }

      // Contains the key and the index to the replacement value in a backwards order
      private List<(string key, int index)> _replaceSources = [];

      private void SetReplaceSources(List<LineKvp<string, string>> values, PathObj po)
      {
         var expectedValues = new string[values.Count];

         for (var i = 0; i < values.Count; i++)
            expectedValues[i] = values[i].Key;

         var matches = ReplaceRegex().Matches(effect);

         for (var i = matches.Count - 1; i >= 0; i--)
         {
            var match = matches[i];
            if (expectedValues.Contains(match.Value))
            {
               // the value is one of the expected values
               // we trim the $ to be able to use .Equals() in later usage
               _replaceSources.Add((match.Value[1..^1], match.Index));
            }
            else
            {
               _ = new LoadingError(po, $"Unknown argument '{match.Value}' detected in scripted effect '{name}' call.");
            }
         }
      }

      public bool ReplaceValues(List<LineKvp<string, string>> values, PathObj po, out string replaced)
      {
         Debug.Assert(values.Count != 0, "This should never be called for non block scripted effects!");

         // The Scripted effect has not yet been called
         if (_replaceSources.Count != values.Count)
            SetReplaceSources(values, po);

         var sb = new StringBuilder(effect);
         var returnVal = true;

         // The list is backwards so we can go forward and just replace the values
         for (var i = 0; i < _replaceSources.Count; i++)
         {
            var (key, index) = _replaceSources[i];
            var replacement = values.FirstOrDefault(x => x.Key.Equals(key));
            if (replacement.Key == null)
            {
               _ = new LoadingError(po, $"Missing argument in scripted effect ({name}) call: '{key}'");
               returnVal = false;
               continue;
            }
            sb.Remove(index, key.Length).Insert(index, replacement.Value);
         }

         replaced = sb.ToString();
         return returnVal;
      }

      [GeneratedRegex(@"\$\w+\$")]
      private static partial Regex ReplaceRegex();
   }
}