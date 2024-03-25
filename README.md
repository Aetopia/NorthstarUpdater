> [!IMPORTANT]
> This repository is not affiliated with [Northstar](https://github.com/R2Northstar)!

# Northstar Updater
A dedicated minimal standalone updater & installer for [Northstar](https://github.com/R2Northstar), a Titanfall 2 modding and custom server framework.

> [!NOTE]
> If you need more robust functionality please consider looking into:<br>
> - [FlightCore](https://github.com/R2NorthstarTools/FlightCore)
> - [VTOL](https://github.com/BigSpice/VTOL)
> - [Viper](https://github.com/0neGal/viper)

## Features
The Northstar Updater provides the following features:
- Updating/Installing Northstar.
- Launching Northstar.
- Automatic Updates for Northstar.

> [!NOTE]
> The updater only manages the [Northstar Launcher](https://github.com/R2Northstar/NorthstarLauncher).

## Installation
1. Download the latest version of Northstar Updater from [GitHub Releases](https://github.com/Aetopia/Northstar-Updater/releases).

2. Place `NorthstarUpdater.exe` in the root of Titanfall 2's installation directory.
    |Store|Default Location|
    |-|-|
    |Steam|`C:\Program Files (x86)\Steam\steamapps\common\Titanfall2`|
    |Origin|`C:\Program Files (x86)\Origin Games\Titanfall2`|
    |EA App|`C:\Program Files\EA Games\Titanfall2`|

3. Run `NorthstarUpdater.exe` to install and run Northstar.
    > Any arguments passed to `NorthstarUpdater.exe` will be forwarded to Northstar.

## Uninstallation
### Steam
Run the following script in PowerShell.

```powershell
Get-Content  "$(Split-Path $(([string]((Get-ItemPropertyValue `
-Path "Registry::HKEY_CLASSES_ROOT\steam\Shell\Open\Command" -Name "(Default)") `
-Split "-", 2, "SimpleMatch")[0]).Trim().Trim('"')))\config\libraryfolders.vdf" | 
ForEach-Object { 
    if ($_ -like '*"path"*') {
        [string]$Path = "$(([string]$_).Trim().Trim('"path"').Trim().Trim('"').Replace("\\", "\"))\steamapps\common\Titanfall2" 
        @(
            "bin", 
            "R2Northstar", 
            "debug.log", 
            "LEGAL.txt", 
            "Northstar.dll", 
            "Northstar.zip", 
            "NorthstarLauncher.exe", 
            "NorthstarUpdater.exe", 
            "ns_startup_args.txt", 
            "ns_startup_args_dedi.txt", 
            "r2ds.bat"
        ) | 
        ForEach-Object {
            try { Remove-Item "$Path\$_" -Recurse -Force }
            catch {}
        }
    }
}
```

### Origin/EA App
Run the following script in PowerShell in the root of Titanfall 2's installation directory.

```powershell
@(
    "bin", 
    "R2Northstar", 
    "debug.log", 
    "LEGAL.txt", 
    "Northstar.dll", 
    "Northstar.zip", 
    "NorthstarLauncher.exe", 
    "NorthstarUpdater.exe", 
    "ns_startup_args.txt", 
    "ns_startup_args_dedi.txt", 
    "r2ds.bat"
) | 
ForEach-Object {
    try { Remove-Item "$Path\$_" -Recurse -Force }
    catch {}
}
```

### Manual
```
bin
R2Northstar
debug.log
LEGAL.txt 
Northstar.dll 
Northstar.zip
NorthstarLauncher.exe
NorthstarUpdater.exe
ns_startup_args.txt 
ns_startup_args_dedi.txt
r2ds.bat
```
Delete the following files/folders from Titanfall 2's installation directory.

> [!IMPORTANT]
> Once you uninstall Northstar, make sure to repair the game via Steam, Origin or the EA App.<br>
> Since the uninstallation methods mentioned here, also remove certain game files.

## Building
1. Download the following:
    - [.NET SDK](https://dotnet.microsoft.com/en-us/download)
    - [.NET Framework 4.8.1 Developer Pack](https://dotnet.microsoft.com/en-us/download/dotnet-framework/thank-you/net481-developer-pack-offline-installer)

2. Run the following to compile:
    
    ```cmd
    dotnet publish -c Release
    ```