#Requires -PSEdition Desktop

$edition = $PSVersionTable.PSEdition

Write-Host "PSEdition : $edition"

if ($edition -eq 'Desktop') {
    Write-Host "OK: Auto mode correctly selected Windows PowerShell (Desktop edition)"
    exit 0
}

Write-Host "FAIL: Expected Desktop edition, got '$edition'"
exit 1
