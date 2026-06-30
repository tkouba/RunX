using System.Diagnostics;

namespace RunX;

internal sealed class CmdShell : IShell
{
    public string Name => "cmd";

    public bool IsInstalled() =>
        File.Exists(Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.System),
            "cmd.exe"));

    public bool CanRun(string scriptPath)
    {
        var ext = Path.GetExtension(scriptPath);
        return ext.Equals(".bat", StringComparison.OrdinalIgnoreCase)
            || ext.Equals(".cmd", StringComparison.OrdinalIgnoreCase);
    }

    public ProcessStartInfo CreateProcessStartInfo(string scriptPath, IEnumerable<string> scriptArguments)
    {
        var psi = new ProcessStartInfo("cmd.exe");
        psi.ArgumentList.Add("/C");
        psi.ArgumentList.Add(scriptPath);
        foreach (var arg in scriptArguments)
            psi.ArgumentList.Add(arg);
        psi.RedirectStandardOutput = true;
        psi.RedirectStandardError = true;
        psi.UseShellExecute = false;
        psi.StandardOutputEncoding = Console.OutputEncoding;
        psi.StandardErrorEncoding = Console.OutputEncoding;
        return psi;
    }
}
