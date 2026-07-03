namespace RunX;

internal static class ShellResolver
{
    static readonly IShell[] Shells =
    [
        new PwshShell(),
        new WindowsPowerShellShell(),
        new CmdShell()
    ];

    internal static bool IsKnownShell(string shellName)
    {
        foreach (var shell in Shells)
        {
            if (String.Equals(shell.Name, shellName, StringComparison.OrdinalIgnoreCase))
                return true;
        }
        return false;
    }

    internal static IEnumerable<string> KnownShellNames()
    {
        foreach (var shell in Shells)
            yield return shell.Name;
    }

    internal static IShell? Resolve(string? shellOption, string scriptPath, Action<string> onError)
    {
        if (!String.IsNullOrWhiteSpace(shellOption) && !String.Equals(shellOption, "auto", StringComparison.OrdinalIgnoreCase))
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
        foreach (var shell in Shells)
        {
            if (shell.CanRun(scriptPath) && shell.IsInstalled())
                return shell;
        }

        onError($"No suitable shell found for '{Path.GetExtension(scriptPath)}' scripts.");
        return null;
    }

    private static IShell CreateExplicit(string shellName)
    {
        foreach (var shell in Shells)
        {
            if (String.Equals(shell.Name, shellName, StringComparison.OrdinalIgnoreCase))
                return shell;
        }
        throw new InvalidOperationException($"Unknown shell: {shellName}");
    }
}
