param([string]$Expected)

$actual = (Get-Location).Path

Write-Host "CWD=$actual"

if ($actual -ieq $Expected.TrimEnd('\')) {
    Write-Host "OK: Working directory set correctly"
    exit 0
}

Write-Host "FAIL: Expected '$($Expected.TrimEnd('\'))', got '$actual'"
exit 1
