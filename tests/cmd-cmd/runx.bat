"%~dp0..\..\src\RunX\bin\Debug\net10.0\RunX.exe" "%~dp0test.cmd"
@set ACTUAL=%ERRORLEVEL%
@echo off
if %ACTUAL%==0 (
    echo OK: .cmd script executed via CmdShell
    exit /b 0
)
echo FAIL: Expected exit code 0, got %ACTUAL%
exit /b 1
