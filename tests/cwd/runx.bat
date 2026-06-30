"%~dp0..\..\src\RunX\bin\Debug\net10.0\RunX.exe" --shell pwsh --cwd "%~dp0workdir" "%~dp0test.ps1" "%~dp0workdir"
@exit /b %ERRORLEVEL%
