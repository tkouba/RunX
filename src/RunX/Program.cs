using System.Diagnostics;
using System.Runtime.Versioning;

namespace RunX;

internal class Program
{
    static async Task<int> Main(string[] args)
    {
        try
        {
            ConsoleSetup.Initialize();

            if (args.Length == 1 && args[0] == "--help")
            {
                ArgsParser.PrintUsage();
                return 0;
            }

            if (args.Length == 1 && args[0] == "--version")
            {
                var ver = typeof(Program).Assembly.GetName().Version;
                Console.WriteLine($"runx {(ver is not null ? $"{ver.Major}.{ver.Minor}.{ver.Build}" : "unknown")}");
                return 0;
            }

            var parsed = ArgsParser.Parse(args);
            if (parsed == null) return 1;

            using var runxLogger = parsed.NoRunxLog
                ? Logger.CreateDisabled()
                : Logger.Create(parsed.LogDir, string.Empty, "runx", parsed.Append, timestampLines: true);

            void WriteRunxLog(string line)
            {
                runxLogger.WriteLineToFile(line);
                if (parsed.Verbose)
                    Console.Error.WriteLine(line.Length == 0 ? string.Empty : $"[runx] {line}");
            }

            void RunxError(string message)
            {
                string logMessage = $"[runx] Error: {message}";
                Console.Error.WriteLine(logMessage);
                runxLogger.WriteLineToFile(logMessage);
                if (parsed.EventLog && OperatingSystem.IsWindows())
                    TryWriteEventLog(logMessage);
            }

            if (parsed.Cwd != null && !Directory.Exists(parsed.Cwd))
            {
                RunxError($"Working directory '{parsed.Cwd}' does not exist.");
                return 1;
            }

            IShell? shell;
            try
            {
                shell = ShellResolver.Resolve(parsed.Shell, parsed.ScriptPath, RunxError);
                if (shell == null)
                {
                    RunxError($"Failed to resolve shell '{parsed.Shell}' for script '{parsed.ScriptPath}'");
                    return 1;
                }
            }
            catch (Exception ex)
            {
                RunxError($"Failed to resolve shell: {ex.Message}");
                return 1;
            }

            try
            {
                if (!shell.IsInstalled())
                {
                    RunxError($"Shell '{parsed.Shell ?? shell.Name}' is not installed or not found.");
                    return 1;
                }
            }
            catch (Exception ex)
            {
                RunxError($"Failed to check if shell is installed: {ex.Message}");
                return 1;
            }

            var version = typeof(Program).Assembly.GetName().Version;
            var versionStr = version is not null ? $"{version.Major}.{version.Minor}.{version.Build}" : "unknown";
            WriteRunxLog($"runx version: {versionStr}");
            WriteRunxLog($"shell: {shell.Name}");
            WriteRunxLog($"cwd: {parsed.Cwd ?? Directory.GetCurrentDirectory()}");
            WriteRunxLog($"timestamp lines: {parsed.TimestampLines}");
            WriteRunxLog($"append: {parsed.Append}");
            WriteRunxLog($"script: {parsed.ScriptPath}");
            WriteRunxLog($"logDir: {Logger.ResolveLogDir(parsed.LogDir)}");

            using var scriptLogger = Logger.Create(
                    parsed.LogDir,
                    parsed.ScriptPath,
                    parsed.LogName,
                    parsed.Append,
                    parsed.TimestampLines);

            var psi = shell.CreateProcessStartInfo(parsed.ScriptPath, parsed.ScriptArgs);

            if (parsed.Cwd != null)
                psi.WorkingDirectory = parsed.Cwd;

            foreach (var (key, value) in parsed.EnvOverrides)
                psi.Environment[key] = value;

            Process process;
            var stopwatch = Stopwatch.StartNew();
            try
            {
                process = Process.Start(psi)!;
            }
            catch (Exception ex)
            {
                RunxError($"Failed to start process: {ex.Message}");
                return 1;
            }

            using (process)
            {
                var stdoutTask = DrainStreamAsync(process.StandardOutput, Console.Out, scriptLogger);
                var stderrTask = DrainStreamAsync(process.StandardError, Console.Error, scriptLogger);

                if (parsed.TimeoutSeconds.HasValue)
                {
                    using var cts = new CancellationTokenSource(parsed.TimeoutSeconds.Value * 1000);
                    try
                    {
                        await process.WaitForExitAsync(cts.Token);
                    }
                    catch (OperationCanceledException)
                    {
                        process.Kill(entireProcessTree: true);
                        RunxError($"Timeout after {parsed.TimeoutSeconds}s");
                        await Task.WhenAll(stdoutTask, stderrTask);
                        var timeoutDuration = stopwatch.Elapsed.TotalSeconds;
                        WriteRunxLog($"END ERROR (exit code 1, duration {timeoutDuration:F1}s)");
                        return 1;
                    }
                }
                else
                {
                    await process.WaitForExitAsync();
                }

                await Task.WhenAll(stdoutTask, stderrTask);
                var exitCode = process.ExitCode;
                stopwatch.Stop();
                var duration = stopwatch.Elapsed.TotalSeconds;
                var status = exitCode == 0 ? "OK" : "ERROR";
                WriteRunxLog($"END {status} (exit code {exitCode}, duration {duration:F1}s)");
                return exitCode;
            }
        }
        catch (Exception ex)
        {
            string logMessage = $"[runx] Error: {ex.Message}";
            Console.Error.WriteLine(logMessage);
            if (!Environment.UserInteractive && OperatingSystem.IsWindows())
                TryWriteEventLog(logMessage);
            return 1;
        }
    }

    private static async Task DrainStreamAsync(StreamReader reader, TextWriter console, Logger logger)
    {
        string? line;
        while ((line = await reader.ReadLineAsync()) != null)
        {
            console.WriteLine(line);
            logger.WriteLineToFile(line);
        }
    }

    [SupportedOSPlatform("windows")]
    private static void TryWriteEventLog(string message)
    {
        try
        {
            const string source = "Application";
            if (EventLog.SourceExists(source))
                EventLog.WriteEntry(source, message, EventLogEntryType.Error);
        }
        catch { }
    }
}
