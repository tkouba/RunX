namespace RunX;

internal sealed record RunnerArgs(
    string? Shell,
    string? LogDir,
    string? LogName,
    string? Cwd,
    bool TimestampLines,
    bool Append,
    int? TimeoutSeconds,
    bool EventLog,
    bool NoRunxLog,
    bool Verbose,
    IReadOnlyList<(string Key, string Value)> EnvOverrides,
    string ScriptPath,
    IReadOnlyList<string> ScriptArgs
);
