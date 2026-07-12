using System.Diagnostics;

namespace RunX;

internal sealed class WindowsPowerShellShell : IShell
{
    public string Name => "powershell";

    public bool IsInstalled() => ShellHelper.ExistsOnPath("powershell") || ExistsAtKnownPath();

    public bool CanRun(string scriptPath) =>
        Path.GetExtension(scriptPath).Equals(".ps1", StringComparison.OrdinalIgnoreCase)
        && !String.Equals(ShellHelper.GetRequiredPSEdition(scriptPath), "Core", StringComparison.OrdinalIgnoreCase);

    public ProcessStartInfo CreateProcessStartInfo(string scriptPath, IEnumerable<string> scriptArguments)
    {
        var psi = new ProcessStartInfo("powershell");
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
        File.Exists(@"C:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe") ||
        File.Exists(@"C:\Windows\SysWOW64\WindowsPowerShell\v1.0\powershell.exe");
}
