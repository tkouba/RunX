$isAdmin = ([Security.Principal.WindowsPrincipal]::new(
    [Security.Principal.WindowsIdentity]::GetCurrent())).IsInRole(
        [Security.Principal.WindowsBuiltInRole]::Administrator)
		
$target = if ($isAdmin) { "C:\Tools" } else { Join-Path $env:LOCALAPPDATA "runx" }

New-Item -ItemType Directory -Force -Path $target | Out-Null

$release = Invoke-RestMethod -Uri "https://api.github.com/repos/tkouba/RunX/releases/latest"
$asset = $release.assets | Where-Object { $_.name -eq "runx.exe" } | Select-Object -First 1
if (-not $asset) {
    throw "Asset runx.exe not found in the last release."
}
$outFile = Join-Path $target $asset.name
Invoke-WebRequest -Uri $asset.browser_download_url -OutFile $outFile

# PATH
$scope = if ($isAdmin) { "Machine" } else { "User" }
$path = [Environment]::GetEnvironmentVariable("PATH", $scope)

if (($path -split ';') -notcontains $target) {
        [Environment]::SetEnvironmentVariable(
            "PATH",
            "$path;$target",
            $scope
        )
    }

if ($isAdmin) {    
    if (-not [System.Diagnostics.EventLog]::SourceExists("RunX")) {
        New-EventLog -LogName Application -Source "RunX"
    }
	Write-Host "runx installed to $target for all users"
}
else {
    Write-Host "runx installed to $target for current user only"
}



