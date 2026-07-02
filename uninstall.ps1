function Remove-RunXInstallation {
    param(
        [string]$Target,
        [ValidateSet("User", "Machine")]
        [string]$Scope
    )

    # Remove EXE
    Remove-Item (Join-Path $Target "runx.exe") -Force -ErrorAction SilentlyContinue
	$isEmpty = (Test-Path $Target) -and ((Get-ChildItem $Target -Force | Measure-Object).Count -eq 0)

	if ($isEmpty) {
		# Remove folder
		Remove-Item $Target -Force -ErrorAction SilentlyContinue
		# Remove from PATH
		$path = [Environment]::GetEnvironmentVariable("PATH", $Scope)
		if ($path) {
			$newPath = ($path -split ";" | Where-Object { $_ -and $_ -ne $Target }) -join ";"
			[Environment]::SetEnvironmentVariable("PATH", $newPath, $Scope)
		}
    }
}

# User installation
Remove-RunXInstallation -Target (Join-Path $env:LOCALAPPDATA "runx") -Scope "User"

# Machine installation
Remove-RunXInstallation -Target "C:\Tools" -Scope "Machine"

Write-Host "runx uninstalled from all targets"