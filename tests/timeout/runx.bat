"%~dp0..\..\src\RunX\bin\Debug\net10.0\RunX.exe" --shell pwsh --timeout 1 "%~dp0test.ps1"
@set ACTUAL=%ERRORLEVEL%
@echo off
if %ACTUAL%==1 (
    echo OK: Process was terminated after timeout
    exit /b 0
)
echo FAIL: Expected exit code 1 ^(timeout^), got %ACTUAL%
exit /b 1
