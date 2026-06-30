"%~dp0..\..\src\RunX\bin\Debug\net10.0\RunX.exe" --shell pwsh --env TEST_VAR=hello-from-runx "%~dp0test.ps1"
@exit /b %ERRORLEVEL%
