$target = "C:\Tools\runx"

New-Item -ItemType Directory -Force -Path $target | Out-Null

Copy-Item ".\runx.exe" $target -Force

# PATH
$path = [Environment]::GetEnvironmentVariable("PATH", "User")

if ($path -notlike "*$target*") {
    [Environment]::SetEnvironmentVariable(
        "PATH",
        "$path;$target",
        "User"
    )
}

Write-Host "runx installed to $target"
