using System.Text.RegularExpressions;

namespace RunX;

internal static class ShellHelper
{
    internal static bool ExistsOnPath(string exeName)
    {
        var pathVar = Environment.GetEnvironmentVariable("PATH") ?? string.Empty;
        foreach (var dir in pathVar.Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries))
        {
            if (File.Exists(Path.Combine(dir.Trim(), exeName + ".exe")))
                return true;
        }
        return false;
    }

    // #Requires can appear on any line in a script (per PowerShell spec), so we scan the entire file.
    internal static string? GetRequiredPSEdition(string scriptPath)
    {
        if (!File.Exists(scriptPath))
            return null;
        foreach (var line in File.ReadLines(scriptPath))
        {
            var match = Regex.Match(line.TrimStart(), @"^#Requires\s+-PSEdition\s+(\w+)", RegexOptions.IgnoreCase);
            if (match.Success)
                return match.Groups[1].Value;
        }
        return null;
    }
}
