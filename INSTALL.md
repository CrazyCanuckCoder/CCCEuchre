# Euchre Installation Guide

## System Requirements

- **Operating System**: Windows 7 Service Pack 1 or later (Windows 10, Windows 11 recommended)
- **Processor**: 1 GHz or faster processor
- **RAM**: 512 MB minimum (1 GB recommended)
- **Disk Space**: Approximately 100-150 MB for installation
- **.NET Runtime**: .NET 10 runtime (installer will guide you if not present)

## Installation Instructions

### Method 1: Using the Installer (Recommended)

1. **Download the Installer**
   - Download `Euchre-1.0.0-Setup.msi` from the releases page

2. **Run the Installer**
   - Double-click `Euchre-1.0.0-Setup.msi` to launch the installation wizard
   - If prompted by User Account Control (UAC), click **Yes** to allow the installer to run

3. **Follow the Installation Wizard**
   - **Welcome**: Click **Next** to proceed
   - **Installation Folder**: Accept the default location `C:\Program Files\Euchre` or choose a different folder
   - **Ready to Install**: Review your settings and click **Install**
   - **Completing the Setup**: Click **Finish** when installation completes

4. **Launch the Application**
   - **Desktop Shortcut**: Double-click the "Euchre" icon on your desktop, or
   - **Start Menu**: Open the Start Menu and search for "Euchre", or
   - **Program Files**: Navigate to `C:\Program Files\Euchre\` and double-click `Euchre.exe`

### Method 2: Command Line Installation

For system administrators or batch installations:

```powershell
msiexec /i Euchre-1.0.0-Setup.msi /qn
```

**Installation Options:**
- `/qn` - Silent installation (no UI)
- `/qb` - Basic UI (progress bar only)
- `/l*v install.log` - Create detailed installation log

Example with logging:
```powershell
msiexec /i Euchre-1.0.0-Setup.msi /l*v euchre_install.log
```

## Uninstallation

### Method 1: Control Panel (Windows 10/11)

1. Open **Settings** → **Apps** → **Apps & Features**
2. Search for "Euchre"
3. Click on "Euchre" and select **Uninstall**
4. Click **Uninstall** again to confirm
5. Follow the uninstall wizard and click **Finish**

### Method 2: Control Panel (Windows 7)

1. Open **Control Panel** → **Programs** → **Programs and Features**
2. Find "Euchre" in the list
3. Click **Uninstall**
4. Follow the uninstall wizard

### Method 3: Command Line

```powershell
msiexec /x Euchre-1.0.0-Setup.msi /qn
```

## After Installation

### Getting Started

1. **First Launch**: The application will initialize on first run
2. **Create a New Game**: Click "New Game" to start playing
3. **Settings**: Configure game preferences and player options in the Settings menu

### Documentation

After installation, you can find documentation files in the installation folder:
- `About.txt` - Application information and credits
- `HowToPlay.md` - Game rules and how to play guide
- `LICENSE.txt` - Apache License 2.0

## Troubleshooting

### Installation Issues

**Problem: "The installer cannot continue. The installation may be corrupted."**

**Solution:**
1. Download the MSI file again to ensure it's not corrupted
2. Temporarily disable antivirus software during installation
3. Try installing from a different location (e.g., Desktop instead of Downloads)

**Problem: "You do not have sufficient privileges to complete this installation."**

**Solution:**
1. Run the installer as Administrator: Right-click the MSI file and select "Run as administrator"
2. Ensure your user account has Administrator privileges

**Problem: ".NET Runtime 10 not found"**

**Solution:**
1. Install .NET 10 runtime from https://dotnet.microsoft.com/download/dotnet/10.0
2. Restart your computer after installing .NET
3. Run the Euchre installer again

### Runtime Issues

**Problem: Application crashes on startup**

**Solution:**
1. Uninstall Euchre completely using Control Panel → Programs and Features
2. Restart your computer
3. Reinstall Euchre using the installer

**Problem: "The application cannot find required DLL files"**

**Solution:**
1. This usually indicates incomplete installation
2. Uninstall and reinstall the application
3. If the issue persists, check your antivirus is not quarantining files

## System Locations

- **Installation Directory**: `C:\Program Files\Euchre\`
- **Start Menu Folder**: `Start Menu\Programs\Euchre\`
- **Desktop Shortcut**: `[Desktop]\Euchre.lnk`
- **User Settings**: `%APPDATA%\CrazyCanuckCoder\Euchre\`

## Support

For issues, feature requests, or bug reports:
- Visit the GitHub repository: https://github.com/CrazyCanuckCoder/CCCEuchre
- Report issues on GitHub: https://github.com/CrazyCanuckCoder/CCCEuchre/issues

## Version Information

- **Current Version**: 1.0.0
- **Target Framework**: .NET 10
- **Architecture**: 64-bit
- **License**: Apache License 2.0

## Frequently Asked Questions (FAQ)

**Q: Can I install Euchre on a USB drive?**
A: Yes, but it requires additional setup. Use the command line installation method to specify the target directory.

**Q: Can multiple users on the same computer play?**
A: Yes, each user can launch the application independently. User settings are stored separately per user account.

**Q: Is there a portable version?**
A: Not currently. The MSI installer is the standard distribution method.

**Q: Can I run multiple instances of the application?**
A: Yes, you can launch multiple copies of Euchre.exe simultaneously.

## Release Notes

### Version 1.0.0

- Initial release
- Full Euchre card game implementation
- Multiplayer support
- Customizable game settings
- Player avatars and statistics tracking

## License

This software is licensed under the Apache License 2.0. See LICENSE.txt in the installation directory for details.

---

**Last Updated**: May 2026
**For the latest version and updates, visit**: https://github.com/CrazyCanuckCoder/CCCEuchre
