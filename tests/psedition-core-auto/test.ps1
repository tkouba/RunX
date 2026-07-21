#Requires -PSEdition Core

$edition = $PSVersionTable.PSEdition

Write-Host "PSEdition : $edition"

if ($edition -eq 'Core') {
    Write-Host "OK: Auto mode correctly selected PowerShell Core (pwsh)"
    exit 0
}

Write-Host "FAIL: Expected Core edition, got '$edition'"
exit 1
