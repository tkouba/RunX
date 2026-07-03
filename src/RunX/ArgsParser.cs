namespace RunX;

internal static class ArgsParser
{
    internal static RunnerArgs? Parse(string[] args)
    {
        if (args.Length == 0)
        {
            PrintUsage();
            return null;
        }

        string? shell = null;
        string? logDir = null;
        string? logName = null;
        string? cwd = null;
        bool timestampLines = false;
        bool append = false;
        int? timeoutSeconds = null;
        bool eventLog = !Environment.UserInteractive;
        bool noRunxLog = false;
        bool verbose = false;
        var envOverrides = new List<(string Key, string Value)>();

        int i = 0;
        bool scanningOptions = true;

        while (i < args.Length && scanningOptions)
        {
            var arg = args[i];

            if (arg == "--")
            {
                i++;
                scanningOptions = false;
                break;
            }

            if (!arg.StartsWith("--"))
            {
                scanningOptions = false;
                break;
            }

            switch (arg)
            {
                case "--shell":
                    if (i + 1 >= args.Length) { Error("--shell requires a value"); return null; }
                    shell = args[++i];
                    if (!ShellResolver.IsKnownShell(shell) && !String.Equals(shell, "auto", StringComparison.OrdinalIgnoreCase))
                    {
                        Error($"--shell value '{shell}' is invalid; expected one of: {string.Join(", ", ShellResolver.KnownShellNames())} or auto");
                        return null;
                    }
                    break;

                case "--log":
                    if (i + 1 >= args.Length) { Error("--log requires a value"); return null; }
                    logDir = args[++i];
                    break;

                case "--name":
                    if (i + 1 >= args.Length) { Error("--name requires a value"); return null; }
                    logName = args[++i];
                    break;

                case "--cwd":
                    if (i + 1 >= args.Length) { Error("--cwd requires a value"); return null; }
                    cwd = args[++i];
                    break;

                case "--timestamp-lines":
                    timestampLines = true;
                    break;

                case "--append":
                    append = true;
                    break;

                case "--timeout":
                    if (i + 1 >= args.Length) { Error("--timeout requires a value"); return null; }
                    if (!int.TryParse(args[++i], out var timeout) || timeout <= 0)
                    {
                        Error($"--timeout value '{args[i]}' is invalid; expected a positive integer (seconds)");
                        return null;
                    }
                    timeoutSeconds = timeout;
                    break;

                case "--eventlog":
                    eventLog = true;
                    break;

                case "--no-runx-log":
                    noRunxLog = true;
                    break;

                case "--verbose":
                    verbose = true;
                    break;

                case "--env":
                    if (i + 1 >= args.Length) { Error("--env requires a KEY=VALUE argument"); return null; }
                    var envArg = args[++i];
                    var eqIndex = envArg.IndexOf('=');
                    if (eqIndex <= 0)
                    {
                        Error($"--env argument '{envArg}' must be in KEY=VALUE format");
                        return null;
                    }
                    envOverrides.Add((envArg[..eqIndex], envArg[(eqIndex + 1)..]));
                    break;

                default:
                    Error($"Unknown option '{arg}'");
                    return null;
            }

            i++;
        }

        if (i >= args.Length)
        {
            Error("No script path specified");
            return null;
        }

        var scriptPath = args[i++];
        var scriptArgs = args[i..];

        return new RunnerArgs(
            Shell: shell,
            LogDir: logDir,
            LogName: logName,
            Cwd: cwd,
            TimestampLines: timestampLines,
            Append: append,
            TimeoutSeconds: timeoutSeconds,
            EventLog: eventLog,
            NoRunxLog: noRunxLog,
            Verbose: verbose,
            EnvOverrides: envOverrides,
            ScriptPath: scriptPath,
            ScriptArgs: scriptArgs
        );
    }

    private static void Error(string message)
    {
        Console.Error.WriteLine($"[runx] Error: {message}");
        Console.Error.WriteLine("Run 'runx' with no arguments for usage.");
    }

    internal static void PrintUsage()
    {
        Console.Error.WriteLine("""
            runx - Universal Script Runner

            Usage:
              runx [options] <script> [script-args...]

            Options:
              --version                             Print version and exit
              --help                                Print this help and exit
              --shell <pwsh|powershell|cmd|auto>    Shell selection (default: auto)
              --log <dir>                           Log directory
              --name <name>                         Log file name prefix
              --cwd <dir>                           Working directory
              --timestamp-lines                     Prefix log lines with timestamp
              --append                              Append to log; file named <name>.log (no timestamp)
              --timeout <sec>                       Execution timeout in seconds
              --eventlog                            Write runx errors to Windows Event Log (auto-enabled when non-interactive)
              --no-runx-log                         Disable runx internal log (script output log is not affected)
              --verbose                             Write runx meta-information to console with [runx] prefix
              --env KEY=VALUE                       Environment variable (repeatable)
              --                                    End of runx options

            Examples:
              runx Deploy.ps1
              runx Deploy.ps1 -env prod -force
              runx --shell pwsh --log D:\LOG --name Deploy Deploy.ps1
            """);
    }
}
