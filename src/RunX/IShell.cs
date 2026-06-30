using System.Diagnostics;

namespace RunX
{
    interface IShell
    {
        string Name { get; }
        bool IsInstalled();
        bool CanRun(string scriptPath);
        ProcessStartInfo CreateProcessStartInfo(string scriptPath, IEnumerable<string> scriptArguments);
    }
}
