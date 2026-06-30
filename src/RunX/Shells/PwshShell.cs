using System.Diagnostics;

namespace RunX;

internal sealed class PwshShell : IShell
{
    public string Name => "pwsh";

    public bool IsInstalled() => ShellHelper.ExistsOnPath("pwsh") || ExistsAtKnownPath();

    public bool CanRun(string scriptPath) =>
        Path.GetExtension(scriptPath).Equals(".ps1", StringComparison.OrdinalIgnoreCase);

    public ProcessStartInfo CreateProcessStartInfo(string scriptPath, IEnumerable<string> scriptArguments)
    {
        var psi = new ProcessStartInfo("pwsh");
        psi.ArgumentList.Add("-NonInteractive");
        psi.ArgumentList.Add("-File");
        psi.ArgumentList.Add(scriptPath);
        foreach (var arg in scriptArguments)
            psi.ArgumentList.Add(arg);
        psi.RedirectStandardOutput = true;
        psi.RedirectStandardError = true;
        psi.UseShellExecute = false;
        psi.StandardOutputEncoding = System.Text.Encoding.UTF8;
        psi.StandardErrorEncoding = System.Text.Encoding.UTF8;
        return psi;
    }

    private static bool ExistsAtKnownPath() =>
        File.Exists(@"C:\Program Files\PowerShell\7\pwsh.exe") ||
        File.Exists(@"C:\Program Files\PowerShell\pwsh.exe");
}
