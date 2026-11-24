namespace UPCD.App.Models;

public sealed class AppSettings
{
    public string Language { get; set; } = "ru";
    public string Theme { get; set; } = "system";
    public string ExperienceMode { get; set; } = "simple";
    public string AccentHex { get; set; } = "#398BFA";
    public bool AutoCheckUpdates { get; set; } = true;
    public bool DownloadUpdatesOnWifi { get; set; } = true;
    public bool NotifyAboutUpdates { get; set; } = true;
    public bool InstallAfterConfirmation { get; set; } = true;
    public bool HasOpenedGitHubOnFirstLaunch { get; set; }
}
