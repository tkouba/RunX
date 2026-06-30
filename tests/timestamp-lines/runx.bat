"%~dp0..\..\src\RunX\bin\Debug\net10.0\RunX.exe" --shell pwsh --timestamp-lines --log "%~dp0logs" --name ts-test "%~dp0test.ps1"
@set RUNX_EXIT=%ERRORLEVEL%
@echo off
if not %RUNX_EXIT%==0 (
    echo FAIL: runx exited with code %RUNX_EXIT%
    exit /b 1
)
pwsh -NoProfile -File "%~dp0verify.ps1" "%~dp0logs"
exit /b %ERRORLEVEL%
