using System.Reflection.Metadata;

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
   private readonly Dictionary<string, Command> _commands = new();
   private ClearanceLevel _currentClearance = ClearanceLevel.User;
   private readonly Dictionary<string, string> _macros = [];

   private readonly List<string> _history = [];
   private int _historyIndex = 0;
   private const int HistoryCapacity = 100;

   internal int HistoryIndex
   {
      get => _historyIndex;
      set => _historyIndex = value;
   }

   internal List<string> History => _history;

   private RichTextBox OutputBox { get; }

   public CommandHandler(RichTextBox output)
   {
      OutputBox = output;

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

   public void SetClearance(ClearanceLevel level) => _currentClearance = level;

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

   public bool AddMacro(string key, string value)
   {
      if (!_macros.TryAdd(key, value))
         return false;
      return true;
   }

   public bool RemoveMacro(string key)
   {
      if (_macros.ContainsKey(key))
      {
         _macros.Remove(key);
         return true;
      }
      return false;
   }

   public bool RunMacro(string key, out string[] value)
   {
      if (_macros.TryGetValue(key, out var macro))
      {
         value = ExecuteCommandInternal(macro);
         return true;
      }
      value = [];
      return false;
   }

   public void RegisterCommand(Command command)
   {
      _commands[command.Name] = command;
      foreach (var alias in command.Aliases)
         _commands[alias] = command;
   }

   private string[] ExecuteCommandInternal(string input)
   {
      List<string> output = [];
      var parts = SplitCmd(input);
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

   private string[] SplitCmd(string cmd)
   {
      List<string> parts = [];
      var inQuotes = false;
      var current = "";
      foreach (var c in cmd)
      {
         if (c == '"')
         {
            inQuotes = !inQuotes;
            continue;
         }
         if (c == ' ' && !inQuotes)
         {
            if (!string.IsNullOrEmpty(current))
               parts.Add(current);
            current = "";
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
      if (_history.Count > HistoryCapacity)
         _history.RemoveAt(0);
      _historyIndex = _history.Count;
   }
}
