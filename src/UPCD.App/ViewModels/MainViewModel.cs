using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using UPCD.App.Models;
using UPCD.App.Services;

namespace UPCD.App.ViewModels;

public sealed class MainViewModel : INotifyPropertyChanged
{
    private readonly SettingsService _settingsService = new();
    private readonly UpdateService _updateService = new();
    private readonly HardwareInventoryService _hardwareInventory = new();
    private bool _isScanning;
    private bool _hasScannedHardware;
    private AppSettings _settings = new();
    private string _selectedSection = "Главная";
    private string _status = "";

    public event PropertyChangedEventHandler? PropertyChanged;

    public ObservableCollection<HardwareItem> HardwareItems { get; } = [];
    public bool IsScanning { get => _isScanning; private set { if (_isScanning == value) return; _isScanning = value; OnPropertyChanged(); OnPropertyChanged(nameof(ScanLabel)); } }
    public string HardwareSummary => HardwareItems.Count == 0 ? (IsEnglish ? "No inventory entries" : "Сведения об оборудовании не получены") : (IsEnglish ? $"Found {HardwareItems.Count} hardware and system entries" : $"Получено записей об оборудовании и системе: {HardwareItems.Count}");
    public string CpuSummary => GetCategorySummary("CPU");
    public string GpuSummary => GetCategorySummary("GPU");
    public string MemorySummary => GetMemorySummary();

    private string GetCategorySummary(string category)
    {
        if (IsScanning) return IsEnglish ? "Scanning…" : "Сканирование…";
        if (!_hasScannedHardware) return IsEnglish ? "Not scanned" : "Не проверено";
        var item = HardwareItems.FirstOrDefault(x => x.Category.Equals(category, StringComparison.OrdinalIgnoreCase));
        return item?.Name ?? (IsEnglish ? "Not detected" : "Не обнаружено");
    }

    private string GetMemorySummary()
    {
        if (IsScanning) return IsEnglish ? "Scanning…" : "Сканирование…";
        if (!_hasScannedHardware) return IsEnglish ? "Not scanned" : "Не проверено";
        var count = HardwareItems.Count(x => x.Category.Equals("RAM", StringComparison.OrdinalIgnoreCase));
        if (count == 0) return IsEnglish ? "Not detected" : "Не обнаружено";
        if (count == 1 && HardwareItems[0].Name.Equals("Installed memory (total)", StringComparison.OrdinalIgnoreCase))
            return HardwareItems[0].Details;
        return IsEnglish ? $"{count} module(s) detected" : $"Обнаружено модулей: {count}";
    }

