using System.Reflection.Metadata.Ecma335;
using Editor.Analyzers;
using Editor.Controls;

namespace Editor.DataClasses.ConsoleCommands
{
   internal class CsCAnalyzeProvince : ConsoleCommand
   {
      public override string Command => "analyze";

      public override string Description => "Analzyes the given province for the given context";

      public override string Help => "analyze <provinceId> <attribute>";

      public override string[] Aliases => [];

      public override void Execute(string[] args)
      {
         if (args.Length < 3)
         {
            Output.WriteError("Invalid number of arguments; can not analyze!");
            return;
         }

         if (!int.TryParse(args[1], out var id) ||! Globals.Provinces.TryGetValue(id, out var province))
         {
            Output.WriteError($"Invalid province ID [{args[1]}]; can not analyze!");
            return;
         }

         if (!Globals.UniqueAttributeKeys.Contains(args[2]) || !AnalyzerFacotry.HasAnalyzer(args[2]))
         {
            Output.WriteError($"Invalid attribute [{args[2]}]; can not analyze!");
            return;
         }

         AnalyzeProvince(province, args[2], out var feedback);
         Output.WriteLine(feedback.Feedback);
      }

      private static void AnalyzeProvince(Province province, string attribute, out AnalyzerFeeback feedback)
      {
         var analyzer = AnalyzerFacotry.GetAnalyzer(attribute);
         analyzer.AnalyzeProvince(province.Id, out feedback);
      }
   }
}
