# Universal PC Doctor — Foundation Preview 0.1.0

This is the initial Windows desktop project scaffold, not a complete PC diagnostic utility.

## Technology
- C# / .NET 10
- WPF
- MVVM-oriented structure
- Local JSON settings in `%LOCALAPPDATA%\\UniversalPCDoctor\\settings.json`
- xUnit test project

## Requirements
- Windows 10/11
- .NET 10 SDK

## Run
Open `UniversalPCDoctor.slnx` in Visual Studio 2026 with the .NET desktop development workload, or run:

```powershell
dotnet run --project .\src\UPCD.App\UPCD.App.csproj
```

## Current functionality
- Initial WPF window and navigation shell
- English/Russian UI labels
- Simple/Advanced mode setting
- System/Dark/Light theme selector (system theme reads the Windows app-theme preference)
- Accent palette selector
- Local persistence for settings
- Update preferences and Wi-Fi/internet availability detection

## Not implemented yet
- Hardware inventory and confirmation workflow
- Sensor and fan RPM collection
- Stress tests and diagnostics
- Windows event log analysis
- Repair operations
- Signed update feed, download/installation workflow, and feedback backend

No hardware changes are made. The app does not request administrator privileges.

## Important engineering notes
- Do not present sensor values as real until a sensor provider reports them.
- Do not add automatic repair actions without per-action consent, logging, and rollback analysis.
- Before public beta, add automated tests, accessibility review, secure update signing, privacy review, and installer signing.

## Update safety
Update preferences are present and saved locally. The preview can detect whether a Wi-Fi interface and network connection are available. The signed update feed is intentionally not configured yet, so this build does not download or install updates. Before enabling real updates, the project needs a controlled HTTPS release feed, signed packages, signature/hash verification, staged rollout, and rollback testing. Installation must remain user-confirmed.
\n## Community links\n- Telegram channel: https://t.me/+4Eh06gmqGhg5NjM6\n- GitHub repository button is present as a placeholder until the repository URL is supplied.\n