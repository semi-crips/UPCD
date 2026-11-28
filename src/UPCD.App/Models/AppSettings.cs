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

    /// <summary>
    /// Restores safe defaults for missing or invalid values loaded from disk.
    /// </summary>
    public void Normalize()
    {
        if (Language is not ("ru" or "en"))
            Language = "ru";

        if (Theme is not ("system" or "dark" or "light"))
            Theme = "system";

        if (ExperienceMode is not ("simple" or "advanced"))
            ExperienceMode = "simple";

        if (!IsValidAccentHex(AccentHex))
            AccentHex = "#398BFA";
    }

    private static bool IsValidAccentHex(string? value)
    {
        if (value is null || value.Length != 7 || value[0] != '#')
            return false;

        for (var i = 1; i < value.Length; i++)
        {
            var c = value[i];
            if (!((c >= '0' && c <= '9') ||
                  (c >= 'A' && c <= 'F') ||
                  (c >= 'a' && c <= 'f')))
                return false;
        }

        return true;
    }
}
