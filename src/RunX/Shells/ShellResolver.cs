namespace RunX;

internal static class ShellResolver
{
    internal static IShell? Resolve(string? shellOption, string scriptPath, Action<string> onError)
    {
        if (shellOption is not null and not "auto")
        {
            var shell = CreateExplicit(shellOption);
            if (!shell.IsInstalled())
            {
                onError($"Shell '{shellOption}' is not installed or not found.");
                return null;
            }
            return shell;
        }

        // auto mode
        var pwsh = new PwshShell();
        if (pwsh.CanRun(scriptPath) && pwsh.IsInstalled()) return pwsh;

        var ps = new WindowsPowerShellShell();
        if (ps.CanRun(scriptPath) && ps.IsInstalled()) return ps;

        var cmd = new CmdShell();
        if (cmd.CanRun(scriptPath)) return cmd;

        onError($"No suitable shell found for '{Path.GetExtension(scriptPath)}' scripts.");
        return null;
    }

    private static IShell CreateExplicit(string shellName) => shellName switch
    {
        "pwsh" => new PwshShell(),
        "powershell" => new WindowsPowerShellShell(),
        "cmd" => new CmdShell(),
        _ => throw new InvalidOperationException($"Unknown shell: {shellName}")
    };
}
