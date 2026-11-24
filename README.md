# Universal PC Doctor — 0.1.0 Foundation Preview

**Diagnose. Understand. Repair.**

Universal PC Doctor is an early Windows desktop project intended to grow into a diagnostic assistant for home PCs, gaming PCs, workstations, and (in later stages) servers.

> **Status: very early foundation preview.** This is not yet a working PC diagnostic suite. Hardware inventory, sensor monitoring, diagnostic tests, Windows log analysis, and repair workflows are not implemented.

## Current foundation

- C# / .NET 10 and WPF desktop application
- English and Russian interface labels
- Simple and Advanced interface modes (initial settings foundation)
- System, Dark, and Light theme selection
- Accent color palette
- Local settings persistence under `%LOCALAPPDATA%\UniversalPCDoctor\settings.json`
- First-launch GitHub page opening (once per Windows user profile)\n- Update preference settings and Wi-Fi/network readiness display; no update feed is configured and no update is downloaded or installed
- Telegram and GitHub community links in the lower-left sidebar

## Planned development

1. Reliable hardware inventory with user confirmation
2. Sensor monitoring for temperatures, clocks, fan and pump speeds where hardware exposes them
3. Diagnostics for CPU, GPU, RAM, storage, Windows, and drivers
4. Windows Event Log and crash-dump analysis
5. Safe repair guidance with explicit consent, audit trail, and rollback considerations
6. Signed, user-confirmed updates
7. Privacy-conscious feedback and diagnostic report export

## Safety principles

- Read-only diagnostics by default.
- Never invent sensor readings or claim a diagnosis without evidence.
- No automatic BIOS, voltage, frequency, or driver changes.
- Each risky action requires separate explicit user consent.
- Update installation remains user-confirmed; no update endpoint is configured in this preview.

## Requirements

- Windows 10/11
- .NET 10 SDK
- Visual Studio 2026 with the .NET desktop development workload (optional)

## Build and run

```powershell
dotnet restore .\UniversalPCDoctor.slnx
dotnet build .\UniversalPCDoctor.slnx
dotnet run --project .\src\UPCD.App\UPCD.App.csproj
dotnet test .\UniversalPCDoctor.slnx
```

## Community

- Telegram: https://t.me/+4Eh06gmqGhg5NjM6
- GitHub: https://github.com/semi-crips/UPCD

Contributions and bug reports are welcome. Please remember that this is an experimental project and should not be relied on as a repair tool yet.
