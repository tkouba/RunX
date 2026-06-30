namespace RunX;

internal sealed class Logger : IDisposable
{
    private StreamWriter? _fileWriter;
    private readonly object _lock = new();
    private readonly bool _timestampLines;

    private Logger(StreamWriter? fileWriter, bool timestampLines)
    {
        _fileWriter = fileWriter;
        _timestampLines = timestampLines;
    }

    internal static Logger CreateDisabled() => new Logger(null, false);

    internal static Logger Create(
        string? logDirOverride,
        string scriptPath,
        string? nameOverride,
        bool append,
        bool timestampLines)
    {
        var logDir = ResolveLogDir(logDirOverride);
        if (logDir == null)
            return new Logger(null, timestampLines);

        try
        {
            Directory.CreateDirectory(logDir);
            var name = nameOverride ?? Path.GetFileNameWithoutExtension(scriptPath);
            var fileName = append ? $"{name}.log" : $"{name}-{DateTime.Now:yyyyMMdd-HHmmss}.log";
            var filePath = Path.Combine(logDir, fileName);
            var fileMode = append ? FileMode.Append : FileMode.Create;
            var fs = new FileStream(filePath, fileMode, FileAccess.Write, FileShare.Read);
            var writer = new StreamWriter(fs, System.Text.Encoding.UTF8) { AutoFlush = true };
            return new Logger(writer, timestampLines);
        }
        catch
        {
            return new Logger(null, timestampLines);
        }
    }

    internal void WriteLineToFile(string line)
    {
        if (_fileWriter == null) return;
        var output = _timestampLines
            ? $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} {line}"
            : line;
        lock (_lock)
        {
            _fileWriter.WriteLine(output);
        }
    }

    public void Dispose() => _fileWriter?.Dispose();

    public static string? ResolveLogDir(string? logDirOverride)
    {
        if (logDirOverride != null) return logDirOverride;

        foreach (var varName in new[] { "RUNNER_LOG_DIR", "LOG_DIR", "LOGDIR", "LOG" })
        {
            var val = Environment.GetEnvironmentVariable(varName);
            if (!string.IsNullOrWhiteSpace(val)) return val;
        }

        var temp = Environment.GetEnvironmentVariable("TEMP");
        if (!string.IsNullOrWhiteSpace(temp)) return Path.Combine(temp, "run-logs");

        var tmp = Environment.GetEnvironmentVariable("TMP");
        if (!string.IsNullOrWhiteSpace(tmp)) return Path.Combine(tmp, "run-logs");

        return null;
    }
}
