# #Requires appears mid-file — PowerShell applies it before execution regardless of position
# (see: https://learn.microsoft.com/powershell/module/microsoft.powershell.core/about/about_requires)
Write-Host "Line before #Requires statement"

#Requires -PSEdition Desktop

$edition = $PSVersionTable.PSEdition

Write-Host "PSEdition : $edition"

if ($edition -eq 'Desktop') {
    Write-Host "OK: Auto mode detected mid-file #Requires -PSEdition Desktop and selected Windows PowerShell"
    exit 0
}

Write-Host "FAIL: Expected Desktop edition, got '$edition'"
exit 1
