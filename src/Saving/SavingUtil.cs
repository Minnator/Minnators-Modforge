using System.Globalization;
using System.Text;
using Editor.DataClasses.GameDataClasses;
using Editor.DataClasses.Misc;
using Editor.DataClasses.Saveables;
using Editor.Loading.Enhanced.PCFL.Implementation;
using Editor.Parser;

namespace Editor.Saving
{
   public static class SavingUtil
   {
      public static int MaxLineWidth = 125;
      public static bool SortIntLists = true;
      public static bool SameSpacePerInt = true;
      public static bool OneLinePerString = true;

      public static void SaveValue<T>(string tokenName, T value, int tabs, ref StringBuilder sb)
      {
         switch (value)
         {
            case int i:
               AddInt(tabs, i, tokenName, ref sb);
               break;
            case float f:
               AddFloat(tabs, f, tokenName, ref sb);
               break;
            case bool b:
               AddBool(tabs, b, tokenName, ref sb);
               break;
            default:
            {
               if (value is bool c)
                  AddBool(tabs, c, tokenName, ref sb);
               else 
                  AddString(tabs, value?.ToString() ?? throw new EvilActions("Saving null is bad"), tokenName, ref sb);
               break;
            }
         }
      }

      public static void FormatTokens(List<IToken> tokens, int tabs, ref StringBuilder sb)
      {
         foreach (var effect in tokens)
            effect.GetTokenString(tabs, ref sb);
      }

      public static void FormatSimpleTokenBlock(List<IToken> tokens, int tabs, string name, ref StringBuilder sb)
      {
         OpenBlock(ref tabs, name, ref sb);
         FormatTokens(tokens, tabs, ref sb);
         CloseBlock(ref tabs, ref sb);
      }

      public static string GetYesNo(bool value)
      {
         return value ? "yes" : "no";
      }

      private static void AddFormattedList(string blockName, ICollection<string> strings, int tabCount, bool isNumber, ref StringBuilder sb)
      {
         AddTabs(tabCount, ref sb);

         sb.Append(blockName).Append(" = {\n");
         AddTabs(tabCount + 1, ref sb);
         var lineLength = 0;
         foreach (var i in strings)
         {
            if (lineLength > MaxLineWidth)
            {
               sb.Append('\n');
               AddTabs(tabCount + 1, ref sb);
               lineLength = 0;
            }

            if (isNumber && SameSpacePerInt)
            {
               sb.Append($"{i,4}").Append(' ');
               lineLength += 5;
            }
            else if (!isNumber && !OneLinePerString)
            {
               sb.AppendLine($"{i} ");
               AddTabs(tabCount + 1, ref sb);
            }
            else
            {
               sb.Append(i).Append(' ');
               lineLength += i.Length + 1;
            }
         }
         if (isNumber)
            sb.Append('\n');
         else if (!OneLinePerString)
            sb.Remove(sb.Length - 1, 1);
         else
            sb.Remove(sb.Length - 2, 2);
         AddTabs(tabCount, ref sb);
         sb.Append("}\n\n");
      }

      public static void AddFormattedIntList(string blockName, ICollection<int> ints, int tabCount, ref StringBuilder sb)
      {
         if (SortIntLists)
         {
            var intsList = ints.ToList();
            intsList.Sort();
            ints = intsList;
         }
         AddFormattedList(blockName, ints.Select(i => i.ToString()).ToList(), tabCount, true, ref sb);
      }

