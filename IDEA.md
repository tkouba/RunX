# runx.exe – Universal Script Runner (IDEA)

## Goal

Create a lightweight command-line tool (`runx.exe`) for executing scripts (primarily PowerShell) with:

- consistent execution behavior
- transparent output forwarding
- structured logging

Core idea:

> Execute scripts exactly as native shell would, while capturing and logging their output.

---

## CLI Syntax

```text
runx [runner-options] <script> [script-args...]
```

---

## Runner Options

| Option | Description |
|--------|------------|
| `--shell <pwsh|powershell|cmd|auto>` | Shell selection (default: auto) |
| `--log <dir>` | Log directory |
| `--name <name>` | Override log file prefix |
| `--cwd <dir>` | Working directory |
| `--timestamp-lines` | Prefix log lines with timestamp local timestamp (YYYY-MM-dd HH:mm:ss.fff) |
| `--append` | Append to log instead of new file |
| `--timeout <sec>` | Execution timeout |
| `--env KEY=VALUE` | Environment variable override |

---

## Defaults

### Shell resolution

- `auto`:
  - `.ps1` → pwsh if available, fallback to powershell.exe
  - `.bat/.cmd` → cmd

Explicit shell must exist, otherwise execution fails.

---

### Log directory resolution

Priority order:

1. `--log`
2. `RUNNER_LOG_DIR`
3. `LOG_DIR`
4. `LOGDIR`
5. `LOG`
6. `%TEMP%\run-logs`
7. `%TMP%\run-logs`
8. no logging if none available

Directory is created if missing.

---

## Logging

### File naming

```text
<name>-YYYYMMdd-HHmmss.log
```

Where:

- `name` = `--name` or script filename (no extension)

---

### Behavior

- stdout + stderr merged
- identical to console output
- ANSI sequences preserved

---

## Execution Model

1. Resolve shell
2. Build command
3. Start process
4. Stream output
5. Write output to console and log
6. Return exit code

---

## Key Constraints

- DO NOT modify script behavior
- DO NOT interpret output
- DO NOT inject code into scripts

---

## Complexity

Low–Medium:

- Process execution
- stream forwarding
- argument parsing

Main challenge:

- ANSI preservation

---

## Summary

- fully feasible
- minimal scope
- high practical value

