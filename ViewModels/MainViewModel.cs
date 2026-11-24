using System.Windows;
using System.Windows.Media;
using UPCD.App.Models;
using UPCD.App.Services;

namespace UPCD.App.ViewModels;

public sealed class MainViewModel
{
    private readonly SettingsService _settingsService = new();
    private readonly UpdateService _updateService = new();
    private AppSettings _settings = new();

    public string Language { get => _settings.Language; set { _settings.Language = value; RefreshLabels(); } }
    public string Theme { get => _settings.Theme; set { _settings.Theme = value; } }
    public string ExperienceMode { get => _settings.ExperienceMode; set { _settings.ExperienceMode = value; } }
    public string AccentHex { get => _settings.AccentHex; set { _settings.AccentHex = value; } }
    public bool AutoCheckUpdates { get => _settings.AutoCheckUpdates; set => _settings.AutoCheckUpdates = value; }
    public bool DownloadUpdatesOnWifi { get => _settings.DownloadUpdatesOnWifi; set => _settings.DownloadUpdatesOnWifi = value; }
    public bool NotifyAboutUpdates { get => _settings.NotifyAboutUpdates; set => _settings.NotifyAboutUpdates = value; }
    public bool InstallAfterConfirmation { get => _settings.InstallAfterConfirmation; set => _settings.InstallAfterConfirmation = value; }

    public bool IsEnglish => Language == "en";
    public string SelectedSection { get; set; } = "Home";
    public string Status { get; set; } = "";
    public string UpdateStatus => _updateService.GetReadinessMessage(IsEnglish);

    public string[] Navigation => IsEnglish
        ? ["Home", "Hardware", "Diagnostics", "Monitoring", "Windows errors", "BIOS guide", "History", "Settings", "Feedback"]
        : ["Главная", "Комплектующие", "Диагностика", "Мониторинг", "Ошибки Windows", "Помощь с BIOS", "История", "Настройки", "Обратная связь"];

    public string[] AccentOptions => ["#398BFA", "#9567E8", "#32B889", "#E45D70", "#E6A548", "#35BBD0", "#DC78A8", "#D9E0EA"];
    public string Subtitle => "Diagnose. Understand. Repair.";
    public string WelcomeLabel => IsEnglish ? "Welcome to Universal PC Doctor" : "Добро пожаловать в Universal PC Doctor";
    public string StatusLabel => IsEnglish ? "BETA PREVIEW" : "БЕТА-ПРЕДВАРИТЕЛЬНАЯ ВЕРСИЯ";
    public string ScanLabel => IsEnglish ? "Start diagnostics" : "Начать диагностику";
    public string SettingsHint => IsEnglish ? "This foundation preview is a starting point. Hardware scanning will be added in later builds." : "Это основа будущей программы. Реальное определение оборудования появится в следующих версиях.";
    public string SettingsLabel => IsEnglish ? "Settings" : "Настройки";
    public string LanguageLabel => IsEnglish ? "Language" : "Язык";
    public string ThemeLabel => IsEnglish ? "Theme" : "Тема";
    public string ModeLabel => IsEnglish ? "Experience mode" : "Режим интерфейса";
    public string AccentLabel => IsEnglish ? "Accent color" : "Цвет акцента";
    public string UpdatesLabel => IsEnglish ? "Updates" : "Обновления";
    public string AutoCheckLabel => IsEnglish ? "Check for updates automatically" : "Автоматически проверять обновления";
    public string WifiDownloadLabel => IsEnglish ? "Download updates over Wi-Fi" : "Загружать обновления по Wi-Fi";
    public string NotifyUpdatesLabel => IsEnglish ? "Notify when updates are available" : "Уведомлять о новых обновлениях";
    public string ConfirmInstallLabel => IsEnglish ? "Install only after my confirmation" : "Устанавливать только после моего подтверждения";
    public string ConfirmInstallLabelUnused => ConfirmInstallLabel;
    public string SaveLabel => IsEnglish ? "Save settings" : "Сохранить настройки";

    public async Task InitializeAsync()
    {
        _settings = await _settingsService.LoadAsync();
        RefreshLabels();
    }

    public async Task SaveSettingsAsync()
    {
        await _settingsService.SaveAsync(_settings);
        Status = IsEnglish ? "Settings saved." : "Настройки сохранены.";
    }

    public void ApplyTheme()
    {
        var app = Application.Current;
        if (app is null) return;
        var dark = Theme == "dark" || (Theme == "system" && IsSystemDark());
        app.Resources["WindowBackground"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString(dark ? "#10151D" : "#EEF2F7"));
        app.Resources["PanelBackground"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString(dark ? "#B51D2734" : "#DDFBFDFF"));
        app.Resources["PanelBorder"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString(dark ? "#45FFFFFF" : "#8093A4B8"));
        app.Resources["PrimaryText"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString(dark ? "#F3F6FA" : "#172333"));
        app.Resources["SecondaryText"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString(dark ? "#AAB5C3" : "#59697A"));
        app.Resources["AccentBrush"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString(AccentHex));
    }

    private static bool IsSystemDark()
    {
        try { return Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize")?.GetValue("AppsUseLightTheme") is int v && v == 0; }
        catch { return true; }
    }

    private void RefreshLabels() { }
}
