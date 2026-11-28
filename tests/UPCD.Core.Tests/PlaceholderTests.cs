using System.Text.Json;
using UPCD.App.Models;
using Xunit;

namespace UPCD.Core.Tests;

public sealed class AppSettingsTests
{
    [Fact]
    public void NewSettings_UseSafeExpectedDefaults()
    {
        var settings = new AppSettings();

        Assert.Equal("ru", settings.Language);
        Assert.Equal("system", settings.Theme);
        Assert.Equal("simple", settings.ExperienceMode);
        Assert.Equal("#398BFA", settings.AccentHex);
        Assert.True(settings.InstallAfterConfirmation);
        Assert.False(settings.HasOpenedGitHubOnFirstLaunch);
    }

    [Fact]
    public void Settings_SurviveJsonRoundTrip()
    {
        var original = new AppSettings
        {
            Language = "en",
            Theme = "dark",
            ExperienceMode = "advanced",
            AccentHex = "#9567E8",
            AutoCheckUpdates = false,
            DownloadUpdatesOnWifi = false,
            NotifyAboutUpdates = false,
            InstallAfterConfirmation = true
        };

        var json = JsonSerializer.Serialize(original);
        var restored = JsonSerializer.Deserialize<AppSettings>(json);

        Assert.NotNull(restored);
        Assert.Equal(original.Language, restored.Language);
        Assert.Equal(original.Theme, restored.Theme);
        Assert.Equal(original.ExperienceMode, restored.ExperienceMode);
        Assert.Equal(original.AccentHex, restored.AccentHex);
        Assert.Equal(original.AutoCheckUpdates, restored.AutoCheckUpdates);
        Assert.Equal(original.DownloadUpdatesOnWifi, restored.DownloadUpdatesOnWifi);
        Assert.Equal(original.NotifyAboutUpdates, restored.NotifyAboutUpdates);
        Assert.Equal(original.InstallAfterConfirmation, restored.InstallAfterConfirmation);
    }
}