      public static void AddFormattedPointFList(string blockName, ICollection<PointF> floats, int tabCount, bool adjustHeight, ref StringBuilder sb)
      {
         List<string> strings = [];
         for (var i = 0; i < floats.ToList().Count; i++)
         {
            var f = floats.ToList()[i];
            strings.Add(f.X.ToString("F6", CultureInfo.InvariantCulture));
            if (adjustHeight)
               f.Y = Globals.MapHeight - f.Y;
            strings.Add(f.Y.ToString("F6", CultureInfo.InvariantCulture));
         }

         AddTabs(tabCount, ref sb);
         sb.AppendLine($"{blockName} = {{");
         var lineLength = 0;
         AddTabs(tabCount + 1, ref sb);
         foreach (var s in strings)
         {
            if (lineLength > MaxLineWidth)
            {
               sb.Append('\n');
               AddTabs(tabCount + 1, ref sb);
               lineLength = 0;
            }

            sb.Append($"{s,11}").Append(' ');
            lineLength += s.Length + 1;
         }

         sb.Append('\n');
         AddTabs(tabCount, ref sb);
         sb.AppendLine("}");
      }

      public static void AddFormattedStringListOnePerRow(string blockName, ICollection<string> strings, int tabCount,
         ref StringBuilder sb)
      {
         AddTabs(tabCount, ref sb);
         sb.AppendLine($"{blockName} = {{");
         foreach (var s in strings)
         {
            AddTabs(tabCount + 1, ref sb);
            sb.AppendLine(s);
         }
         AddTabs(tabCount, ref sb);
         sb.AppendLine("}");
      }

      public static void AddFormattedStringListAutoQuote(string blockName, List<string> strings, int tabCount, ref StringBuilder sb)
      {
         Dictionary<char, List<string>> groups = new (strings.Count);

         // Single-pass grouping: iterate over the list and assign each word to its group.
         foreach (string s in strings)
         {
            if (!string.IsNullOrEmpty(s))
            {
               // Convert the first character to uppercase using the invariant culture.
               var key = char.ToUpper(s[0], CultureInfo.InvariantCulture);

               if (!groups.TryGetValue(key, out List<string> list))
               {
                  list = new();
                  groups.Add(key, list);
               }
               list.Add(s);
            }
         }


         OpenBlock(ref tabCount, blockName, ref sb);
         foreach (var group in groups.Values) {
            group.Sort();
            sb.AppendLine();
            AddTabs(tabCount, ref sb);
            foreach (var s in group)
               if (s.IndexOf(' ') != -1)
                  sb.Append('\"').Append($"{s}").Append('\"').Append(' ');
               else
                  sb.Append($"{s}").Append(' ');
         }

         sb.AppendLine();
         CloseBlock(ref tabCount, ref sb);
      }

      public static void AddFormattedStringList(string blockName, ICollection<string> strings, int tabCount, ref StringBuilder sb)
      {
         AddFormattedList(blockName, strings, tabCount, false, ref sb);
      }

      public static void AddTabs(int tabCount, ref StringBuilder sb)
      {
         for (var i = 0; i < tabCount; i++) 
            sb.Append('\t');
      }

      public static void AddColor(int tabs, Color color, ref StringBuilder sb)
      {
         AddTabs(tabs, ref sb);
         sb.AppendLine($"color = {{ {color.R} {color.G} {color.B} }}");
      }

      public static void AddBool(int tabs, bool b, string boolName, ref StringBuilder sb)
      {
         AddTabs(tabs, ref sb);
         sb.AppendLine($"{boolName} = {GetYesNo(b)}");
      }

      public static void AddBoolIfYes(int tabs, bool b, string boolName, ref StringBuilder sb)
      {
         if (b)
            AddBool(tabs, b, boolName, ref sb);
      }

      public static void AddBoolIfNo(int tabs, bool b, string boolName, ref StringBuilder sb)
      {
         if (!b)
            AddBool(tabs, b, boolName, ref sb);
      }

      public static void AddInt(int tabs, int num, string intName, ref StringBuilder sb)
      {
         AddIntIfNotValue(tabs, num, intName, 0, ref sb);
      }

      public static void AddIntIfNotValue(int tabs, int num, string intName, int val, ref StringBuilder sb)
      {
         if (num != val)
         {
            AddTabs(tabs, ref sb);
            sb.AppendLine($"{intName} = {num}");
         }
      }

      public static void AddString(int tabs, string s, string stringName, ref StringBuilder sb)
      {
         if (string.IsNullOrEmpty(s))
            return;
         AddTabs(tabs, ref sb);
         sb.AppendLine($"{stringName} = {s}");
      }

