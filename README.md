# runx.exe – Universal Script Runner

## Overview

`runx.exe` is a lightweight tool for executing scripts with reliable logging and transparent output handling.

It improves usability of Windows scripting by providing:

- automatic logging
- consistent execution behavior
- minimal configuration

---

## Quick Start

```text
runx script.ps1
```

No configuration required.

---

## Features

- automatic shell detection
- fallback: `pwsh` → `powershell.exe`
- merged stdout + stderr logging
- ANSI color passthrough
- per-run log files
- simple CLI

---

## Examples

### Basic

```text
runx Deploy.ps1
```

---

### With script arguments

```text
runx Deploy.ps1 -env prod -force
```

---

### Explicit shell selection

```text
runx --shell pwsh Deploy.ps1
runx --shell powershell Deploy.ps1
runx --shell cmd Build.bat
```

---

### Custom log directory and name

```text
runx --log D:\LOG --name Deploy Deploy.ps1
```

---

### Timestamp each log line

```text
runx --timestamp-lines --log D:\LOG Deploy.ps1
```

---

### Set working directory

```text
runx --cwd D:\Projects\MyApp Deploy.ps1
```

---

### Set environment variables

```text
runx --env ENV=prod Deploy.ps1
runx --env ENV=prod --env VERSION=1.2.3 Deploy.ps1
```

---

### Execution timeout

```text
runx --timeout 30 Deploy.ps1
```

---

### Write runx errors to Windows Event Log

```text
runx --eventlog Deploy.ps1
```

Enabled automatically when running non-interactively (scheduled tasks, Windows services, CI).
Writes to the `Application` event log. Requires the `Application` source to exist — no auto-registration.

---

### Print help

```text
runx --help
```

---

### Print version

```text
runx --version
```

---

### Verbose runx meta-information on console

```text
runx --verbose Deploy.ps1
```

Prints runx header (version, shell, cwd, script, log directory) and footer (exit code, duration) to stderr with `[runx]` prefix. Combined with `--no-runx-log`, runx meta-information goes only to console.

---

### Disable runx internal log

```text
runx --no-runx-log Deploy.ps1
```

Suppresses the runx internal log file. Script output log is not affected.

---

### Run a cmd script (auto-detected)

```text
runx Build.bat
runx Build.cmd
```

---

### Combined options

```text
runx --shell pwsh --log D:\LOG --name Deploy --timestamp-lines --timeout 120 --env ENV=prod Deploy.ps1
```

---

## Log Files

Default format (new file per run):

```text
<name>-YYYYMMdd-HHmmss.log
```

Example:

```text
Deploy-20260624-130501.log
```

With `--append` (fixed filename, runs accumulate):

```text
<name>.log
```

Example:

```text
Deploy.log
```

---

## Environment Variables

Used for log directory:

- RUNNER_LOG_DIR
- LOG_DIR
- LOGDIR
- LOG

Fallback:

- %TEMP%\run-logs
- %TMP%\run-logs

---

## Shell Detection (auto)

| Extension | Shell |
|----------|------|
| .ps1 | pwsh → fallback powershell.exe |
| .bat/.cmd | cmd |

---

## Design Principles

- do not modify script behavior
- do not interpret output
- act as execution + logging layer only

---

## Exit Code

Returns the same exit code as the executed script.

---

## Releasing

Releases are published automatically via GitHub Actions when a version tag is pushed.

```text
git tag v1.2.3
git push origin v1.2.3
```

The pipeline compiles the version number into the executable and publishes `runx.exe` as a GitHub Release with auto-generated release notes.

---

## Status

MVP design – simple, practical, extensible.