    public string Language
    {
        get => _settings.Language;
        set
        {
            if (_settings.Language == value) return;
            _settings.Language = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsEnglish));
            RefreshLabels();
        }
    }

    public string Theme { get => _settings.Theme; set { if (_settings.Theme == value) return; _settings.Theme = value; OnPropertyChanged(); } }
    public string ExperienceMode { get => _settings.ExperienceMode; set { if (_settings.ExperienceMode == value) return; _settings.ExperienceMode = value; OnPropertyChanged(); } }
    public string AccentHex { get => _settings.AccentHex; set { if (_settings.AccentHex == value) return; _settings.AccentHex = value; OnPropertyChanged(); } }
    public bool AutoCheckUpdates { get => _settings.AutoCheckUpdates; set { if (_settings.AutoCheckUpdates == value) return; _settings.AutoCheckUpdates = value; OnPropertyChanged(); } }
    public bool DownloadUpdatesOnWifi { get => _settings.DownloadUpdatesOnWifi; set { if (_settings.DownloadUpdatesOnWifi == value) return; _settings.DownloadUpdatesOnWifi = value; OnPropertyChanged(); } }
    public bool NotifyAboutUpdates { get => _settings.NotifyAboutUpdates; set { if (_settings.NotifyAboutUpdates == value) return; _settings.NotifyAboutUpdates = value; OnPropertyChanged(); } }
    public bool InstallAfterConfirmation { get => _settings.InstallAfterConfirmation; set { if (_settings.InstallAfterConfirmation == value) return; _settings.InstallAfterConfirmation = value; OnPropertyChanged(); } }

    public bool IsEnglish => Language == "en";
    public string SelectedSection { get => _selectedSection; set { if (_selectedSection == value) return; _selectedSection = value; OnPropertyChanged(); } }
    public string Status { get => _status; set { if (_status == value) return; _status = value; OnPropertyChanged(); } }
    public string UpdateStatus => _updateService.GetReadinessMessage(IsEnglish);
    public bool ShouldOpenGitHubOnFirstLaunch => !_settings.HasOpenedGitHubOnFirstLaunch;

    public async Task MarkGitHubOpenedAsync()
    {
        _settings.HasOpenedGitHubOnFirstLaunch = true;
        await _settingsService.SaveAsync(_settings);
    }

    public string[] Navigation => IsEnglish
        ? ["Home", "Hardware", "Diagnostics", "Monitoring", "Windows errors", "BIOS guide", "History", "Settings", "Feedback"]
        : ["Главная", "Комплектующие", "Диагностика", "Мониторинг", "Ошибки Windows", "Помощь с BIOS", "История", "Настройки", "Обратная связь"];

    public string[] AccentOptions => ["#398BFA", "#9567E8", "#32B889", "#E45D70", "#E6A548", "#35BBD0", "#DC78A8", "#D9E0EA"];
    public string Subtitle => "Diagnose. Understand. Repair.";
    public string WelcomeLabel => IsEnglish ? "Welcome to Universal PC Doctor" : "Добро пожаловать в Universal PC Doctor";
    public string StatusLabel => IsEnglish ? "BETA PREVIEW" : "БЕТА-ПРЕДВАРИТЕЛЬНАЯ ВЕРСИЯ";
    public string ScanLabel => IsScanning ? (IsEnglish ? "Scanning…" : "Сканирование…") : (IsEnglish ? "Scan hardware" : "Определить оборудование");
    public string SettingsHint => IsEnglish
        ? "Read-only hardware inventory is available. Live sensors and diagnostic tests are still being developed."
        : "Доступен сбор сведений об оборудовании в режиме чтения. Мониторинг датчиков и диагностические тесты ещё разрабатываются.";
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
    public string SaveLabel => IsEnglish ? "Save settings" : "Сохранить настройки";

    public async Task InitializeAsync()
    {
        _settings = await _settingsService.LoadAsync();
        OnPropertyChanged(string.Empty);
    }

    public async Task ScanHardwareAsync()
    {
        if (IsScanning) return;
        IsScanning = true;
        Status = IsEnglish ? "Reading hardware information from Windows…" : "Получаем сведения об оборудовании из Windows…";
        HardwareItems.Clear();
        _hasScannedHardware = false;
        OnPropertyChanged(nameof(HardwareSummary));
        OnPropertyChanged(nameof(CpuSummary));
        OnPropertyChanged(nameof(GpuSummary));
        OnPropertyChanged(nameof(MemorySummary));
        try
        {
            var found = await _hardwareInventory.ScanAsync();
            foreach (var item in found) HardwareItems.Add(item);
            _hasScannedHardware = true;
            OnPropertyChanged(nameof(CpuSummary));
            OnPropertyChanged(nameof(GpuSummary));
            OnPropertyChanged(nameof(MemorySummary));
            Status = IsEnglish ? "Read-only hardware inventory completed. Check the detected values before relying on them." : "Сбор сведений завершён в режиме только чтения. Проверьте определённые значения.";
        }
        catch (Exception ex)
        {
            Status = (IsEnglish ? "Hardware scan failed: " : "Ошибка сканирования: ") + ex.Message;
        }
        finally
        {
            IsScanning = false;
            OnPropertyChanged(nameof(HardwareSummary));
            OnPropertyChanged(nameof(CpuSummary));
            OnPropertyChanged(nameof(GpuSummary));
            OnPropertyChanged(nameof(MemorySummary));
        }
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
        app.Resources["WindowBackground"] = Brush(dark ? "#10151D" : "#EEF2F7");
        app.Resources["PanelBackground"] = Brush(dark ? "#B51D2734" : "#DDFBFDFF");
        app.Resources["PanelBorder"] = Brush(dark ? "#45FFFFFF" : "#8093A4B8");
        app.Resources["PrimaryText"] = Brush(dark ? "#F3F6FA" : "#172333");
        app.Resources["SecondaryText"] = Brush(dark ? "#AAB5C3" : "#59697A");
        app.Resources["AccentBrush"] = Brush(AccentHex);
    }

    private static SolidColorBrush Brush(string hex) =>
        new((Color)ColorConverter.ConvertFromString(hex));

    private static bool IsSystemDark()
    {
        try
        {
            return Microsoft.Win32.Registry.CurrentUser
                .OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize")
                ?.GetValue("AppsUseLightTheme") is int value && value == 0;
        }
        catch
        {
            return true;
        }
    }

    private void RefreshLabels()
    {
        foreach (var property in new[]
        {
            nameof(Navigation), nameof(IsEnglish), nameof(UpdateStatus), nameof(HardwareSummary), nameof(CpuSummary), nameof(GpuSummary), nameof(MemorySummary), nameof(WelcomeLabel),
            nameof(StatusLabel), nameof(ScanLabel), nameof(SettingsHint), nameof(SettingsLabel),
            nameof(LanguageLabel), nameof(ThemeLabel), nameof(ModeLabel), nameof(AccentLabel),
            nameof(UpdatesLabel), nameof(AutoCheckLabel), nameof(WifiDownloadLabel),
            nameof(NotifyUpdatesLabel), nameof(ConfirmInstallLabel), nameof(SaveLabel)
        })
            OnPropertyChanged(property);
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
