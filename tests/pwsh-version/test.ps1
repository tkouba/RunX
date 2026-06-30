$version = $PSVersionTable.PSVersion
$edition = $PSVersionTable.PSEdition

Write-Host "PSVersion : $version"
Write-Host "PSEdition : $edition"

if ($edition -eq 'Core' -and $version.Major -ge 7) {
    Write-Host "OK: Running in PowerShell $($version.Major) (pwsh)"
    exit 0
}

Write-Host "FAIL: Expected PowerShell Core 7+, got edition='$edition' version='$version'"
exit 1