      public static void AddQuotedString(int tabs, string s, string stringName, ref StringBuilder sb)
      {
         if (string.IsNullOrEmpty(s))
            return;
         AddTabs(tabs, ref sb);
         sb.AppendLine($"{stringName} = \"{s}\"");
      }

      public static void AddStringList(int tabs, string stringName, ICollection<string> strings, ref StringBuilder sb)
      {
         if (strings.Count == 0)
            return;
         foreach (var s in strings) 
            sb.AppendLine($"{stringName} = {s}");
      }

      public static void AddEffects(int tabs, List<IElement> effects, ref StringBuilder sb)
      {
         foreach (var effect in effects) 
            effect.FormatElement(tabs, ref sb);
      }

      public static void AddFloat(int tabs, float num, string floatName, ref StringBuilder sb)
      {
         if (num == 0)
            return;
         AddTabs(tabs, ref sb);
         sb.AppendLine($"{floatName} = {num.ToString("F2", CultureInfo.InvariantCulture)}");
      }

      public static void AddFloatIfNotValue(int tabs, float num, string floatName, float val, ref StringBuilder sb)
      {
         if (num != val)
         {
            AddTabs(tabs, ref sb);
            sb.AppendLine($"{floatName} = {num.ToString("F2", CultureInfo.InvariantCulture)}");
         }
      }
      public static void AddModifiers(int tabs, List<ISaveModifier> modifiers, ref StringBuilder sb)
      {
         if (modifiers.Count == 0)
            return;
         foreach (var modifier in modifiers) 
            AddModifier(tabs, modifier, ref sb);
      }

      public static string ApplyModPrefix(string str)
      {
         if (!string.IsNullOrEmpty(Globals.Settings.Saving.Formatting.ModPrefix))
            return Globals.Settings.Saving.Formatting.ModPrefix + "_" + str;
         return str;
      }

      public static void OpenBlock(ref int tabs, string blockName, ref StringBuilder sb)
      {
         AddTabs(tabs, ref sb);
         sb.AppendLine($"{blockName} = {{");
         tabs++;
      }

      public static void CloseBlock(ref int tabs, ref StringBuilder sb)
      {
         AddTabs(--tabs, ref sb);
         sb.AppendLine("}");
      }

      public static void AddModifier(int tabs, ISaveModifier mod, ref StringBuilder sb)
      {
         AddTabs(tabs, ref sb);
         sb.AppendLine(mod.GetModifierString());
      }

      public static void AddFormattedProvinceList(int tabCount, ICollection<Province> provinces, string blockName, ref StringBuilder sb)
      {
         var ids = provinces.Select(x => x.Id).ToList();
         AddFormattedIntList(blockName, ids, tabCount, ref sb);
      }

      public static void AddNames(int tabs, ICollection<TriggeredName> Names, ref StringBuilder sb)
      {
         foreach (var name in Names)
         {
            OpenBlock(ref tabs, "names", ref sb);
            if (name.Trigger != null)
               name.Trigger.FormatElement(tabs, ref sb);
            AddString(tabs, name.Name, "name", ref sb);
            CloseBlock(ref tabs, ref sb);
         }
      }

      public static void AddElements(int tabs, ICollection<IElement> elements, ref StringBuilder sb)
      {
         foreach (var element in elements) 
            element.FormatElement(tabs, ref sb);
      }

      /// <summary>
      /// this ignores all dates for the 1.1.1
      /// </summary>
      /// <param name="tabs"></param>
      /// <param name="date"></param>
      /// <param name="dateString"></param>
      /// <param name="sb"></param>
      public static void AddDate(int tabs, Date date, string dateString, ref StringBuilder sb)
      {
         if (date == Date.MinValue)
            return;
         AddTabs(tabs, ref sb);
         sb.AppendLine($"{dateString} = {date:yyyy.M.d}");
      }
   }
}