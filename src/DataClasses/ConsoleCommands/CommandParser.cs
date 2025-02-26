using System.IO;
using System.Reflection.Metadata;
using Editor.Helper;

namespace Editor.DataClasses.ConsoleCommands;

public enum ClearanceLevel
{
   Debug,
   User,
   Admin // Higher than User but should not be important until project files are added
}

public class Command(string name, string usage, Func<string[], string[]> execute, ClearanceLevel clearance, params string[] aliases)
{
   public string Name { get; } = name;
   public string[] Aliases { get; } = aliases;
   public Func<string[], string[]> Execute { get; } = execute; // simple flags use one '-' while flags with values use two '--'
   public ClearanceLevel Clearance { get; } = clearance;
   public string Usage { get; } = usage;
}

public class CommandHandler
{
   // The string is the console Identifier
   private static readonly Dictionary<string, Dictionary<string, string>> _macros = [];

   public static Dictionary<string, string> GetMacros(string identifier) => _macros[identifier];

   private readonly Dictionary<string, Command> _commands = new();
   private ClearanceLevel _currentClearance = ClearanceLevel.User;

   private readonly List<string> _history = [];
   private int _historyIndex = 0;
   public bool TrimQuotesOnArguments { get; } = true;
   private const int HISTORY_CAPACITY = 100;
   private const string MACRO_FILE = "macros.json";

   public string Identifier { get; }

   internal int HistoryIndex
   {
      get => _historyIndex;
      set => _historyIndex = value;
   }

   internal List<string> History => _history;

   private RichTextBox OutputBox { get; }

   public CommandHandler(RichTextBox output, string identifier)
   {
      OutputBox = output;
      Identifier = identifier;
      CommandHandler.Register(this);
      
      var loader = new CommandLoader(this, output);
      loader.Load();
   }

   internal List<string> GetCommandNames() => _commands.Values.Select(x => x.Name).Distinct().ToList();
   internal List<string> GetCommandAliases()
   {
      HashSet<string> described = [];
      foreach (var command in _commands.Values)
         foreach (var alias in command.Aliases)
            described.Add($"{alias} ({command.Name})");
      return described.ToList();
   }

   internal List<Command> GetCommands() => _commands.Values.Distinct().ToList();

   public static void SaveMacros() => JSONWrapper.SaveToModforgeData(_macros, MACRO_FILE);
   public static void LoadMacros()
   {
      if (JSONWrapper.LoadFromModforgeData<Dictionary<string, Dictionary<string, string>>>(MACRO_FILE, out var macros))
         foreach (var (key, value) in macros)
            _macros.Add(key, value);
   }

   public bool SetAlias(string alias, string command)
   {
      if (_commands.TryGetValue(command, out var cmd))
      {
         _commands[alias] = cmd;
         return true;
      }
      return false;
   }

   public bool RemoveAlias(string alias)
   {
      if (_commands.ContainsKey(alias))
      {
         _commands.Remove(alias);
         return true;
      }
      return false;
   }

   public static void Register(CommandHandler handler)
   {
      if (!_macros.ContainsKey(handler.Identifier))
         _macros[handler.Identifier] = [];
   }

   public bool AddMacro(string key, string value)
   {
      return _macros[Identifier].TryAdd(key, value);
   }

   public bool RemoveMacro(string key)
   {
      if (_macros[Identifier].ContainsKey(key))
      {
         _macros[Identifier].Remove(key);
         return true;
      }
      return false;
   }

   public bool RunMacro(string key, out string[] value)
   {
      if (_macros[Identifier].TryGetValue(key, out var macro))
      {
         value = ExecuteCommandInternal(macro);
         return true;
      }
      value = [];
      return false;
   }

   public void ClearMacros() => _macros[Identifier].Clear();

   public void RegisterCommand(Command command)
   {
      _commands[command.Name] = command;
      foreach (var alias in command.Aliases)
         _commands[alias] = command;
   }

   private string[] ExecuteCommandInternal(string input)
   {
      List<string> output = [];
      var parts = SplitStringQuotes(input, trimQutoes:false);
      if (parts.Length == 0)
         return [.. output];

      var commandName = parts[0].ToLower();
      var args = parts.Skip(1).ToArray();

      if (_commands.TryGetValue(commandName, out var command))
         if (_currentClearance >= command.Clearance)
            output.AddRange(command.Execute(args));
         else
            Console.WriteLine($"❌ Permission denied. Requires {command.Clearance} clearance.");
      else
      {
         output.Add($"❌ Unknown command: {commandName}");
         var suggestion = FindClosestCommand(commandName);
         if (!string.IsNullOrEmpty(suggestion))
            output.Add($"💡 Did you mean: {suggestion}?");
      }
      return [.. output];
   }

   public static string[] SplitStringQuotes(string cmd, char splitChar = ' ', char quoteChar = '"', bool trimQutoes = true)
   {
      List<string> parts = [];
      var inQuotes = false;
      var current = string.Empty;
      foreach (var c in cmd)
      {
         if (c == quoteChar)
         {
            inQuotes = !inQuotes;
            if (trimQutoes)
               continue;
         }
         if (c == splitChar && !inQuotes)
         {
            if (!string.IsNullOrEmpty(current))
               parts.Add(current);
            current = string.Empty;
            continue;
         }
         current += c;
      }
      if (!string.IsNullOrEmpty(current))
         parts.Add(current);
      return parts.ToArray();
   }

   private string? FindClosestCommand(string input)
   {
      return _commands.Keys.OrderBy(cmd => LevenshteinDistance(input, cmd)).FirstOrDefault();
   }

   private static int LevenshteinDistance(string s1, string s2)
   {
      var dp = new int[s1.Length + 1, s2.Length + 1];

      for (var i = 0; i <= s1.Length; i++)
         for (var j = 0; j <= s2.Length; j++)
            if (i == 0) dp[i, j] = j;
            else if (j == 0) dp[i, j] = i;
            else
               dp[i, j] = Math.Min(
                   Math.Min(dp[i - 1, j] + 1, dp[i, j - 1] + 1),
                   dp[i - 1, j - 1] + (s1[i - 1] == s2[j - 1] ? 0 : 1));

      return dp[s1.Length, s2.Length];
   }
   
   internal void ExecuteCommand(string cmd)
   {
      var usedCmdStr = cmd[2..];
      var result = ExecuteCommandInternal(usedCmdStr);
      AddToHistory(usedCmdStr);
      for (var i = 0; i < result.Length; i++)
      {
         if (i > 0)
            result[i] = "  " + result[i];
         OutputBox.AppendText(result[i] + Environment.NewLine);
      }

      OutputBox.AppendText("> ");
      OutputBox.ScrollToCaret();
      OutputBox.SelectionStart = OutputBox.Text.Length;
   }


   internal void AddToHistory(string cmd)
   {
      _history.Add(cmd);
      if (_history.Count > HISTORY_CAPACITY)
         _history.RemoveAt(0);
      _historyIndex = _history.Count;
   }
}
