param([string]$LogDir)

$log = Get-ChildItem $LogDir -Filter "ts-test-*.log" | Sort-Object LastWriteTime | Select-Object -Last 1
if (-not $log) {
    Write-Host "FAIL: No log file found in $LogDir"
    exit 1
}

$lines = Get-Content $log.FullName
if ($lines.Count -eq 0) {
    Write-Host "FAIL: Log file is empty"
    exit 1
}

$pattern = '^\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}\.\d{3} '
$bad = $lines | Where-Object { $_ -notmatch $pattern }
if ($bad) {
    Write-Host "FAIL: Line missing timestamp prefix: $($bad[0])"
    exit 1
}

Write-Host "OK: All $($lines.Count) line(s) have timestamp prefix"
Write-Host "    Sample: $($lines[0])"
