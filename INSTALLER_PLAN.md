# CCCEuchre Installation Executable Plan

## Executive Summary
Create a professional Windows installer for the Euchre application that handles installation, shortcuts, and uninstallation. The application is a WPF-based card game (.NET 10, Windows-only) that requires packaging for end-user distribution.

## Project Context
- **Application Type**: WPF Desktop Application (Windows only)
- **Target Framework**: .NET 10 (net10.0-windows7.0)
- **Main Assembly**: Euchre.exe
- **Dependencies**: 
  - Euchre.Logic.dll
  - CrazyCanuckCoder libraries (Common, WPF, WPFControls)
  - System packages (Markdown.Xaml, Microsoft.Extensions.*, Newtonsoft.Json)
- **Assets**: Rich set of card images, icons, and documentation
- **Repository**: https://github.com/CrazyCanuckCoder/CCCEuchre

## Installer Options Comparison

### Option 1: WiX Toolset (Recommended for Professional Installers)
**Pros:**
- Industry standard for Windows installers
- Full control over installation process
- Creates standard .msi files
- Professional appearance and behavior
- Supports upgrades, repairs, and uninstall
- Can be integrated into CI/CD

**Cons:**
- Steeper learning curve
- Requires WiX installation
- More initial setup required

### Option 2: NSIS (Nullsoft Scriptable Install System)
**Pros:**
- Lightweight and easy to use
- Creates .exe installers (single file)
- Good scripting capabilities
- Small installer file sizes
- No dependencies to install

**Cons:**
- Requires NSIS installation
- Less integrated with Windows
- Requires separate script language

### Option 3: Visual Studio Installer Projects
**Pros:**
- Built into Visual Studio
- Easy setup
- Good for simple applications

**Cons:**
- Deprecated in newer Visual Studio versions
- Limited capabilities
- Not recommended for new projects

### Recommended Approach: **WiX Toolset**
This plan uses WiX as it provides professional-grade installers that meet industry standards.

## Implementation Approach

The plan involves:
1. **Setup Infrastructure**: Install WiX tools and create a new WiX setup project
2. **Build Configuration**: Configure Release build settings for the main application
3. **Feature Definition**: Define installer components (binaries, resources, shortcuts)
4. **Dialog Customization**: Create a branded installer UI
5. **MSI Packaging**: Build the .msi installer
6. **Testing & Validation**: Test install, uninstall, and shortcut creation
7. **Distribution Preparation**: Document deployment instructions

## Key Files to Create/Modify

- `Euchre.Setup/` - New WiX setup project directory
- `Euchre.Setup/Product.wxs` - Main WiX product definition
- `Euchre.Setup/Euchre.Setup.wixproj` - WiX project file
- `Euchre.Setup/License.rtf` - License agreement for installer
- `Euchre.sln` - Update to include new setup project
- Build scripts for automation

## Installation Features

### Included in Installer
- Main application executable and binaries
- All dependencies (DLLs)
- Documentation files (About.txt, HowToPlay.md, LICENSE.txt)
- Application icon and resources
- Desktop shortcut
- Start Menu folder with shortcuts
- Add/Remove Programs entry
- Uninstaller

### System Requirements
- Windows 7 Service Pack 1 or later
- .NET Runtime 10 (will be installed via bootstrapper or prerequisite)
- Approximately 50-100 MB disk space

## Steps

1. **Install WiX Toolset**
   - Download and install WiX Toolset v4 (or v3)
   - Install Visual Studio extension for WiX support
   - Verify installation in Visual Studio

2. **Create WiX Setup Project**
   - Create new directory: `Euchre.Setup`
   - Add new WiX project file: `Euchre.Setup.wixproj`
   - Create project structure with necessary directories

3. **Create Product Definition File (Product.wxs)**
   - Define product metadata (name, version, manufacturer, URL)
   - Specify installation directory structure
   - Configure file harvesting from build output
   - Define shortcut creation on desktop and Start Menu
   - Add registry entries for Add/Remove Programs

4. **Create License File**
   - Convert or create License.rtf from LICENSE.txt
   - Include in installer as EULA

5. **Configure Build Dependencies**
   - Update Euchre.sln to include setup project
   - Set build order: Build Euchre project first, then setup
   - Configure Release build profile

6. **Create Directory Structure**
   - Organize source files in WiX project
   - Set up heat.exe heat generation for file discovery (optional)
   - Define feature structure (Main application, documentation, etc.)

7. **Build and Test Installer**
   - Build the WiX project to generate .msi file
   - Test installation on clean machine or VM
   - Verify shortcuts are created
   - Test uninstallation
   - Check Add/Remove Programs entry

8. **Create Distribution Package**
   - Document installer filename convention (e.g., Euchre-1.0.0-Setup.msi)
   - Create release notes document
   - Set up automated build process (optional)

9. **Document Installation Instructions**
   - Create INSTALL.md file
   - Include system requirements
   - Installation steps for end users
   - Troubleshooting guide

10. **Setup Continuous Integration (Optional)**
	- Configure GitHub Actions to build MSI on releases
	- Upload MSI to GitHub releases
	- Document versioning strategy

## Risks & Open Questions

- **Question**: What is the current version number for the application? This will be needed in the installer metadata.
- **Question**: Should the installer support upgrading previous versions, or will it require uninstall first?
- **Question**: Is .NET Runtime 10 expected to be pre-installed, or should the installer bundle/check for it?
- **Risk**: The custom library references (CrazyCanuckCoder libraries) are loaded from external paths. Ensure these are available during build, or update paths.
- **Risk**: WiX has a learning curve; consider dedicating time for tool familiarization.

## Post-Implementation

After completing the installer:
- Add versioning workflow to match app version updates
- Consider code signing the MSI for trusted distribution
- Create a deployment checklist before releases
- Monitor user feedback on installation process
