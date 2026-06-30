$value = $env:TEST_VAR

Write-Host "TEST_VAR=$value"

if ($value -eq 'hello-from-runx') {
    Write-Host "OK: Environment variable set correctly"
    exit 0
}

Write-Host "FAIL: Expected 'hello-from-runx', got '$value'"
exit 1
