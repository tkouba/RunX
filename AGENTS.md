# AGENTS.md – Development Guidelines for run.exe

## Philosophy

The runner must:

- behave as a transparent execution wrapper
- never alter script behavior
- only handle execution and logging

---

## Core Principles

### 1. Transparency

- Output must be identical to native shell execution
- ANSI sequences must pass through untouched

---

### 2. No Business Logic

- Do NOT interpret output
- Do NOT parse logs
- Do NOT add semantic meaning

---

### 3. Deterministic Logging

- Everything written to console must be written to log
- No hidden output
- No formatting differences

---

### 4. Shell Responsibility Separation

Shell handles:
- script execution
- argument parsing

Runner handles:
- process lifecycle
- output streaming
- logging

---

## Implementation Rules

### Process

- Use `ProcessStartInfo`
- Always redirect stdout and stderr
- Use async read to avoid deadlocks

---

### Output

- Merge stdout + stderr
- Write to console immediately
- Write to file immediately

---

### Encoding

- Default to UTF-8
- Do not transform data

---

### Exit Code

- Return child process exit code

---

### Logging

- Create log directory if needed
- Fail silently if logging cannot be initialized

---

## Shell Resolution Rules

### auto

- prefer pwsh
- fallback to powershell.exe
- cmd for .bat/.cmd

---

## Non-Goals

- scheduling
- orchestration
- UI

---

## Style Guidelines

- keep code minimal
- avoid hidden behavior
- no magic

---

## Summary

Runner = transport layer for execution + logging

Not a framework.

