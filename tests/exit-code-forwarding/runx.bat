"%~dp0..\..\src\RunX\bin\Debug\net10.0\RunX.exe" --shell pwsh "%~dp0test.ps1"
@set ACTUAL=%ERRORLEVEL%
@echo off
if %ACTUAL%==42 (
    echo OK: Exit code forwarded correctly ^(42^)
    @exit /b 0
)
echo FAIL: Expected exit code 42, got %ACTUAL%
exit /b 1
