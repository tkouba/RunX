$version = $PSVersionTable.PSVersion
$edition = $PSVersionTable.PSEdition

Write-Host "PSVersion : $version"
Write-Host "PSEdition : $edition"

if ($edition -eq 'Desktop') {
    Write-Host "OK: Running in Windows PowerShell (legacy)"
    exit 0
}

Write-Host "FAIL: Expected Windows PowerShell (Desktop edition), got '$edition'"
exit 1
