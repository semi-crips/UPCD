using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using UPCD.App.ViewModels;

namespace UPCD.App;

public partial class MainWindow : Window
{
    private const string GitHubUrl = "https://github.com/semi-crips/UPCD";
    private const string TelegramUrl = "https://t.me/+4Eh06gmqGhg5NjM6";

    private readonly MainViewModel _vm = new();
    private bool _initializing;

    public MainWindow()
    {
        InitializeComponent();
        DataContext = _vm;
        Loaded += async (_, _) =>
        {
            _initializing = true;
            await _vm.InitializeAsync();
            SelectByTag(LanguageBox, _vm.Language);
            SelectByTag(ThemeBox, _vm.Theme);
            SelectByTag(ModeBox, _vm.ExperienceMode);
            _initializing = false;
            _vm.ApplyTheme();

            // Open the project page once on the first launch for this Windows user.
            if (_vm.ShouldOpenGitHubOnFirstLaunch)
            {
                try
                {
                    OpenExternalLink(GitHubUrl);
                    await _vm.MarkGitHubOpenedAsync();
                }
                catch
                {
                    _vm.Status = _vm.IsEnglish
                        ? "Could not open the GitHub page. Use the GitHub link in the sidebar."
                        : "Не удалось открыть GitHub автоматически. Используйте ссылку в боковом меню.";
                }
            }
        };
    }

    private static void SelectByTag(ComboBox box, string tag)
    {
        box.SelectedItem = box.Items.Cast<ComboBoxItem>()
            .FirstOrDefault(x => string.Equals(x.Tag?.ToString(), tag, StringComparison.OrdinalIgnoreCase));
    }

    private void Setting_Changed(object sender, SelectionChangedEventArgs e)
    {
        if (_initializing) return;
        if (LanguageBox.SelectedValue is string lang) _vm.Language = lang;
        if (ThemeBox.SelectedValue is string theme) _vm.Theme = theme;
        if (ModeBox.SelectedValue is string mode) _vm.ExperienceMode = mode;
        _vm.ApplyTheme();
    }

    private async void Save_Click(object sender, RoutedEventArgs e)
    {
        await _vm.SaveSettingsAsync();
    }

    private void Accent_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: string color })
        {
            _vm.AccentHex = color;
            _vm.ApplyTheme();
        }
    }

    private void Navigation_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: string section })
        {
            _vm.SelectedSection = section;
            _vm.Status = _vm.IsEnglish
                ? $"The {section} module is planned; this preview does not perform hardware operations."
                : $"Модуль «{section}» запланирован; эта версия пока не выполняет операции с оборудованием.";
        }
    }

    private void Scan_Click(object sender, RoutedEventArgs e)
    {
        _vm.Status = _vm.IsEnglish
            ? "Hardware scanning is not implemented in this foundation preview."
            : "Определение оборудования ещё не реализовано в этой предварительной версии.";
    }

    private static void OpenExternalLink(string url)
    {
        Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
    }

    private void Telegram_Click(object sender, RoutedEventArgs e) =>
        OpenExternalLink(TelegramUrl);

    private void GitHub_Click(object sender, RoutedEventArgs e) =>
        OpenExternalLink(GitHubUrl);
}
