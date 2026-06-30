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
}
