# Quick Reference - Euchre Installer Commands

## Building the Installer

### Build the Main Application (Release)
```powershell
cd E:\Develop\Euchre
dotnet build -c Release
```

### Build the MSI Installer
```powershell
cd E:\Develop\Euchre
wix build Euchre.Setup\Product.wxs -arch x64 -o Euchre.Setup\bin\Release\Euchre.msi
```

### Build Both (Main App + MSI)
```powershell
cd E:\Develop\Euchre
dotnet build -c Release
wix build Euchre.Setup\Product.wxs -arch x64 -o Euchre.Setup\bin\Release\Euchre.msi
```

## Installing the Application

### Interactive Installation (End User)
```powershell
# Double-click the MSI file in Windows Explorer
Euchre.Setup\bin\Release\Euchre.msi
```

### Silent Installation (System Administrator)
```powershell
msiexec /i Euchre.Setup\bin\Release\Euchre.msi /qn
```

### Installation with Logging
```powershell
msiexec /i Euchre.Setup\bin\Release\Euchre.msi /l*v euchre_install.log
```

## Uninstalling the Application

### Interactive Uninstall
1. Open Settings → Apps → Apps & Features
2. Search for "Euchre"
3. Click Uninstall

### Command Line Uninstall (Silent)
```powershell
msiexec /x Euchre.Setup\bin\Release\Euchre.msi /qn
```

## Validating the Installer

### Validate MSI Integrity
```powershell
wix msi validate Euchre.Setup\bin\Release\Euchre.msi
```

### Get MSI Information
```powershell
msiexec /i Euchre.Setup\bin\Release\Euchre.msi /help
```

## WiX Toolset Commands

### Accept EULA (One-time setup)
```powershell
wix eula accept wix7
```

### Check WiX Version
```powershell
wix --version
```

### Get Help on WiX Build
```powershell
wix build --help
```

## File Locations

### Installer Output
```
E:\Develop\Euchre\Euchre.Setup\bin\Release\Euchre.msi
```

### Installation Directory (After Install)
```
C:\Program Files\Euchre\
```

### User Settings (After Install)
```
%APPDATA%\CrazyCanuckCoder\Euchre\
```

### Documentation in Repository
```
E:\Develop\Euchre\INSTALL.md           (Installation guide)
E:\Develop\Euchre\INSTALLER_PLAN.md    (Planning details)
E:\Develop\Euchre\INSTALLER_SUMMARY.md (What was done)
E:\Develop\Euchre\COMPLETION_CHECKLIST.md (Status check)
```

## Troubleshooting Commands

### Clean Build (Remove Previous Build)
```powershell
cd E:\Develop\Euchre
dotnet clean -c Release
dotnet build -c Release
```

### Verify Build Output
```powershell
Get-Item E:\Develop\Euchre\bin\Release\net10.0-windows7.0\Euchre.exe
```

### List All Installed WiX Extensions
```powershell
wix extension list
```

### Check System PATH for WiX
```powershell
$env:PATH -split ';' | Where-Object {$_ -match 'wix'}
```

## Advanced Options

### Install to Custom Location
```powershell
msiexec /i Euchre.msi INSTALLFOLDER="C:\CustomPath\Euchre\" /qn
```

### Repair Existing Installation
```powershell
msiexec /f Euchre.msi
```

### Reinstall (Uninstall then Install)
```powershell
msiexec /x Euchre.msi /qn
msiexec /i Euchre.msi /qn
```

## Version Information

- **Installer Version**: 1.0.0.0
- **WiX Toolset**: v7.0.0
- **Target Framework**: .NET 10
- **Architecture**: 64-bit (x64)
- **Platform**: Windows 7 SP1 and later

## Repository Information

- **Repository**: https://github.com/CrazyCanuckCoder/CCCEuchre
- **Branch**: CreateInstallation
- **License**: Apache License 2.0

## Common Tasks

### Update Version Number (When Releasing New Version)

1. Edit `Euchre.Setup\Product.wxs`
2. Change line: `<Package Name="Euchre" ... Version="1.0.0.0"` to new version
3. Change line: `<RegistryValue Type="string" Name="DisplayVersion" Value="1.0.0.0"` to match
4. Rebuild MSI with updated version

### Create Versioned Installer Filename

```powershell
$version = "1.0.0"
Copy-Item "Euchre.Setup\bin\Release\Euchre.msi" "Euchre-$version-Setup.msi"
```

## Performance Tips

- Build time is typically 30-60 seconds
- MSI validation adds ~10 seconds
- First WiX build may take longer due to tool setup
- Release build of main application: ~5 seconds

## Environment Variables

### Set for Current Session
```powershell
$env:WixUI = "WixUI_InstallDir"
```

### Permanent (in System Properties)
- WIX_TOOLSET_ROOT
- WIX_SDK_PATH (if custom SDK location)

---

## Quick Links

- WiX Toolset Documentation: https://wixtoolset.org/
- .NET 10 Download: https://dotnet.microsoft.com/download/dotnet/10.0
- Apache License 2.0: https://www.apache.org/licenses/LICENSE-2.0

---

**Last Updated**: May 25, 2026
**For**: Euchre Card Game Installation
**Status**: Ready for Production Use
